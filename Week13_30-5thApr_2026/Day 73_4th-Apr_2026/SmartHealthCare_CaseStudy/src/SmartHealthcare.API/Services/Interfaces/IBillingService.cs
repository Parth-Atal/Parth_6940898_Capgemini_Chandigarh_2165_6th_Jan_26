using SmartHealthcare.Models.DTOs;

namespace SmartHealthcare.API.Services.Interfaces;

public interface IBillingService
{
    Task<BillDTO?> GetByIdAsync(int id);
    Task<BillDTO?> GetByAppointmentIdAsync(int appointmentId);
    Task<PagedResult<BillDTO>> GetAllAsync(int pageNumber, int pageSize);
    Task<PagedResult<BillDTO>> GetByPatientIdAsync(int patientId, int pageNumber, int pageSize);
    Task<BillDTO> GenerateBillAsync(GenerateBillDTO dto);
    Task<bool> UpdatePaymentStatusAsync(int id, UpdateBillDTO dto);
    Task<bool> DeleteAsync(int id);
}