using SmartHealthcare.Models.Entities;

namespace SmartHealthcare.API.Repositories.Interfaces;

public interface IBillingRepository
{
    Task<Bill?> GetByIdAsync(int id);
    Task<Bill?> GetByAppointmentIdAsync(int appointmentId);
    Task<IEnumerable<Bill>> GetAllAsync(int pageNumber, int pageSize);
    Task<IEnumerable<Bill>> GetByPatientIdAsync(int patientId, int pageNumber, int pageSize);
    Task<int> CountAsync();
    Task<int> CountByPatientAsync(int patientId);
    Task AddAsync(Bill bill);
    void Update(Bill bill);
    void Delete(Bill bill);
    Task SaveAsync();
}