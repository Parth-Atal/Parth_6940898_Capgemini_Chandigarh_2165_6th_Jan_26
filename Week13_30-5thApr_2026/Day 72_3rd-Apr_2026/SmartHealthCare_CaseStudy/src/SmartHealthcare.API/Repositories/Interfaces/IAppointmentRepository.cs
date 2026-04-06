using SmartHealthcare.Models.Entities;

namespace SmartHealthcare.API.Repositories.Interfaces;

public interface IAppointmentRepository : IGenericRepository<Appointment>
{
    Task<Appointment?> GetWithDetailsAsync(int id);
    Task<IEnumerable<Appointment>> GetByPatientIdAsync(int patientId, int page, int pageSize);
    Task<IEnumerable<Appointment>> GetByDoctorIdAsync(int doctorId, int page, int pageSize);
    Task<IEnumerable<Appointment>> GetByDateAsync(DateTime date, int page, int pageSize);
    Task<IEnumerable<Appointment>> GetAllWithDetailsAsync(int page, int pageSize);
    Task<int> CountByPatientAsync(int patientId);
    Task<int> CountByDoctorAsync(int doctorId);
    Task<int> CountByDateAsync(DateTime date);
}
