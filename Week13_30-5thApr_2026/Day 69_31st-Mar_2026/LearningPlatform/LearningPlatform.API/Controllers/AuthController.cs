using AutoMapper;
using LearningPlatform.API.Data;
using LearningPlatform.API.DTOs;
using LearningPlatform.API.Models;
using LearningPlatform.API.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


using UserProfile = LearningPlatform.API.Models.Profile;

namespace LearningPlatform.API.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IJwtService _jwtService;
        private readonly IMapper _mapper;

        public AuthController(AppDbContext context, IJwtService jwtService, IMapper mapper)
        {
            _context = context;
            _jwtService = jwtService;
            _mapper = mapper;
        }

        
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto dto)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList();
                return BadRequest(new ErrorResponseDto
                {
                    Error = "Validation failed",
                    Details = errors
                });
            }

            // Validate role
            var validRoles = new[] { "Student", "Instructor", "Admin" };
            if (!validRoles.Contains(dto.Role))
            {
                return BadRequest(new ErrorResponseDto
                {
                    Error = "Invalid role. Must be Student, Instructor, or Admin"
                });
            }

            // Check if email already exists
            if (await _context.Users.AnyAsync(u => u.Email == dto.Email))
            {
                return Conflict(new ErrorResponseDto
                {
                    Error = "Email is already registered"
                });
            }

            // Check if username already exists
            if (await _context.Users.AnyAsync(u => u.Username == dto.Username))
            {
                return Conflict(new ErrorResponseDto
                {
                    Error = "Username is already taken"
                });
            }

            // Create new user
            var user = new User
            {
                Username = dto.Username,
                Email = dto.Email,
                Role = dto.Role,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
                CreatedAt = DateTime.UtcNow
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            // Auto-create empty profile
            var profile = new UserProfile
            {
                UserId = user.Id,
                UpdatedAt = DateTime.UtcNow
            };
            _context.Profiles.Add(profile);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(Register), new
            {
                message = "User registered successfully",
                userId = user.Id,
                username = user.Username,
                role = user.Role
            });
        }

        /// <summary>
        /// Login and receive JWT token
        /// POST /api/auth/login
        /// </summary>
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList();
                return BadRequest(new ErrorResponseDto
                {
                    Error = "Validation failed",
                    Details = errors
                });
            }

            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Email == dto.Email);

            if (user == null || !BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash))
            {
                Console.WriteLine(dto.Password);
                return Unauthorized(new ErrorResponseDto
                {
                    Error = "Invalid email or password"
                });
            }

            var token = _jwtService.GenerateToken(user);
            var expiry = _jwtService.GetTokenExpiry();

            return Ok(new AuthResponseDto
            {
                Token = token,
                Username = user.Username,
                Email = user.Email,
                Role = user.Role,
                ExpiresAt = expiry
            });
        }
    }
}
