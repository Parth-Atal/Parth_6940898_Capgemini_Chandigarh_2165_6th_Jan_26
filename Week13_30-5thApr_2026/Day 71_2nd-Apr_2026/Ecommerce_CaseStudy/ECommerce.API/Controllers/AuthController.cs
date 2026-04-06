using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly TokenService _tokenService;
    private readonly AppDbContext _context;

    public AuthController(TokenService tokenService, AppDbContext context)
    {
        _tokenService = tokenService;
        _context = context;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginDto dto)
    {
        if (dto.Username == "admin" && dto.Password == "123")
        {
            var accessToken = _tokenService.GenerateToken(dto.Username, "Admin");
            var refreshToken = _tokenService.GenerateRefreshToken();

            _context.RefreshTokens.Add(new RefreshToken
            {
                Token = refreshToken,
                ExpiryDate = DateTime.Now.AddDays(7),
                UserId = 1
            });

            await _context.SaveChangesAsync();

            return Ok(new { accessToken, refreshToken });
        }

        if (dto.Username == "user" && dto.Password == "123")
        {
            var accessToken = _tokenService.GenerateToken(dto.Username, "User");
            var refreshToken = _tokenService.GenerateRefreshToken();

            _context.RefreshTokens.Add(new RefreshToken
            {
                Token = refreshToken,
                ExpiryDate = DateTime.Now.AddDays(7),
                UserId = 2
            });

            await _context.SaveChangesAsync();

            return Ok(new { accessToken, refreshToken });
        }

        return Unauthorized();
    }

    [HttpPost("refresh")]
    public async Task<IActionResult> Refresh(TokenRequest request)
    {
        var storedToken = await _context.RefreshTokens
            .FirstOrDefaultAsync(t => t.Token == request.RefreshToken);

        if (storedToken == null || storedToken.ExpiryDate < DateTime.Now)
            return Unauthorized("Invalid or expired refresh token");

        var newAccessToken = _tokenService.GenerateToken("user", "User");
        var newRefreshToken = _tokenService.GenerateRefreshToken();

        storedToken.Token = newRefreshToken;
        storedToken.ExpiryDate = DateTime.Now.AddDays(7);

        await _context.SaveChangesAsync();

        return Ok(new { accessToken = newAccessToken, refreshToken = newRefreshToken });
    }
}