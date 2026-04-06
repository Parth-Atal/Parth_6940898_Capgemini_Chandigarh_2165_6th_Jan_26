using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartHealthcare.API.Services.Interfaces;
using SmartHealthcare.Models.DTOs;

namespace SmartHealthcare.API.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class BillingController : ControllerBase
{
    private readonly IBillingService _service;
    private readonly IPatientService _patientService;
    private readonly ILogger<BillingController> _logger;

    public BillingController(
        IBillingService service,
        IPatientService patientService,
        ILogger<BillingController> logger)
    {
        _service = service;
        _patientService = patientService;
        _logger = logger;
    }

    // GET api/billing
    // Admin sees all bills with pagination
    [HttpGet]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetAll(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 20)
    {
        var result = await _service.GetAllAsync(pageNumber, pageSize);
        return Ok(result);
    }

    // GET api/billing/{id}
    // Admin, the patient on the bill, or the doctor on the appointment can view it
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var bill = await _service.GetByIdAsync(id);
        if (bill == null)
            return NotFound(new ErrorResponseDTO { Message = "Bill not found", StatusCode = 404 });

        return Ok(bill);
    }

    // GET api/billing/appointment/{appointmentId}
    // Fetch the bill linked to a specific appointment
    [HttpGet("appointment/{appointmentId:int}")]
    public async Task<IActionResult> GetByAppointmentId(int appointmentId)
    {
        var bill = await _service.GetByAppointmentIdAsync(appointmentId);
        if (bill == null)
            return NotFound(new ErrorResponseDTO
            {
                Message = "No bill found for this appointment",
                StatusCode = 404
            });

        return Ok(bill);
    }

    // GET api/billing/my-bills
    // Patient fetches their own billing history
    [HttpGet("my-bills")]
    [Authorize(Roles = "Patient")]
    public async Task<IActionResult> GetMyBills(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10)
    {
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var patient = await _patientService.GetByUserIdAsync(userId);

        if (patient == null)
            return BadRequest(new ErrorResponseDTO
            {
                Message = "Patient profile not found. Please create your profile first.",
                StatusCode = 400
            });

        var result = await _service.GetByPatientIdAsync(patient.Id, pageNumber, pageSize);
        return Ok(result);
    }

    // POST api/billing/generate
    // Admin or Doctor generates a bill for a completed appointment
    [HttpPost("generate")]
    [Authorize(Roles = "Admin,Doctor")]
    public async Task<IActionResult> Generate([FromBody] GenerateBillDTO dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            var result = await _service.GenerateBillAsync(dto);

            _logger.LogInformation(
                "Bill generated for AppointmentId {AppointmentId} by UserId {UserId}",
                dto.AppointmentId,
                User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new ErrorResponseDTO { Message = ex.Message, StatusCode = 404 });
        }
        catch (InvalidOperationException ex)
        {
            // Fires when a bill already exists for this appointment
            return Conflict(new ErrorResponseDTO { Message = ex.Message, StatusCode = 409 });
        }
    }

    // PUT api/billing/{id}
    // Admin or Doctor updates the payment status (Paid / Unpaid / Waived)
    [HttpPut("{id:int}")]
    
    public async Task<IActionResult> UpdatePaymentStatus(int id, [FromBody] UpdateBillDTO dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var success = await _service.UpdatePaymentStatusAsync(id, dto);
        if (!success)
            return NotFound(new ErrorResponseDTO { Message = "Bill not found", StatusCode = 404 });

        _logger.LogInformation(
            "Bill {BillId} updated to status {Status} by UserId {UserId}",
            id, dto.PaymentStatus,
            User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

        return Ok(new { message = $"Bill updated successfully. Status: {dto.PaymentStatus}" });
    }

    // DELETE api/billing/{id}
    // Admin only — hard delete a bill record
    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int id)
    {
        var success = await _service.DeleteAsync(id);
        if (!success)
            return NotFound(new ErrorResponseDTO { Message = "Bill not found", StatusCode = 404 });

        _logger.LogInformation(
            "Bill {BillId} deleted by UserId {UserId}",
            id,
            User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

        return Ok(new { message = "Bill deleted successfully" });
    }
}