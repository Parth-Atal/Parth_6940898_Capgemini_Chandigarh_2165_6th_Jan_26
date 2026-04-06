using Microsoft.EntityFrameworkCore;
using SmartHealthcare.API.Data;
using SmartHealthcare.API.Repositories.Interfaces;
using SmartHealthcare.Models.Entities;

namespace SmartHealthcare.API.Repositories;

public class UserRepository : GenericRepository<User>, IUserRepository
{
    public UserRepository(ApplicationDbContext context) : base(context) { }

    public async Task<User?> GetByEmailAsync(string email)
        => await _dbSet.Include(u => u.RefreshTokens).FirstOrDefaultAsync(u => u.Email == email);

    public async Task<IEnumerable<User>> GetAllWithPagingAsync(int page, int pageSize)
        => await _dbSet.OrderByDescending(u => u.CreatedAt)
                       .Skip((page - 1) * pageSize)
                       .Take(pageSize)
                       .ToListAsync();
}
