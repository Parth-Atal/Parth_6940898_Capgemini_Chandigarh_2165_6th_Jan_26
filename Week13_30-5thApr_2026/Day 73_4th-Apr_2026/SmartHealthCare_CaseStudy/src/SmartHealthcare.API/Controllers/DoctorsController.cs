using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartHealthcare.API.Services.Interfaces;
using SmartHealthcare.Models.DTOs;

namespace SmartHealthcare.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class DoctorsController : ControllerBase
{
	private readonly IDoctorService _service;

	public DoctorsController(IDoctorService service) => _service = service;

	[HttpGet]
	public async Task<IActionResult> GetAll([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 100000)
	{
		var result = await _service.GetAllAsync(pageNumber, pageSize);
		return Ok(result);
	}

	[HttpGet("{id:int}")]
	public async Task<IActionResult> GetById(int id)
	{
		var doctor = await _service.GetByIdAsync(id);
		if (doctor == null) return NotFound(new ErrorResponseDTO { Message = "Doctor not found", StatusCode = 404 });
		return Ok(doctor);
	}

	[HttpGet("search")]
	public async Task<IActionResult> SearchBySpecialization([FromQuery] string specialization, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
	{
		if (string.IsNullOrEmpty(specialization)) return BadRequest(new ErrorResponseDTO { Message = "Specialization is required", StatusCode = 400 });

		var result = await _service.SearchBySpecializationAsync(specialization, pageNumber, pageSize);
		return Ok(result);
	}

	[HttpGet("my-profile")]
	[Authorize(Roles = "Doctor")]
	public async Task<IActionResult> GetMyProfile()
	{
		var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
		var doctor = await _service.GetByUserIdAsync(userId);
		if (doctor == null) return NotFound(new ErrorResponseDTO { Message = "Profile not found", StatusCode = 404 });
		return Ok(doctor);
	}

	[HttpPost]
	[Authorize(Roles = "Admin,Doctor")]
	public async Task<IActionResult> Create([FromBody] CreateDoctorDTO dto)
	{
		if (!ModelState.IsValid) return BadRequest(ModelState);
		var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
		var result = await _service.CreateAsync(userId, dto);
		return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
	}

	[HttpPut("{id:int}")]
	[Authorize(Roles = "Admin,Doctor")]
	public async Task<IActionResult> Update(int id, [FromBody] UpdateDoctorDTO dto)
	{
		if (!ModelState.IsValid) return BadRequest(ModelState);
		var success = await _service.UpdateAsync(id, dto);
		if (!success) return NotFound(new ErrorResponseDTO { Message = "Doctor not found", StatusCode = 404 });
		return Ok(new { message = "Doctor updated successfully" });
	}

	[HttpPatch("{id:int}")]
	[Authorize(Roles = "Admin,Doctor")]
	public async Task<IActionResult> Patch(int id, [FromBody] Dictionary<string, object> patchData)
	{
		var success = await _service.PatchAsync(id, patchData);
		if (!success) return NotFound(new ErrorResponseDTO { Message = "Doctor not found", StatusCode = 404 });
		return Ok(new { message = "Doctor patched successfully" });
	}

	[HttpDelete("{id:int}")]
	[Authorize(Roles = "Admin")]
	public async Task<IActionResult> Delete(int id)
	{
		var success = await _service.DeleteAsync(id);
		if (!success) return NotFound(new ErrorResponseDTO { Message = "Doctor not found", StatusCode = 404 });
		return Ok(new { message = "Doctor deleted successfully" });
	}
}
