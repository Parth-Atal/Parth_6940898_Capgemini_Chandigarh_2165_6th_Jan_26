using SmartHealthcare.Models.DTOs;

namespace SmartHealthcare.API.Services.Interfaces;

public interface IAppointmentService
{
    Task<PagedResult<AppointmentDTO>> GetAllAsync(int page, int pageSize);
    Task<AppointmentDTO?> GetByIdAsync(int id);
    Task<PagedResult<AppointmentDTO>> GetByPatientIdAsync(int patientId, int page, int pageSize);
    Task<PagedResult<AppointmentDTO>> GetByDoctorIdAsync(int doctorId, int page, int pageSize);
    Task<PagedResult<AppointmentDTO>> GetByDateAsync(DateTime date, int page, int pageSize);
    Task<AppointmentDTO> CreateAsync(int patientId, CreateAppointmentDTO dto);
    Task<bool> UpdateAsync(int id, UpdateAppointmentDTO dto);
    Task<bool> PatchAsync(int id, Dictionary<string, object> patchData);
    Task<bool> DeleteAsync(int id);
}
