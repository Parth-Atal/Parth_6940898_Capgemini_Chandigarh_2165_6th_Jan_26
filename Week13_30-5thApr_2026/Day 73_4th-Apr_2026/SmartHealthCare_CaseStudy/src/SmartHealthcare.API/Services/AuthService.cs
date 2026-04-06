using SmartHealthcare.API.Repositories.Interfaces;
using SmartHealthcare.API.Services.Interfaces;
using SmartHealthcare.Models.DTOs;
using SmartHealthcare.Models.Entities;

namespace SmartHealthcare.API.Services;

public class AuthService : IAuthService
{
    private readonly IUserRepository _userRepo;
    private readonly IPatientRepository _patientRepo;
    private readonly IDoctorRepository _doctorRepo;
    private readonly ITokenService _tokenService;
    private readonly IConfiguration _config;
    private readonly ILogger<AuthService> _logger;

    public AuthService(IUserRepository userRepo, IPatientRepository patientRepo,
        IDoctorRepository doctorRepo, ITokenService tokenService,
        IConfiguration config, ILogger<AuthService> logger)
    {
        _userRepo = userRepo;
        _patientRepo = patientRepo;
        _doctorRepo = doctorRepo;
        _tokenService = tokenService;
        _config = config;
        _logger = logger;
    }

    public async Task<TokenDTO?> RegisterAsync(RegisterDTO dto)
    {
        var existing = await _userRepo.GetByEmailAsync(dto.Email);
        if (existing != null)
        {
            _logger.LogWarning("Registration failed - email already exists: {Email}", dto.Email);
            return null;
        }

        var user = new User
        {
            FullName = dto.FullName,
            Email = dto.Email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
            Role = dto.Role,
            CreatedAt = DateTime.UtcNow
        };

        await _userRepo.AddAsync(user);
        await _userRepo.SaveAsync();

        _logger.LogInformation("User registered: {Email}, Role: {Role}", dto.Email, dto.Role);
        return await GenerateTokenResponse(user);
    }

    public async Task<TokenDTO?> LoginAsync(LoginDTO dto)
    {
        var user = await _userRepo.GetByEmailAsync(dto.Email);
        if (user == null || !BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash))
        {
            _logger.LogWarning("Login attempt failed for: {Email}", dto.Email);
            return null;
        }

        _logger.LogInformation("User logged in: {Email}", dto.Email);
        return await GenerateTokenResponse(user);
    }

    public async Task<TokenDTO?> RefreshTokenAsync(string refreshToken)
    {
        var users = await _userRepo.FindAsync(u =>
            u.RefreshTokens.Any(rt => rt.Token == refreshToken && !rt.IsRevoked && rt.ExpiresAt > DateTime.UtcNow));
        var user = users.FirstOrDefault();

        if (user == null)
        {
            return null;
        }

        var oldToken = user.RefreshTokens.First(rt => rt.Token == refreshToken);
        oldToken.IsRevoked = true;
        _userRepo.Update(user);
        await _userRepo.SaveAsync();

        return await GenerateTokenResponse(user);
    }

    private async Task<TokenDTO> GenerateTokenResponse(User user)
    {
        var accessToken = _tokenService.GenerateAccessToken(user);
        var refreshToken = _tokenService.GenerateRefreshToken();
        var refreshExpiry = DateTime.UtcNow.AddDays(double.Parse(_config["Jwt:RefreshTokenExpirationDays"]!));

        user.RefreshTokens.Add(new RefreshToken
        {
            UserId = user.Id,
            Token = refreshToken,
            ExpiresAt = refreshExpiry,
            CreatedAt = DateTime.UtcNow
        });

        _userRepo.Update(user);
        await _userRepo.SaveAsync();

        int? profileId = null;
        if (user.Role == "Patient")
        {
            var patient = await _patientRepo.GetByUserIdAsync(user.Id);
            profileId = patient?.Id;
        }
        else if (user.Role == "Doctor")
        {
            var doctor = await _doctorRepo.GetByUserIdAsync(user.Id);
            profileId = doctor?.Id;
        }

        return new TokenDTO
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken,
            Expiration = DateTime.UtcNow.AddMinutes(double.Parse(_config["Jwt:AccessTokenExpirationMinutes"]!)),
            Role = user.Role,
            FullName = user.FullName,
            UserId = user.Id,
            ProfileId = profileId
        };
    }
}
