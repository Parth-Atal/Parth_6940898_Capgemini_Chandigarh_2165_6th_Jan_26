using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartHealthcare.API.Services.Interfaces;
using SmartHealthcare.Models.DTOs;

namespace SmartHealthcare.API.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class PatientsController : ControllerBase
{
	private readonly IPatientService _service;

	public PatientsController(IPatientService service) => _service = service;

	[HttpGet]
	[Authorize(Roles = "Admin,Doctor")]
	public async Task<IActionResult> GetAll([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
	{
		var result = await _service.GetAllAsync(pageNumber, pageSize);
		return Ok(result);
	}

	[HttpGet("{id:int}")]
	public async Task<IActionResult> GetById(int id)
	{
		var patient = await _service.GetByIdAsync(id);
		if (patient == null) return NotFound(new ErrorResponseDTO { Message = "Patient not found", StatusCode = 404 });
		return Ok(patient);
	}

	[HttpGet("my-profile")]
	[Authorize(Roles = "Patient")]
	public async Task<IActionResult> GetMyProfile()
	{
		var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
		var patient = await _service.GetByUserIdAsync(userId);
		if (patient == null) return NotFound(new ErrorResponseDTO { Message = "Profile not found. Please create one.", StatusCode = 404 });
		return Ok(patient);
	}

	[HttpPost]
	[Authorize(Roles = "Patient")]
	public async Task<IActionResult> Create([FromBody] CreatePatientDTO dto)
	{
		if (!ModelState.IsValid) return BadRequest(ModelState);
		var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

		var existing = await _service.GetByUserIdAsync(userId);
		if (existing != null) return BadRequest(new ErrorResponseDTO { Message = "Profile already exists", StatusCode = 400 });

		var result = await _service.CreateAsync(userId, dto);
		return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
	}

	[HttpPut("{id:int}")]
	[Authorize(Roles = "Patient,Admin")]
	public async Task<IActionResult> Update(int id, [FromBody] UpdatePatientDTO dto)
	{
		if (!ModelState.IsValid) return BadRequest(ModelState);
		var success = await _service.UpdateAsync(id, dto);
		if (!success) return NotFound(new ErrorResponseDTO { Message = "Patient not found", StatusCode = 404 });
		return Ok(new { message = "Patient updated successfully" });
	}

	[HttpPatch("{id:int}")]
	[Authorize(Roles = "Patient,Admin")]
	public async Task<IActionResult> Patch(int id, [FromBody] Dictionary<string, object> patchData)
	{
		var success = await _service.PatchAsync(id, patchData);
		if (!success) return NotFound(new ErrorResponseDTO { Message = "Patient not found", StatusCode = 404 });
		return Ok(new { message = "Patient patched successfully" });
	}

	[HttpDelete("{id:int}")]
	[Authorize(Roles = "Admin")]
	public async Task<IActionResult> Delete(int id)
	{
		var success = await _service.DeleteAsync(id);
		if (!success) return NotFound(new ErrorResponseDTO { Message = "Patient not found", StatusCode = 404 });
		return Ok(new { message = "Patient deleted successfully" });
	}
}
