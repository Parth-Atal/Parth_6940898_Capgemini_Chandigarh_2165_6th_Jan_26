using AutoMapper;
using SmartHealthcare.API.Repositories.Interfaces;
using SmartHealthcare.API.Services.Interfaces;
using SmartHealthcare.Models.DTOs;
using SmartHealthcare.Models.Entities;

namespace SmartHealthcare.API.Services;

public class PrescriptionService : IPrescriptionService
{
    private readonly IPrescriptionRepository _repo;
    private readonly IAppointmentRepository _appointmentRepo;
    private readonly IMapper _mapper;
    private readonly ILogger<PrescriptionService> _logger;

    public PrescriptionService(
        IPrescriptionRepository repo,
        IAppointmentRepository appointmentRepo,
        IMapper mapper,
        ILogger<PrescriptionService> logger)
    {
        _repo = repo;
        _appointmentRepo = appointmentRepo;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<PrescriptionDTO?> GetByIdAsync(int id)
    {
        var prescription = await _repo.GetByIdAsync(id);
        if (prescription == null)
        {
            _logger.LogWarning("Prescription with Id {Id} was not found", id);
            return null;
        }

        return _mapper.Map<PrescriptionDTO>(prescription);
    }

    public async Task<PrescriptionDTO?> GetByAppointmentIdAsync(int appointmentId)
    {
        var prescription = await _repo.GetByAppointmentIdAsync(appointmentId);
        if (prescription == null)
        {
            _logger.LogWarning("No prescription found for AppointmentId {AppointmentId}", appointmentId);
            return null;
        }

        return _mapper.Map<PrescriptionDTO>(prescription);
    }

    public async Task<IEnumerable<PrescriptionDTO>> GetAllAsync()
    {
        var prescriptions = await _repo.GetAllAsync();
        return _mapper.Map<IEnumerable<PrescriptionDTO>>(prescriptions);
    }

    public async Task<PrescriptionDTO> CreateAsync(CreatePrescriptionDTO dto)
    {
        // Make sure the appointment actually exists before creating a prescription for it
        var appointment = await _appointmentRepo.GetByIdAsync(dto.AppointmentId);
        if (appointment == null)
        {
            _logger.LogError("Cannot create prescription — Appointment {AppointmentId} does not exist", dto.AppointmentId);
            throw new KeyNotFoundException($"Appointment with ID {dto.AppointmentId} was not found.");
        }

        // Check if a prescription already exists for this appointment (1-to-1 enforcement)
        var existing = await _repo.GetByAppointmentIdAsync(dto.AppointmentId);
        if (existing != null)
        {
            _logger.LogWarning("Prescription already exists for AppointmentId {AppointmentId}", dto.AppointmentId);
            throw new InvalidOperationException($"A prescription already exists for appointment {dto.AppointmentId}. Use the update endpoint to modify it.");
        }

        var prescription = new Prescription
        {
            AppointmentId = dto.AppointmentId,
            Diagnosis = dto.Diagnosis,
            Notes = dto.Notes,
            CreatedAt = DateTime.UtcNow,
            Medicines = new List<Medicine>()
        };

        // Map and attach each medicine to this prescription
        foreach (var medDto in dto.Medicines)
        {
            prescription.Medicines.Add(new Medicine
            {
                Name = medDto.Name,
                Dosage = medDto.Dosage,
                Duration = medDto.Duration,
                Instructions = medDto.Instructions
            });
        }

        await _repo.AddAsync(prescription);
        await _repo.SaveAsync();

        _logger.LogInformation(
            "Prescription created successfully with Id {PrescriptionId} for AppointmentId {AppointmentId}",
            prescription.Id,
            dto.AppointmentId);

        // Reload with all includes so the returned DTO is fully populated
        var created = await _repo.GetByIdAsync(prescription.Id);
        return _mapper.Map<PrescriptionDTO>(created!);
    }

    public async Task<bool> UpdateAsync(int id, UpdatePrescriptionDTO dto)
    {
        var prescription = await _repo.GetByIdAsync(id);
        if (prescription == null)
        {
            _logger.LogWarning("Update failed — Prescription {Id} not found", id);
            return false;
        }

        // Update the scalar fields
        prescription.Diagnosis = dto.Diagnosis;
        prescription.Notes = dto.Notes;

            
        prescription.Medicines.Clear();

        foreach (var medDto in dto.Medicines)
        {
            prescription.Medicines.Add(new Medicine
            {
                Name = medDto.Name,
                Dosage = medDto.Dosage,
                Duration = medDto.Duration,
                Instructions = medDto.Instructions,
                PrescriptionId = prescription.Id
            });
        }

        _repo.Update(prescription);
        await _repo.SaveAsync();

        _logger.LogInformation(
            "Prescription {PrescriptionId} updated successfully. Medicines count: {Count}",
            id,
            prescription.Medicines.Count);

        return true;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var prescription = await _repo.GetByIdAsync(id);
        if (prescription == null)
        {
            _logger.LogWarning("Delete failed — Prescription {Id} not found", id);
            return false;
        }

        _repo.Delete(prescription);
        await _repo.SaveAsync();

        _logger.LogInformation("Prescription {PrescriptionId} deleted successfully", id);
        return true;
    }
}