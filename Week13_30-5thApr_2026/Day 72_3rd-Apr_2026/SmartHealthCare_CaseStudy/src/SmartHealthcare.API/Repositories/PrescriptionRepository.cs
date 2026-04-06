using Microsoft.EntityFrameworkCore;
using SmartHealthcare.API.Data;
using SmartHealthcare.API.Repositories.Interfaces;
using SmartHealthcare.Models.Entities;

namespace SmartHealthcare.API.Repositories;

public class PrescriptionRepository : IPrescriptionRepository
{
    private readonly ApplicationDbContext _context;

    public PrescriptionRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Prescription?> GetByIdAsync(int id)
    {
        return await _context.Prescriptions
            .Include(p => p.Medicines)
            .Include(p => p.Appointment)
                .ThenInclude(a => a.Patient)
                    .ThenInclude(pt => pt.User)
            .Include(p => p.Appointment)
                .ThenInclude(a => a.Doctor)
                    .ThenInclude(d => d.User)
            .FirstOrDefaultAsync(p => p.Id == id);
    }

    public async Task<Prescription?> GetByAppointmentIdAsync(int appointmentId)
    {
        return await _context.Prescriptions
            .Include(p => p.Medicines)
            .Include(p => p.Appointment)
                .ThenInclude(a => a.Patient)
                    .ThenInclude(pt => pt.User)
            .Include(p => p.Appointment)
                .ThenInclude(a => a.Doctor)
                    .ThenInclude(d => d.User)
            .FirstOrDefaultAsync(p => p.AppointmentId == appointmentId);
    }

    public async Task<Prescription?> GetWithMedicinesAsync(int id)
    {
        return await _context.Prescriptions
            .Include(p => p.Medicines)
            .FirstOrDefaultAsync(p => p.Id == id);
    }

    public async Task<IEnumerable<Prescription>> GetAllAsync()
    {
        return await _context.Prescriptions
            .Include(p => p.Medicines)
            .Include(p => p.Appointment)
                .ThenInclude(a => a.Patient)
                    .ThenInclude(pt => pt.User)
            .Include(p => p.Appointment)
                .ThenInclude(a => a.Doctor)
                    .ThenInclude(d => d.User)
            .OrderByDescending(p => p.CreatedAt)
            .ToListAsync();
    }

    public async Task AddAsync(Prescription prescription)
    {
        await _context.Prescriptions.AddAsync(prescription);
    }

    public void Update(Prescription prescription)
    {
        _context.Prescriptions.Update(prescription);
    }

    public void Delete(Prescription prescription)
    {
        _context.Prescriptions.Remove(prescription);
    }

    public async Task SaveAsync()
    {
        await _context.SaveChangesAsync();
    }
}