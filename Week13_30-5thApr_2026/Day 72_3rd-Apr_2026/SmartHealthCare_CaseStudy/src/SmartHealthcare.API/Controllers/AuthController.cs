using Microsoft.AspNetCore.Mvc;
using SmartHealthcare.API.Services.Interfaces;
using SmartHealthcare.Models.DTOs;

namespace SmartHealthcare.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
	private readonly IAuthService _authService;
	private readonly ILogger<AuthController> _logger;

	public AuthController(IAuthService authService, ILogger<AuthController> logger)
	{
		_authService = authService;
		_logger = logger;
	}

	[HttpPost("register")]
	public async Task<IActionResult> Register([FromBody] RegisterDTO dto)
	{
		if (!ModelState.IsValid) return BadRequest(ModelState);

		var result = await _authService.RegisterAsync(dto);
		if (result == null) return BadRequest(new ErrorResponseDTO { Message = "Email already exists", StatusCode = 400 });

		return CreatedAtAction(nameof(Register), result);
	}

	[HttpPost("login")]
	public async Task<IActionResult> Login([FromBody] LoginDTO dto)
	{
		if (!ModelState.IsValid) return BadRequest(ModelState);

		var result = await _authService.LoginAsync(dto);
		if (result == null) return Unauthorized(new ErrorResponseDTO { Message = "Invalid email or password", StatusCode = 401 });

		return Ok(result);
	}

	[HttpPost("refresh-token")]
	public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenDTO dto)
	{
		var result = await _authService.RefreshTokenAsync(dto.RefreshToken);
		if (result == null) return Unauthorized(new ErrorResponseDTO { Message = "Invalid or expired refresh token", StatusCode = 401 });

		return Ok(result);
	}
}
