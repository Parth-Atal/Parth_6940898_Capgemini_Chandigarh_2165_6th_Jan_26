using Microsoft.AspNetCore.Mvc;
using ProductApi.Auth;

namespace ProductApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly JwtTokenService _tokenService;

    public AuthController(JwtTokenService tokenService)
    {
        _tokenService = tokenService;
    }

    /// <summary>
    /// Login to get a JWT token. Use username: admin, password: admin123
    /// </summary>
    [HttpPost("login")]
    public IActionResult Login([FromBody] LoginRequest request)
    {
        // Simple hardcoded check for demo purposes
        if (request.Username == "admin" && request.Password == "admin123")
        {
            var token = _tokenService.GenerateToken(request.Username);
            return Ok(new { Token = token, Message = "Login successful! Copy the token and use it in Swagger Authorize." });
        }

        return Unauthorized(new { Message = "Invalid credentials. Use admin / admin123" });
    }
}

public class LoginRequest
{
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}
