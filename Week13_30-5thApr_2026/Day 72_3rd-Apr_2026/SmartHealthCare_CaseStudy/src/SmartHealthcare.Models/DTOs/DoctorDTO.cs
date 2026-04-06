using System.ComponentModel.DataAnnotations;

namespace SmartHealthcare.Models.DTOs;

public class DoctorDTO
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string LicenseNumber { get; set; } = string.Empty;
    public int YearsOfExperience { get; set; }
    public decimal ConsultationFee { get; set; }
    public string Phone { get; set; } = string.Empty;
    public bool IsAvailable { get; set; }
    public List<string> Specializations { get; set; } = new();
}

public class CreateDoctorDTO
{
    [Required]
    public string LicenseNumber { get; set; } = string.Empty;

    [Required]
    [Range(0, 60)]
    public int YearsOfExperience { get; set; }

    [Required]
    [Range(0, 100000)]
    public decimal ConsultationFee { get; set; }

    [Required]
    [Phone]
    public string Phone { get; set; } = string.Empty;

    public bool IsAvailable { get; set; } = true;

    public List<int> SpecializationIds { get; set; } = new();
}

public class UpdateDoctorDTO
{
    [Required]
    public string LicenseNumber { get; set; } = string.Empty;

    [Required]
    [Range(0, 60)]
    public int YearsOfExperience { get; set; }

    [Required]
    [Range(0, 100000)]
    public decimal ConsultationFee { get; set; }

    [Required]
    [Phone]
    public string Phone { get; set; } = string.Empty;

    public bool IsAvailable { get; set; } = true;

    public List<int> SpecializationIds { get; set; } = new();
}
