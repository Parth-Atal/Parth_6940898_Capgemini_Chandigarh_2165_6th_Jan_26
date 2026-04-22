using BookStore.Domain.Entities;

namespace BookStore.Application.Interfaces;

public interface ITokenService
{
    string GenerateAccessToken(User user);
    string GenerateRefreshToken();
}