using SmartHealthcare.Models.DTOs;

namespace SmartHealthcare.API.Services.Interfaces;

public interface IDoctorService
{
    Task<PagedResult<DoctorDTO>> GetAllAsync(int page, int pageSize);
    Task<DoctorDTO?> GetByIdAsync(int id);
    Task<DoctorDTO?> GetByUserIdAsync(int userId);
    Task<DoctorDTO> CreateAsync(int userId, CreateDoctorDTO dto);
    Task<bool> UpdateAsync(int id, UpdateDoctorDTO dto);
    Task<bool> PatchAsync(int id, Dictionary<string, object> patchData);
    Task<bool> DeleteAsync(int id);
    Task<PagedResult<DoctorDTO>> SearchBySpecializationAsync(string specialization, int page, int pageSize);
}
