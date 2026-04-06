using Microsoft.EntityFrameworkCore;
using SmartHealthcare.API.Data;
using SmartHealthcare.API.Repositories.Interfaces;
using SmartHealthcare.Models.Entities;

namespace SmartHealthcare.API.Repositories;

public class AppointmentRepository : GenericRepository<Appointment>, IAppointmentRepository
{
    public AppointmentRepository(ApplicationDbContext context) : base(context) { }

    public async Task<Appointment?> GetWithDetailsAsync(int id)
        => await _dbSet.Include(a => a.Patient).ThenInclude(p => p.User)
                       .Include(a => a.Doctor).ThenInclude(d => d.User)
                       .Include(a => a.Prescription).ThenInclude(p => p!.Medicines)
                       .FirstOrDefaultAsync(a => a.Id == id);

    public async Task<IEnumerable<Appointment>> GetByPatientIdAsync(int patientId, int page, int pageSize)
        => await _dbSet.Include(a => a.Doctor).ThenInclude(d => d.User)
                       .Include(a => a.Patient).ThenInclude(p => p.User)
                       .Where(a => a.PatientId == patientId)
                       .OrderByDescending(a => a.AppointmentDate)
                       .Skip((page - 1) * pageSize).Take(pageSize)
                       .ToListAsync();

    public async Task<IEnumerable<Appointment>> GetByDoctorIdAsync(int doctorId, int page, int pageSize)
        => await _dbSet.Include(a => a.Patient).ThenInclude(p => p.User)
                       .Include(a => a.Doctor).ThenInclude(d => d.User)
                       .Where(a => a.DoctorId == doctorId)
                       .OrderByDescending(a => a.AppointmentDate)
                       .Skip((page - 1) * pageSize).Take(pageSize)
                       .ToListAsync();

    public async Task<IEnumerable<Appointment>> GetByDateAsync(DateTime date, int page, int pageSize)
        => await _dbSet.Include(a => a.Patient).ThenInclude(p => p.User)
                       .Include(a => a.Doctor).ThenInclude(d => d.User)
                       .Where(a => a.AppointmentDate.Date == date.Date)
                       .OrderBy(a => a.AppointmentDate)
                       .Skip((page - 1) * pageSize).Take(pageSize)
                       .ToListAsync();

    public async Task<IEnumerable<Appointment>> GetAllWithDetailsAsync(int page, int pageSize)
        => await _dbSet.Include(a => a.Patient).ThenInclude(p => p.User)
                       .Include(a => a.Doctor).ThenInclude(d => d.User)
                       .OrderByDescending(a => a.AppointmentDate)
                       .Skip((page - 1) * pageSize).Take(pageSize)
                       .ToListAsync();

    public async Task<int> CountByPatientAsync(int patientId)
        => await _dbSet.CountAsync(a => a.PatientId == patientId);

    public async Task<int> CountByDoctorAsync(int doctorId)
        => await _dbSet.CountAsync(a => a.DoctorId == doctorId);

    public async Task<int> CountByDateAsync(DateTime date)
        => await _dbSet.CountAsync(a => a.AppointmentDate.Date == date.Date);
}
