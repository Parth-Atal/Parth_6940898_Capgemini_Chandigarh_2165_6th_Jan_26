using System.ComponentModel.DataAnnotations;

namespace SmartHealthcare.Models.DTOs;

public class UserDTO
{
    public int Id { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}

public class UpdateUserRoleDTO
{
    [Required]
    public string Role { get; set; } = string.Empty;
}
