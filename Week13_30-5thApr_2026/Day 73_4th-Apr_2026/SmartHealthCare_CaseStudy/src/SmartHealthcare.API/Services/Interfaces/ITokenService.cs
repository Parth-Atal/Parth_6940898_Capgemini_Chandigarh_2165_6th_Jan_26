using SmartHealthcare.Models.Entities;

namespace SmartHealthcare.API.Services.Interfaces;

public interface ITokenService
{
    string GenerateAccessToken(User user);
    string GenerateRefreshToken();
}
