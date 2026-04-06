using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartHealthcare.API.Services.Interfaces;
using SmartHealthcare.Models.DTOs;

namespace SmartHealthcare.API.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class AppointmentsController : ControllerBase
{
	private readonly IAppointmentService _service;
	private readonly IPatientService _patientService;
	private readonly IDoctorService _doctorService;
	private readonly ILogger<AppointmentsController> _logger;

	public AppointmentsController(IAppointmentService service, IPatientService patientService, IDoctorService doctorService, ILogger<AppointmentsController> logger)
	{
		_service = service;
		_patientService = patientService;
		_doctorService = doctorService;
		_logger = logger;
	}

	[HttpGet]
	[Authorize(Roles = "Admin")]
	public async Task<IActionResult> GetAll([FromQuery] DateTime? date, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
	{
		if (date.HasValue)
		{
			var result = await _service.GetByDateAsync(date.Value, pageNumber, pageSize);
			return Ok(result);
		}

		return Ok(await _service.GetAllAsync(pageNumber, pageSize));
	}

	[HttpGet("{id:int}")]
	public async Task<IActionResult> GetById(int id)
	{
		var appt = await _service.GetByIdAsync(id);
		if (appt == null) return NotFound(new ErrorResponseDTO { Message = "Appointment not found", StatusCode = 404 });
		return Ok(appt);
	}

	[HttpGet("my-appointments")]
	[Authorize(Roles = "Patient")]
	public async Task<IActionResult> GetMyAppointments([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
	{
		var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
		var patient = await _patientService.GetByUserIdAsync(userId);
		if (patient == null) return BadRequest(new ErrorResponseDTO { Message = "Please create a patient profile first", StatusCode = 400 });

		var result = await _service.GetByPatientIdAsync(patient.Id, pageNumber, pageSize);
		return Ok(result);
	}

	[HttpGet("doctor-appointments")]
	[Authorize(Roles = "Doctor")]
	public async Task<IActionResult> GetDoctorAppointments([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
	{
		var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
		var doctor = await _doctorService.GetByUserIdAsync(userId);
		if (doctor == null) return BadRequest(new ErrorResponseDTO { Message = "Doctor profile not found", StatusCode = 400 });

		var result = await _service.GetByDoctorIdAsync(doctor.Id, pageNumber, pageSize);
		return Ok(result);
	}

	[HttpPost]
	[Authorize(Roles = "Patient")]
	public async Task<IActionResult> Create([FromBody] CreateAppointmentDTO dto)
	{
		if (!ModelState.IsValid) return BadRequest(ModelState);

		var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
		var patient = await _patientService.GetByUserIdAsync(userId);
		if (patient == null) return BadRequest(new ErrorResponseDTO { Message = "Please create a patient profile first", StatusCode = 400 });

		_logger.LogInformation("Appointment booking initiated by PatientId: {PatientId}", patient.Id);
		var result = await _service.CreateAsync(patient.Id, dto);
		return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
	}

	[HttpPut("{id:int}")]
	[Authorize(Roles = "Doctor,Admin")]
	public async Task<IActionResult> Update(int id, [FromBody] UpdateAppointmentDTO dto)
	{
		if (!ModelState.IsValid) return BadRequest(ModelState);
		var success = await _service.UpdateAsync(id, dto);
		if (!success) return NotFound(new ErrorResponseDTO { Message = "Appointment not found", StatusCode = 404 });
		return Ok(new { message = "Appointment updated" });
	}

	[HttpPatch("{id:int}")]
	[Authorize(Roles = "Doctor,Admin")]
	public async Task<IActionResult> Patch(int id, [FromBody] Dictionary<string, object> patchData)
	{
		var success = await _service.PatchAsync(id, patchData);
		if (!success) return NotFound(new ErrorResponseDTO { Message = "Appointment not found", StatusCode = 404 });
		return Ok(new { message = "Appointment patched" });
	}

	[HttpDelete("{id:int}")]
	[Authorize(Roles = "Patient,Admin")]
	public async Task<IActionResult> Delete(int id)
	{
		var success = await _service.DeleteAsync(id);
		if (!success) return NotFound(new ErrorResponseDTO { Message = "Appointment not found", StatusCode = 404 });
		return Ok(new { message = "Appointment deleted" });
	}
}
