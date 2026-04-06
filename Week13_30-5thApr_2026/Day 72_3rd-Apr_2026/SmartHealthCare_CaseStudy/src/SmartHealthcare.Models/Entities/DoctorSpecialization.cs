namespace SmartHealthcare.Models.Entities;

public class DoctorSpecialization
{
    public int DoctorId { get; set; }
    public Doctor Doctor { get; set; } = null!;

    public int SpecializationId { get; set; }
    public Specialization Specialization { get; set; } = null!;
}
