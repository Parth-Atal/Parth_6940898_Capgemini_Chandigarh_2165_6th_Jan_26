using System.Security.Cryptography;
using System.Text;
using Asp.Versioning;
using BookStore.Application.DTOs;
using BookStore.Application.Interfaces;
using BookStore.Domain.Entities;
using BookStore.Infrastructure.Data;
using BookStore.Shared;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BookStore.API.Controllers;

[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly BookStoreDbContext _context;
    private readonly ITokenService _tokenService;

    public AuthController(BookStoreDbContext context, ITokenService tokenService) { _context = context; _tokenService = tokenService; }

    [HttpPost("register")]
    public async Task<ActionResult<ApiResponse>> Register(UserRegisterDto dto)
    {
        if (await _context.Users.AnyAsync(u => u.Email == dto.Email))
            return BadRequest(ApiResponse.Fail("Email already registered."));
        var user = new User { FullName = dto.FullName, Email = dto.Email, PasswordHash = HashPassword(dto.Password), Phone = dto.Phone, RoleId = 2 };
        _context.Users.Add(user);
        await _context.SaveChangesAsync();
        return Ok(ApiResponse.Ok("Registration successful."));
    }

    [HttpPost("login")]
    public async Task<ActionResult<ApiResponse<AuthResponseDto>>> Login(UserLoginDto dto)
    {
        var user = await _context.Users.Include(u => u.Role).FirstOrDefaultAsync(u => u.Email == dto.Email);
        if (user == null || user.PasswordHash != HashPassword(dto.Password))
            return Unauthorized(ApiResponse<AuthResponseDto>.Fail("Invalid credentials."));
        var accessToken = _tokenService.GenerateAccessToken(user);
        var refreshToken = _tokenService.GenerateRefreshToken();
        user.RefreshToken = refreshToken;
        user.RefreshTokenExpiry = DateTime.UtcNow.AddDays(7);
        await _context.SaveChangesAsync();
        return Ok(ApiResponse<AuthResponseDto>.Ok(new AuthResponseDto { Token = accessToken, RefreshToken = refreshToken, FullName = user.FullName, Role = user.Role.RoleName }));
    }

    [HttpPost("refresh")]
    public async Task<ActionResult<ApiResponse<AuthResponseDto>>> Refresh(RefreshTokenDto dto)
    {
        var user = await _context.Users.Include(u => u.Role).FirstOrDefaultAsync(u => u.RefreshToken == dto.RefreshToken);
        if (user == null || user.RefreshTokenExpiry < DateTime.UtcNow)
            return Unauthorized(ApiResponse<AuthResponseDto>.Fail("Invalid or expired refresh token."));
        var newAccess = _tokenService.GenerateAccessToken(user);
        var newRefresh = _tokenService.GenerateRefreshToken();
        user.RefreshToken = newRefresh;
        user.RefreshTokenExpiry = DateTime.UtcNow.AddDays(7);
        await _context.SaveChangesAsync();
        return Ok(ApiResponse<AuthResponseDto>.Ok(new AuthResponseDto { Token = newAccess, RefreshToken = newRefresh, FullName = user.FullName, Role = user.Role.RoleName }));
    }

    private static string HashPassword(string password) => Convert.ToBase64String(SHA256.HashData(Encoding.UTF8.GetBytes(password)));
}