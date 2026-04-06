using System.ComponentModel.DataAnnotations;

namespace SmartHealthcare.Models.Entities;

public class Appointment
{
    public int Id { get; set; }
    public int PatientId { get; set; }
    public int DoctorId { get; set; }

    [Required]
    public DateTime AppointmentDate { get; set; }

    [Required, MaxLength(20)]
    public string Status { get; set; } = "Pending";

    [MaxLength(500)]
    public string? Notes { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public Patient Patient { get; set; } = null!;
    public Doctor Doctor { get; set; } = null!;
    public Prescription? Prescription { get; set; }

    public Bill? Bill { get; set; }
}
