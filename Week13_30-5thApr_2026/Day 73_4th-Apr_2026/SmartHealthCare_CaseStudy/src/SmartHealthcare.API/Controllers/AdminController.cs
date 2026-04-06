using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using SmartHealthcare.API.Data;
using SmartHealthcare.API.Repositories.Interfaces;
using SmartHealthcare.Models.DTOs;

namespace SmartHealthcare.API.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize(Roles = "Admin")]
public class AdminController : ControllerBase
{
	private readonly IUserRepository _userRepo;
	private readonly IMapper _mapper;
	private readonly ApplicationDbContext _context;
	private readonly IMemoryCache _cache;

	public AdminController(IUserRepository userRepo, IMapper mapper, ApplicationDbContext context, IMemoryCache cache)
	{
		_userRepo = userRepo;
		_mapper = mapper;
		_context = context;
		_cache = cache;
	}

	[HttpGet("users")]
	public async Task<IActionResult> GetAllUsers([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
	{
		var users = await _userRepo.GetAllWithPagingAsync(pageNumber, pageSize);
		var total = await _userRepo.CountAsync();
		return Ok(new PagedResult<UserDTO>
		{
			Items = _mapper.Map<List<UserDTO>>(users),
			TotalCount = total,
			PageNumber = pageNumber,
			PageSize = pageSize
		});
	}

	[HttpPut("users/{id:int}/role")]
	public async Task<IActionResult> UpdateRole(int id, [FromBody] UpdateUserRoleDTO dto)
	{
		var user = await _userRepo.GetByIdAsync(id);
		if (user == null) return NotFound(new ErrorResponseDTO { Message = "User not found", StatusCode = 404 });
		user.Role = dto.Role;
		_userRepo.Update(user);
		await _userRepo.SaveAsync();
		return Ok(new { message = "Role updated" });
	}

	[HttpDelete("users/{id:int}")]
	public async Task<IActionResult> DeleteUser(int id)
	{
		var user = await _userRepo.GetByIdAsync(id);
		if (user == null) return NotFound(new ErrorResponseDTO { Message = "User not found", StatusCode = 404 });
		_userRepo.Delete(user);
		await _userRepo.SaveAsync();
		return Ok(new { message = "User deleted" });
	}

	[HttpGet("reports")]
	public async Task<IActionResult> GetReports()
	{
		var cacheKey = "admin_reports";
		if (!_cache.TryGetValue(cacheKey, out object? report))
		{
			report = new
			{
				TotalUsers = await _context.Users.CountAsync(),
				TotalPatients = await _context.Patients.CountAsync(),
				TotalDoctors = await _context.Doctors.CountAsync(),
				TotalAppointments = await _context.Appointments.CountAsync(),
				PendingAppointments = await _context.Appointments.CountAsync(a => a.Status == "Pending"),
				CompletedAppointments = await _context.Appointments.CountAsync(a => a.Status == "Completed"),
				CancelledAppointments = await _context.Appointments.CountAsync(a => a.Status == "Cancelled"),
				TotalPrescriptions = await _context.Prescriptions.CountAsync()
			};

			_cache.Set(cacheKey, report, new MemoryCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromMinutes(5)));
		}

		return Ok(report);
	}

	[HttpGet("specializations")]
	[AllowAnonymous]
	public async Task<IActionResult> GetSpecializations()
	{
		var cacheKey = "specializations_list";
		if (!_cache.TryGetValue(cacheKey, out List<SpecializationDTO>? specs))
		{
			var specializations = await _context.Specializations.ToListAsync();
			specs = _mapper.Map<List<SpecializationDTO>>(specializations);
			_cache.Set(cacheKey, specs, new MemoryCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromMinutes(30)));
		}

		return Ok(specs);
	}
}
