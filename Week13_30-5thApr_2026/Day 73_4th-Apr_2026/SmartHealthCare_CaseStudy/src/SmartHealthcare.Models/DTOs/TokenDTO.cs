using System.ComponentModel.DataAnnotations;

namespace SmartHealthcare.Models.DTOs;

public class TokenDTO
{
    public string AccessToken { get; set; } = string.Empty;
    public string RefreshToken { get; set; } = string.Empty;
    public DateTime Expiration { get; set; }
    public string Role { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public int UserId { get; set; }
    public int? ProfileId { get; set; }
}

public class RefreshTokenDTO
{
    [Required]
    public string RefreshToken { get; set; } = string.Empty;
}
