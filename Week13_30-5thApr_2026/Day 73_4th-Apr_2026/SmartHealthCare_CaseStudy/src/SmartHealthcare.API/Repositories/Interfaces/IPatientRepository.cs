using SmartHealthcare.Models.Entities;

namespace SmartHealthcare.API.Repositories.Interfaces;

public interface IPatientRepository : IGenericRepository<Patient>
{
    Task<Patient?> GetByUserIdAsync(int userId);
    Task<Patient?> GetWithDetailsAsync(int id);
    Task<IEnumerable<Patient>> GetAllWithDetailsAsync(int page, int pageSize);
}
