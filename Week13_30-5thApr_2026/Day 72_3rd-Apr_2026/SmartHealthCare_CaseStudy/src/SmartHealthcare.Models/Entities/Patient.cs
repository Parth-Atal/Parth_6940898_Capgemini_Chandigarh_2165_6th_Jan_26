using System.ComponentModel.DataAnnotations;

namespace SmartHealthcare.Models.Entities;

public class Patient
{
    public int Id { get; set; }
    public int UserId { get; set; }

    [Required]
    public DateTime DateOfBirth { get; set; }

    [Required, MaxLength(10)]
    public string Gender { get; set; } = string.Empty;

    [MaxLength(5)]
    public string? BloodGroup { get; set; }

    [Required, MaxLength(300)]
    public string Address { get; set; } = string.Empty;

    [Required, Phone, MaxLength(15)]
    public string Phone { get; set; } = string.Empty;

    public User User { get; set; } = null!;
    public ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
}
