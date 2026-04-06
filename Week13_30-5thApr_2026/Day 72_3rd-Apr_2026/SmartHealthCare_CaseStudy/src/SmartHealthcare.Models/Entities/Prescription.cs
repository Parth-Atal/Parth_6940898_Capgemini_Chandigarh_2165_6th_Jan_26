using System.ComponentModel.DataAnnotations;

namespace SmartHealthcare.Models.Entities;

public class Prescription
{
    public int Id { get; set; }
    public int AppointmentId { get; set; }

    [Required, MaxLength(500)]
    public string Diagnosis { get; set; } = string.Empty;

    [MaxLength(1000)]
    public string? Notes { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public Appointment Appointment { get; set; } = null!;
    public ICollection<Medicine> Medicines { get; set; } = new List<Medicine>();
}
