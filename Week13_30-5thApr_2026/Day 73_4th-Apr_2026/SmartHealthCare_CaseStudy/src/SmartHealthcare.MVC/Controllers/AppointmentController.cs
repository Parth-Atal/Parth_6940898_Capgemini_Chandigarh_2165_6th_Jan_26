using Microsoft.AspNetCore.Mvc;
using SmartHealthcare.MVC.Services;
using SmartHealthcare.Models.DTOs;

namespace SmartHealthcare.MVC.Controllers;

public class AppointmentController : Controller
{
    private readonly IApiService _apiService;
    private readonly ILogger<AppointmentController> _logger;

    public AppointmentController(IApiService apiService, ILogger<AppointmentController> logger)
    {
        _apiService = apiService;
        _logger = logger;
    }

    public async Task<IActionResult> Index()
    {
        var token = HttpContext.Session.GetString("Token");
        if (string.IsNullOrEmpty(token))
            return RedirectToAction("Login", "Account");

        var role = HttpContext.Session.GetString("Role");
        PagedResult<AppointmentDTO>? result = null;

        if (role == "Patient")
        {
            result = await _apiService.GetAsync<PagedResult<AppointmentDTO>>(
                "appointments/my-appointments?pageNumber=1&pageSize=50", token);
        }
        else if (role == "Doctor")
        {
            result = await _apiService.GetAsync<PagedResult<AppointmentDTO>>(
                "appointments/doctor-appointments?pageNumber=1&pageSize=50", token);
        }
        else if (role == "Admin")
        {
            result = await _apiService.GetAsync<PagedResult<AppointmentDTO>>(
                "appointments?pageNumber=1&pageSize=50", token);
        }

        ViewBag.Role = role;
        return View(result?.Items ?? Enumerable.Empty<AppointmentDTO>());
    }

    [HttpGet]
    public async Task<IActionResult> Create()
    {
        var token = HttpContext.Session.GetString("Token");
        if (string.IsNullOrEmpty(token))
            return RedirectToAction("Login", "Account");

        var doctorsResult = await _apiService.GetAsync<PagedResult<DoctorDTO>>(
            "doctors?pageNumber=1&pageSize=100", token);
        ViewBag.Doctors = doctorsResult?.Items ?? Enumerable.Empty<DoctorDTO>();
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateAppointmentDTO dto)
    {
        var token = HttpContext.Session.GetString("Token");
        if (string.IsNullOrEmpty(token))
            return RedirectToAction("Login", "Account");

        if (!ModelState.IsValid)
        {
            var doctorsResult = await _apiService.GetAsync<PagedResult<DoctorDTO>>(
                "doctors?pageNumber=1&pageSize=100", token);
            ViewBag.Doctors = doctorsResult?.Items ?? Enumerable.Empty<DoctorDTO>();
            return View(dto);
        }

        try
        {
            var result = await _apiService.PostAsync<AppointmentDTO>("appointments", dto, token);
            if (result == null)
            {
                TempData["Error"] = "Failed to create appointment. Please ensure your patient profile is set up and all fields are valid.";
                var doctorsResult = await _apiService.GetAsync<PagedResult<DoctorDTO>>(
                    "doctors?pageNumber=1&pageSize=100", token);
                ViewBag.Doctors = doctorsResult?.Items ?? Enumerable.Empty<DoctorDTO>();
                return View(dto);
            }

            _logger.LogInformation(
                "Appointment {AppointmentId} booked successfully for doctor {DoctorId}",
                result.Id, result.DoctorId);

            TempData["Success"] = "Appointment booked successfully!";
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating appointment");
            TempData["Error"] = "An error occurred while booking the appointment. Please try again.";
            var doctorsResult = await _apiService.GetAsync<PagedResult<DoctorDTO>>(
                "doctors?pageNumber=1&pageSize=100", token);
            ViewBag.Doctors = doctorsResult?.Items ?? Enumerable.Empty<DoctorDTO>();
            return View(dto);
        }
    }

    // Single Details action — loads prescription and bill for ALL roles
    public async Task<IActionResult> Details(int id)
    {
        var token = HttpContext.Session.GetString("Token");
        if (string.IsNullOrEmpty(token))
            return RedirectToAction("Login", "Account");

        var appointment = await _apiService.GetAsync<AppointmentDTO>(
            $"appointments/{id}", token);

        if (appointment == null)
            return NotFound();

        var role = HttpContext.Session.GetString("Role");
        ViewBag.Role = role;

        // Both of these are fetched unconditionally so every role
        // (Patient, Doctor, Admin) sees the prescription and bill
        ViewBag.Prescription = await _apiService.GetAsync<PrescriptionDTO>(
            $"prescriptions/appointment/{id}", token);

        ViewBag.Bill = await _apiService.GetAsync<BillDTO>(
            $"billing/appointment/{id}", token);

        return View(appointment);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdateStatus(int id, string status)
    {
        var token = HttpContext.Session.GetString("Token");
        if (string.IsNullOrEmpty(token))
            return RedirectToAction("Login", "Account");

        var role = HttpContext.Session.GetString("Role");
        if (role != "Doctor" && role != "Admin")
        {
            TempData["Error"] = "You do not have permission to update appointment status.";
            return RedirectToAction(nameof(Details), new { id });
        }

        var patchData = new Dictionary<string, object> { { "Status", status } };
        var result = await _apiService.PatchAsync<object>($"appointments/{id}", patchData, token);

        if (result == null)
        {
            TempData["Error"] = "Failed to update appointment status.";
            return RedirectToAction(nameof(Details), new { id });
        }

        TempData["Success"] = $"Appointment marked as {status} successfully.";
        return RedirectToAction(nameof(Details), new { id });
    }
}