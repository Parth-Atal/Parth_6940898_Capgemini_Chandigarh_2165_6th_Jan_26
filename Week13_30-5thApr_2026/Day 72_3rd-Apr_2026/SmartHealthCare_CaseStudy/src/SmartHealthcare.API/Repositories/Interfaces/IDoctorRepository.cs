using SmartHealthcare.Models.Entities;

namespace SmartHealthcare.API.Repositories.Interfaces;

public interface IDoctorRepository : IGenericRepository<Doctor>
{
    Task<Doctor?> GetByUserIdAsync(int userId);
    Task<Doctor?> GetWithDetailsAsync(int id);
    Task<IEnumerable<Doctor>> GetAllWithDetailsAsync(int page, int pageSize);
    Task<IEnumerable<Doctor>> SearchBySpecializationAsync(string specialization, int page, int pageSize);
    Task<int> CountBySpecializationAsync(string specialization);
}
