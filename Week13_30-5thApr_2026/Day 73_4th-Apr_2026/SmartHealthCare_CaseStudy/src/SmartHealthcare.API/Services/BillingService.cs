using SmartHealthcare.API.Repositories.Interfaces;
using SmartHealthcare.API.Services.Interfaces;
using SmartHealthcare.Models.DTOs;
using SmartHealthcare.Models.Entities;

namespace SmartHealthcare.API.Services;

public class BillingService : IBillingService
{
    private readonly IBillingRepository _repo;
    private readonly IAppointmentRepository _appointmentRepo;
    private readonly ILogger<BillingService> _logger;

    
    private const decimal DefaultPerMedicineCharge = 50m;

    public BillingService(
        IBillingRepository repo,
        IAppointmentRepository appointmentRepo,
        ILogger<BillingService> logger)
    {
        _repo = repo;
        _appointmentRepo = appointmentRepo;
        _logger = logger;
    }

    public async Task<BillDTO?> GetByIdAsync(int id)
    {
        var bill = await _repo.GetByIdAsync(id);
        if (bill == null)
        {
            _logger.LogWarning("Bill {Id} not found", id);
            return null;
        }

        return MapToDTO(bill);
    }

    public async Task<BillDTO?> GetByAppointmentIdAsync(int appointmentId)
    {
        var bill = await _repo.GetByAppointmentIdAsync(appointmentId);
        if (bill == null)
        {
            _logger.LogWarning("No bill found for AppointmentId {AppointmentId}", appointmentId);
            return null;
        }

        return MapToDTO(bill);
    }

    public async Task<PagedResult<BillDTO>> GetAllAsync(int pageNumber, int pageSize)
    {
        var bills = await _repo.GetAllAsync(pageNumber, pageSize);
        var total = await _repo.CountAsync();

        return new PagedResult<BillDTO>
        {
            Items = bills.Select(MapToDTO).ToList(),
            TotalCount = total,
            PageNumber = pageNumber,
            PageSize = pageSize
        };
    }

    public async Task<PagedResult<BillDTO>> GetByPatientIdAsync(int patientId, int pageNumber, int pageSize)
    {
        var bills = await _repo.GetByPatientIdAsync(patientId, pageNumber, pageSize);
        var total = await _repo.CountByPatientAsync(patientId);

        return new PagedResult<BillDTO>
        {
            Items = bills.Select(MapToDTO).ToList(),
            TotalCount = total,
            PageNumber = pageNumber,
            PageSize = pageSize
        };
    }

    public async Task<BillDTO> GenerateBillAsync(GenerateBillDTO dto)
    {
        var appointment = await _appointmentRepo.GetWithDetailsAsync(dto.AppointmentId);
        if (appointment == null)
        {
            _logger.LogError("Cannot generate bill — Appointment {AppointmentId} not found", dto.AppointmentId);
            throw new KeyNotFoundException($"Appointment {dto.AppointmentId} was not found.");
        }

        var existing = await _repo.GetByAppointmentIdAsync(dto.AppointmentId);
        if (existing != null)
        {
            _logger.LogWarning("Bill already exists for AppointmentId {AppointmentId} (BillId: {BillId})",
                dto.AppointmentId, existing.Id);
            throw new InvalidOperationException(
                $"A bill has already been generated for appointment {dto.AppointmentId}. " +
                $"Use the update endpoint to change the payment status.");
        }

        var consultationFee = appointment.Doctor.ConsultationFee;

        
        decimal medicineCharges;

        if (dto.MedicineChargesOverride.HasValue)
        {
            medicineCharges = dto.MedicineChargesOverride.Value;

            _logger.LogInformation(
                "Bill for Appointment {AppointmentId}: medicine charges overridden to {Charges}",
                dto.AppointmentId, medicineCharges);
        }
        else
        {
            var perMedicineRate = dto.PerMedicineCharge ?? DefaultPerMedicineCharge;
            var medicineCount = appointment.Prescription?.Medicines.Count ?? 0;
            medicineCharges = medicineCount * perMedicineRate;

            _logger.LogInformation(
                "Bill for Appointment {AppointmentId}: {Count} medicines × ₹{Rate} = ₹{Charges}",
                dto.AppointmentId, medicineCount, perMedicineRate, medicineCharges);
        }

        var totalAmount = consultationFee + medicineCharges;

        var bill = new Bill
        {
            AppointmentId = dto.AppointmentId,
            ConsultationFee = consultationFee,
            MedicineCharges = medicineCharges,
            TotalAmount = totalAmount,
            PaymentStatus = "Unpaid",
            GeneratedAt = DateTime.UtcNow,
            Notes = dto.Notes
        };

        await _repo.AddAsync(bill);
        await _repo.SaveAsync();

        _logger.LogInformation(
            "Bill {BillId} generated for AppointmentId {AppointmentId} — " +
            "Consultation: ₹{ConsultationFee}, Medicines: ₹{MedicineCharges}, Total: ₹{Total}",
            bill.Id, dto.AppointmentId, consultationFee, medicineCharges, totalAmount);

        var created = await _repo.GetByIdAsync(bill.Id);
        return MapToDTO(created!);
    }

