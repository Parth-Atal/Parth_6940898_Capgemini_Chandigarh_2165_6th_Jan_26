using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using AutoMapper;
using BankingApi.DTOs;
using BankingApi.Data;

namespace BankingApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AccountController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;

        public AccountController(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        // 🔐 SCENARIO 1 + 3
        [Authorize]
        [HttpGet("details")]
        public IActionResult GetDetails()
        {
            var email = User.Identity.Name;
            var role = User.Claims.FirstOrDefault(c => c.Type.Contains("role"))?.Value;

            var account = _context.Accounts.FirstOrDefault(x => x.UserEmail == email);

            if (account == null)
                return NotFound();

            if (role == "Admin")
            {
                var adminData = _mapper.Map<AdminAccountDTO>(account);
                return Ok(adminData);
            }
            else
            {
                var userData = _mapper.Map<UserAccountDTO>(account);
                return Ok(userData);
            }
        }

        // 🔐 SCENARIO 2
        [Authorize]
        [HttpGet("masked")]
        public IActionResult GetMasked()
        {
            var email = User.Identity.Name;

            var account = _context.Accounts.FirstOrDefault(x => x.UserEmail == email);

            var result = _mapper.Map<UserAccountDTO>(account);

            return Ok(result);
        }
    }
}