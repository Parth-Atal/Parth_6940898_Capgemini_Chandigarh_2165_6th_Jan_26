using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartHealthcare.API.Services.Interfaces;
using SmartHealthcare.Models.DTOs;

namespace SmartHealthcare.API.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class PrescriptionsController : ControllerBase
{
    private readonly IPrescriptionService _service;
    private readonly ILogger<PrescriptionsController> _logger;

    public PrescriptionsController(IPrescriptionService service, ILogger<PrescriptionsController> logger)
    {
        _service = service;
        _logger = logger;
    }

    // GET api/prescriptions
    // Admin can see all prescriptions in the system
    [HttpGet]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetAll()
    {
        var prescriptions = await _service.GetAllAsync();
        return Ok(prescriptions);
    }

    // GET api/prescriptions/5
    // Admin, the doctor who wrote it, or the patient it belongs to can view it
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var prescription = await _service.GetByIdAsync(id);
        if (prescription == null)
            return NotFound(new ErrorResponseDTO { Message = "Prescription not found", StatusCode = 404 });

        return Ok(prescription);
    }

    // GET api/prescriptions/appointment/5
    // Fetch the prescription linked to a specific appointment
    [HttpGet("appointment/{appointmentId:int}")]
    public async Task<IActionResult> GetByAppointmentId(int appointmentId)
    {
        var prescription = await _service.GetByAppointmentIdAsync(appointmentId);
        if (prescription == null)
            return NotFound(new ErrorResponseDTO { Message = "No prescription found for this appointment", StatusCode = 404 });

        return Ok(prescription);
    }

    // POST api/prescriptions
    // Only doctors can create prescriptions
    [HttpPost]
    [Authorize(Roles = "Doctor")]
    public async Task<IActionResult> Create([FromBody] CreatePrescriptionDTO dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            var result = await _service.CreateAsync(dto);

            _logger.LogInformation(
                "Doctor (UserId: {UserId}) created prescription for AppointmentId: {AppointmentId}",
                User.FindFirst(ClaimTypes.NameIdentifier)?.Value,
                dto.AppointmentId);

            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new ErrorResponseDTO { Message = ex.Message, StatusCode = 404 });
        }
        catch (InvalidOperationException ex)
        {
            // This fires when a prescription already exists for the appointment
            return Conflict(new ErrorResponseDTO { Message = ex.Message, StatusCode = 409 });
        }
    }

    // PUT api/prescriptions/5
    // Only doctors can update a prescription
    [HttpPut("{id:int}")]
    [Authorize(Roles = "Doctor")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdatePrescriptionDTO dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var success = await _service.UpdateAsync(id, dto);
        if (!success)
            return NotFound(new ErrorResponseDTO { Message = "Prescription not found", StatusCode = 404 });

        _logger.LogInformation(
            "Doctor (UserId: {UserId}) updated Prescription {PrescriptionId}",
            User.FindFirst(ClaimTypes.NameIdentifier)?.Value,
            id);

        return Ok(new { message = "Prescription updated successfully" });
    }

    // DELETE api/prescriptions/5
    // Doctors and Admins can delete a prescription
    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Doctor,Admin")]
    public async Task<IActionResult> Delete(int id)
    {
        var success = await _service.DeleteAsync(id);
        if (!success)
            return NotFound(new ErrorResponseDTO { Message = "Prescription not found", StatusCode = 404 });

        _logger.LogInformation(
            "Prescription {PrescriptionId} deleted by UserId: {UserId}",
            id,
            User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

        return Ok(new { message = "Prescription deleted successfully" });
    }
}