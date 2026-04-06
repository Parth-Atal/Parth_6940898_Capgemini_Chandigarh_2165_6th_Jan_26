using Microsoft.EntityFrameworkCore;
using SmartHealthcare.API.Data;
using SmartHealthcare.API.Repositories.Interfaces;
using SmartHealthcare.Models.Entities;

namespace SmartHealthcare.API.Repositories;

public class BillingRepository : IBillingRepository
{
    private readonly ApplicationDbContext _context;

    public BillingRepository(ApplicationDbContext context)
    {
        _context = context;
    }
    
    private IQueryable<Bill> BillsWithDetails =>
        _context.Bills
            .Include(b => b.Appointment)
                .ThenInclude(a => a.Patient)
                    .ThenInclude(p => p.User)
            .Include(b => b.Appointment)
                .ThenInclude(a => a.Doctor)
                    .ThenInclude(d => d.User)
            .Include(b => b.Appointment)
                .ThenInclude(a => a.Prescription)
                    .ThenInclude(p => p.Medicines);

    public async Task<Bill?> GetByIdAsync(int id)
    {
        return await BillsWithDetails
            .FirstOrDefaultAsync(b => b.Id == id);
    }

    public async Task<Bill?> GetByAppointmentIdAsync(int appointmentId)
    {
        return await BillsWithDetails
            .FirstOrDefaultAsync(b => b.AppointmentId == appointmentId);
    }

    public async Task<IEnumerable<Bill>> GetAllAsync(int pageNumber, int pageSize)
    {
        return await BillsWithDetails
            .OrderByDescending(b => b.GeneratedAt)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }

    public async Task<IEnumerable<Bill>> GetByPatientIdAsync(int patientId, int pageNumber, int pageSize)
    {
        return await BillsWithDetails
            .Where(b => b.Appointment.Patient.Id == patientId)
            .OrderByDescending(b => b.GeneratedAt)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }

    public async Task<int> CountAsync()
    {
        return await _context.Bills.CountAsync();
    }

    public async Task<int> CountByPatientAsync(int patientId)
    {
        return await _context.Bills
            .CountAsync(b => b.Appointment.Patient.Id == patientId);
    }

    public async Task AddAsync(Bill bill)
    {
        await _context.Bills.AddAsync(bill);
    }

    public void Update(Bill bill)
    {
        _context.Bills.Update(bill);
    }

    public void Delete(Bill bill)
    {
        _context.Bills.Remove(bill);
    }

    public async Task SaveAsync()
    {
        await _context.SaveChangesAsync();
    }
}