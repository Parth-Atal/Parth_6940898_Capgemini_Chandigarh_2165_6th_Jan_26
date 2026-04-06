using SmartHealthcare.Models.DTOs;

namespace SmartHealthcare.API.Services.Interfaces;

public interface IPrescriptionService
{
    Task<PrescriptionDTO?> GetByIdAsync(int id);
    Task<PrescriptionDTO?> GetByAppointmentIdAsync(int appointmentId);
    Task<IEnumerable<PrescriptionDTO>> GetAllAsync();
    Task<PrescriptionDTO> CreateAsync(CreatePrescriptionDTO dto);
    Task<bool> UpdateAsync(int id, UpdatePrescriptionDTO dto);
    Task<bool> DeleteAsync(int id);
}