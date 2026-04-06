using SmartHealthcare.Models.Entities;

namespace SmartHealthcare.API.Repositories.Interfaces;

public interface IPrescriptionRepository
{
    Task<Prescription?> GetByIdAsync(int id);
    Task<Prescription?> GetByAppointmentIdAsync(int appointmentId);
    Task<Prescription?> GetWithMedicinesAsync(int id);
    Task<IEnumerable<Prescription>> GetAllAsync();
    Task AddAsync(Prescription prescription);
    void Update(Prescription prescription);
    void Delete(Prescription prescription);
    Task SaveAsync();
}