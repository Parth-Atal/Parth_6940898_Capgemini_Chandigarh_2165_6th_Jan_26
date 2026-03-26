using Microsoft.AspNetCore.Mvc;
using BankingApi.Data;
using BankingApi.DTOs;
using BankingApi.Models;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace BankingApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _config;

        public AuthController(AppDbContext context, IConfiguration config)
        {
            _context = context;
            _config = config;

            // Seed Users
            if (!_context.Users.Any())
            {
                _context.Users.AddRange(
                    new User { Email = "admin@test.com", Password = "1234", Role = "Admin" },
                    new User { Email = "user@test.com", Password = "1234", Role = "User" }
                );

                _context.Accounts.AddRange(
                    new Account { AccountHolderName = "Admin User", AccountNumber = "123456789012", Balance = 50000, UserEmail = "admin@test.com" },
                    new Account { AccountHolderName = "Normal User", AccountNumber = "987654321234", Balance = 20000, UserEmail = "user@test.com" }
                );

                _context.SaveChanges();
            }
        }

        [HttpPost("login")]
        public IActionResult Login(LoginDTO dto)
        {
            var user = _context.Users
                .FirstOrDefault(x => x.Email == dto.Email && x.Password == dto.Password);

            if (user == null)
                return Unauthorized();

            var token = GenerateToken(user);

            return Ok(new { token });
        }

        private string GenerateToken(User user)
        {
            var claims = new[]
            {
                new Claim(ClaimTypes.Name, user.Email),
                new Claim(ClaimTypes.Role, user.Role)
            };

            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_config["Jwt:Key"])
            );

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddMinutes(60),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}