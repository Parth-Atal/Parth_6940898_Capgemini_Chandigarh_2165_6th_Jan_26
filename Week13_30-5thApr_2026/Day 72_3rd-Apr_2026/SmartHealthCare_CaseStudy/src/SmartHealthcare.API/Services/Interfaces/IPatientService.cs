using SmartHealthcare.Models.DTOs;

namespace SmartHealthcare.API.Services.Interfaces;

public interface IPatientService
{
    Task<PagedResult<PatientDTO>> GetAllAsync(int page, int pageSize);
    Task<PatientDTO?> GetByIdAsync(int id);
    Task<PatientDTO?> GetByUserIdAsync(int userId);
    Task<PatientDTO> CreateAsync(int userId, CreatePatientDTO dto);
    Task<bool> UpdateAsync(int id, UpdatePatientDTO dto);
    Task<bool> PatchAsync(int id, Dictionary<string, object> patchData);
    Task<bool> DeleteAsync(int id);
}
