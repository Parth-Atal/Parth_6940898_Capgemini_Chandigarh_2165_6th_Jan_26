using SmartHealthcare.Models.Entities;

namespace SmartHealthcare.API.Repositories.Interfaces;

public interface IUserRepository : IGenericRepository<User>
{
    Task<User?> GetByEmailAsync(string email);
    Task<IEnumerable<User>> GetAllWithPagingAsync(int page, int pageSize);
}
