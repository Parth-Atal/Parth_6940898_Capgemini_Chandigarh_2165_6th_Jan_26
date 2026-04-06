using SmartHealthcare.Models.DTOs;

namespace SmartHealthcare.API.Services.Interfaces;

public interface IAuthService
{
    Task<TokenDTO?> RegisterAsync(RegisterDTO dto);
    Task<TokenDTO?> LoginAsync(LoginDTO dto);
    Task<TokenDTO?> RefreshTokenAsync(string refreshToken);
}
