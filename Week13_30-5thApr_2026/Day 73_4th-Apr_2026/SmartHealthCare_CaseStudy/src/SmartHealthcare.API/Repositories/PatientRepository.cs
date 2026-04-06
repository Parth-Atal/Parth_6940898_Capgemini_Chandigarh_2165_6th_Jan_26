using Microsoft.EntityFrameworkCore;
using SmartHealthcare.API.Data;
using SmartHealthcare.API.Repositories.Interfaces;
using SmartHealthcare.Models.Entities;

namespace SmartHealthcare.API.Repositories;

public class PatientRepository : GenericRepository<Patient>, IPatientRepository
{
    public PatientRepository(ApplicationDbContext context) : base(context) { }

    public async Task<Patient?> GetByUserIdAsync(int userId)
        => await _dbSet.Include(p => p.User).FirstOrDefaultAsync(p => p.UserId == userId);

    public async Task<Patient?> GetWithDetailsAsync(int id)
        => await _dbSet.Include(p => p.User).FirstOrDefaultAsync(p => p.Id == id);

    public async Task<IEnumerable<Patient>> GetAllWithDetailsAsync(int page, int pageSize)
        => await _dbSet.Include(p => p.User)
                       .OrderBy(p => p.Id)
                       .Skip((page - 1) * pageSize)
                       .Take(pageSize)
                       .ToListAsync();
}