    public async Task<bool> UpdatePaymentStatusAsync(int id, UpdateBillDTO dto)
    {
        var bill = await _repo.GetByIdAsync(id);
        if (bill == null)
        {
            _logger.LogWarning("UpdatePaymentStatus failed — Bill {Id} not found", id);
            return false;
        }

        var previousStatus = bill.PaymentStatus;
        bill.PaymentStatus = dto.PaymentStatus;
        bill.Notes = dto.Notes ?? bill.Notes;

        if (dto.PaymentStatus == "Paid" && previousStatus != "Paid")
        {
            bill.PaidAt = DateTime.UtcNow;
            _logger.LogInformation("Bill {BillId} marked as Paid at {PaidAt}", id, bill.PaidAt);
        }

        if (dto.PaymentStatus != "Paid" && previousStatus == "Paid")
        {
            bill.PaidAt = null;
            _logger.LogInformation("Bill {BillId} reverted from Paid to {Status}", id, dto.PaymentStatus);
        }

        _repo.Update(bill);
        await _repo.SaveAsync();

        return true;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var bill = await _repo.GetByIdAsync(id);
        if (bill == null)
        {
            _logger.LogWarning("Delete failed — Bill {Id} not found", id);
            return false;
        }

        _repo.Delete(bill);
        await _repo.SaveAsync();

        _logger.LogInformation("Bill {BillId} deleted", id);
        return true;
    }

    private static BillDTO MapToDTO(Bill bill)
    {
        var appointment = bill.Appointment;
        var medicines = appointment?.Prescription?.Medicines.ToList()
                        ?? new List<Medicine>();

        decimal perMedicineCharge = 0m;
        if (medicines.Count > 0 && bill.MedicineCharges > 0)
        {
            perMedicineCharge = Math.Round(bill.MedicineCharges / medicines.Count, 2);
        }

        var medicineLines = medicines.Select(m => new BillMedicineLineDTO
        {
            MedicineName = m.Name,
            Dosage = m.Dosage,
            Duration = m.Duration,
            Charge = perMedicineCharge
        }).ToList();

        if (medicineLines.Any())
        {
            var lineSum = medicineLines.Sum(l => l.Charge);
            var diff = bill.MedicineCharges - lineSum;
            if (diff != 0)
            {
                medicineLines[^1].Charge += diff;
            }
        }

        return new BillDTO
        {
            Id = bill.Id,
            AppointmentId = bill.AppointmentId,
            PatientId = appointment?.Patient?.Id ?? 0,
            PatientName = appointment?.Patient?.User?.FullName ?? "Unknown",
            DoctorId = appointment?.Doctor?.Id ?? 0,
            DoctorName = appointment?.Doctor?.User?.FullName ?? "Unknown",
            AppointmentDate = appointment?.AppointmentDate ?? DateTime.MinValue,
            ConsultationFee = bill.ConsultationFee,
            MedicineCharges = bill.MedicineCharges,
            TotalAmount = bill.TotalAmount,
            PaymentStatus = bill.PaymentStatus,
            GeneratedAt = bill.GeneratedAt,
            PaidAt = bill.PaidAt,
            Notes = bill.Notes,
            MedicineLines = medicineLines
        };
    }
}