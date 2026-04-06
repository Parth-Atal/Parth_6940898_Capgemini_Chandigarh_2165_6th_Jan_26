using System.ComponentModel.DataAnnotations;
using SmartHealthcare.Models.Validators;

namespace SmartHealthcare.Models.DTOs;

public class AppointmentDTO
{
    public int Id { get; set; }
    public int PatientId { get; set; }
    public string PatientName { get; set; } = string.Empty;
    public int DoctorId { get; set; }
    public string DoctorName { get; set; } = string.Empty;
    public DateTime AppointmentDate { get; set; }
    public string Status { get; set; } = string.Empty;
    public string? Notes { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class CreateAppointmentDTO
{
    [Required(ErrorMessage = "Doctor is required")]
    public int DoctorId { get; set; }

    [Required(ErrorMessage = "Appointment date is required")]
    [FutureDate(ErrorMessage = "Appointment date must be in the future")]
    [DataType(DataType.DateTime)]
    public DateTime AppointmentDate { get; set; }

    [MaxLength(500)]
    public string? Notes { get; set; }
}

public class UpdateAppointmentDTO
{
    [Required]
    public int DoctorId { get; set; }

    [Required]
    [FutureDate]
    [DataType(DataType.DateTime)]
    public DateTime AppointmentDate { get; set; }

    [Required]
    public string Status { get; set; } = string.Empty;

    [MaxLength(500)]
    public string? Notes { get; set; }
}
