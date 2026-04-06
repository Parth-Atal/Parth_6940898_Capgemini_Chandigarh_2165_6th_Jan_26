using System.ComponentModel.DataAnnotations;

namespace SmartHealthcare.Models.Entities;

public class Doctor
{
    public int Id { get; set; }
    public int UserId { get; set; }

    [Required, MaxLength(50)]
    public string LicenseNumber { get; set; } = string.Empty;

    [Range(0, 60)]
    public int YearsOfExperience { get; set; }

    [Range(0, 100000)]
    public decimal ConsultationFee { get; set; }

    [Required, Phone, MaxLength(15)]
    public string Phone { get; set; } = string.Empty;

    public bool IsAvailable { get; set; } = true;

    public User User { get; set; } = null!;
    public ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
    public ICollection<DoctorSpecialization> DoctorSpecializations { get; set; } = new List<DoctorSpecialization>();
}
