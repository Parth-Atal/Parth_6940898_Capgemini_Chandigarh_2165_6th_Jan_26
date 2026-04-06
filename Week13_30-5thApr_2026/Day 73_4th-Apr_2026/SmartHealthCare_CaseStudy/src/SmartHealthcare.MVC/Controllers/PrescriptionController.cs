using Microsoft.AspNetCore.Mvc;
using SmartHealthcare.MVC.Services;
using SmartHealthcare.Models.DTOs;

namespace SmartHealthcare.MVC.Controllers;

public class PrescriptionController : Controller
{
    private readonly IApiService _apiService;
    private readonly ILogger<PrescriptionController> _logger;

    public PrescriptionController(IApiService apiService, ILogger<PrescriptionController> logger)
    {
        _apiService = apiService;
        _logger = logger;
    }

    
    public async Task<IActionResult> Details(int id)
    {
        var token = HttpContext.Session.GetString("Token");
        if (string.IsNullOrEmpty(token))
            return RedirectToAction("Login", "Account");

        var prescription = await _apiService.GetAsync<PrescriptionDTO>($"prescriptions/{id}", token);
        if (prescription == null)
        {
            TempData["Error"] = "Prescription not found.";
            return RedirectToAction("Index", "Appointment");
        }

        ViewBag.Role = HttpContext.Session.GetString("Role");
        return View(prescription);
    }

    
    [HttpGet]
    public async Task<IActionResult> Create(int appointmentId)
    {
        var token = HttpContext.Session.GetString("Token");
        if (string.IsNullOrEmpty(token))
            return RedirectToAction("Login", "Account");

        var role = HttpContext.Session.GetString("Role");
        if (role != "Doctor")
        {
            TempData["Error"] = "Only doctors can create prescriptions.";
            return RedirectToAction("Index", "Appointment");
        }

        var appointment = await _apiService.GetAsync<AppointmentDTO>($"appointments/{appointmentId}", token);
        if (appointment == null)
        {
            TempData["Error"] = "Appointment not found.";
            return RedirectToAction("Index", "Appointment");
        }

        var existingPrescription = await _apiService.GetAsync<PrescriptionDTO>($"prescriptions/appointment/{appointmentId}", token);
        if (existingPrescription != null)
        {
            TempData["Error"] = "A prescription already exists for this appointment. You can edit it instead.";
            return RedirectToAction("Edit", new { id = existingPrescription.Id });
        }

        
        var dto = new CreatePrescriptionDTO
        {
            AppointmentId = appointmentId,
            Medicines = new List<CreateMedicineDTO>
            {
                
                new CreateMedicineDTO()
            }
        };

        ViewBag.Appointment = appointment;
        return View(dto);
    }

    // POST: /Prescription/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreatePrescriptionDTO dto)
    {
        var token = HttpContext.Session.GetString("Token");
        if (string.IsNullOrEmpty(token))
            return RedirectToAction("Login", "Account");

        var role = HttpContext.Session.GetString("Role");
        if (role != "Doctor")
        {
            TempData["Error"] = "Only doctors can create prescriptions.";
            return RedirectToAction("Index", "Appointment");
        }

        
        CleanUpEmptyMedicines(dto);

        if (!ModelState.IsValid)
        {
            var appointment = await _apiService.GetAsync<AppointmentDTO>($"appointments/{dto.AppointmentId}", token);
            ViewBag.Appointment = appointment;
            return View(dto);
        }

        try
        {
            var result = await _apiService.PostAsync<PrescriptionDTO>("prescriptions", dto, token);
            if (result == null)
            {
                TempData["Error"] = "Failed to create the prescription. Please check all fields and try again.";
                var appointment = await _apiService.GetAsync<AppointmentDTO>($"appointments/{dto.AppointmentId}", token);
                ViewBag.Appointment = appointment;
                return View(dto);
            }

            _logger.LogInformation("Prescription created for AppointmentId {AppointmentId}", dto.AppointmentId);
            TempData["Success"] = "Prescription created successfully!";
            return RedirectToAction("Details", "Appointment", new { id = dto.AppointmentId });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating prescription for AppointmentId {AppointmentId}", dto.AppointmentId);
            TempData["Error"] = "An unexpected error occurred while creating the prescription. Please try again.";
            var appointment = await _apiService.GetAsync<AppointmentDTO>($"appointments/{dto.AppointmentId}", token);
            ViewBag.Appointment = appointment;
            return View(dto);
        }
    }

    // GET: /Prescription/Edit/5
    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var token = HttpContext.Session.GetString("Token");
        if (string.IsNullOrEmpty(token))
            return RedirectToAction("Login", "Account");

        var role = HttpContext.Session.GetString("Role");
        if (role != "Doctor")
        {
            TempData["Error"] = "Only doctors can edit prescriptions.";
            return RedirectToAction("Index", "Appointment");
        }

        var prescription = await _apiService.GetAsync<PrescriptionDTO>($"prescriptions/{id}", token);
        if (prescription == null)
        {
            TempData["Error"] = "Prescription not found.";
            return RedirectToAction("Index", "Appointment");
        }

        // Load appointment for context display
        var appointment = await _apiService.GetAsync<AppointmentDTO>($"appointments/{prescription.AppointmentId}", token);
        ViewBag.Appointment = appointment;
        ViewBag.PrescriptionId = id;

        // Map existing prescription data into the update DTO for the form
        var dto = new UpdatePrescriptionDTO
        {
            Diagnosis = prescription.Diagnosis,
            Notes = prescription.Notes,
            Medicines = prescription.Medicines.Select(m => new CreateMedicineDTO
            {
                Name = m.Name,
                Dosage = m.Dosage,
                Duration = m.Duration,
                Instructions = m.Instructions
            }).ToList()
        };

        // Make sure there is at least one medicine row visible
        if (!dto.Medicines.Any())
        {
            dto.Medicines.Add(new CreateMedicineDTO());
        }

        return View(dto);
    }

    // POST: /Prescription/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, UpdatePrescriptionDTO dto)
    {
        var token = HttpContext.Session.GetString("Token");
        if (string.IsNullOrEmpty(token))
            return RedirectToAction("Login", "Account");

        var role = HttpContext.Session.GetString("Role");
        if (role != "Doctor")
        {
            TempData["Error"] = "Only doctors can edit prescriptions.";
            return RedirectToAction("Index", "Appointment");
        }

        CleanUpEmptyMedicines(dto);

        if (!ModelState.IsValid)
        {
            // We need to reload the appointment for the view header
            var prescription = await _apiService.GetAsync<PrescriptionDTO>($"prescriptions/{id}", token);
            if (prescription != null)
            {
                var appointment = await _apiService.GetAsync<AppointmentDTO>($"appointments/{prescription.AppointmentId}", token);
                ViewBag.Appointment = appointment;
            }

            ViewBag.PrescriptionId = id;
            return View(dto);
        }

        try
        {
            var success = await _apiService.PutAsync<object>($"prescriptions/{id}", dto, token);
            if (success == null)
            {
                TempData["Error"] = "Failed to update the prescription. Please try again.";
                ViewBag.PrescriptionId = id;
                return View(dto);
            }

            _logger.LogInformation("Prescription {PrescriptionId} updated successfully", id);
            TempData["Success"] = "Prescription updated successfully!";

            // Go back to the prescription details after a successful edit
            return RedirectToAction("Details", new { id });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating prescription {PrescriptionId}", id);
            TempData["Error"] = "An unexpected error occurred while updating the prescription. Please try again.";
            ViewBag.PrescriptionId = id;
            return View(dto);
        }
    }

    // POST: /Prescription/Delete/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id, int appointmentId)
    {
        var token = HttpContext.Session.GetString("Token");
        if (string.IsNullOrEmpty(token))
            return RedirectToAction("Login", "Account");

        var role = HttpContext.Session.GetString("Role");
        if (role != "Doctor" && role != "Admin")
        {
            TempData["Error"] = "You do not have permission to delete prescriptions.";
            return RedirectToAction("Index", "Appointment");
        }

        try
        {
            await _apiService.DeleteAsync($"prescriptions/{id}", token);
            _logger.LogInformation("Prescription {PrescriptionId} deleted", id);
            TempData["Success"] = "Prescription deleted successfully.";
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting prescription {PrescriptionId}", id);
            TempData["Error"] = "Failed to delete the prescription. Please try again.";
        }

        // After deletion, go back to the appointment that owned this prescription
        return RedirectToAction("Details", "Appointment", new { id = appointmentId });
    }

    // Helper: strips out completely empty medicine entries before validation
    // so doctors don't get validation errors for rows they never intended to fill
    private void CleanUpEmptyMedicines(CreatePrescriptionDTO dto)
    {
        dto.Medicines = dto.Medicines
            .Where(m => !string.IsNullOrWhiteSpace(m.Name)
                     || !string.IsNullOrWhiteSpace(m.Dosage)
                     || !string.IsNullOrWhiteSpace(m.Duration))
            .ToList();

        // Clear the stale ModelState entries so re-validation works correctly
        foreach (var key in ModelState.Keys.Where(k => k.StartsWith("Medicines")).ToList())
            ModelState.Remove(key);
    }

    private void CleanUpEmptyMedicines(UpdatePrescriptionDTO dto)
    {
        dto.Medicines = dto.Medicines
            .Where(m => !string.IsNullOrWhiteSpace(m.Name)
                     || !string.IsNullOrWhiteSpace(m.Dosage)
                     || !string.IsNullOrWhiteSpace(m.Duration))
            .ToList();

        foreach (var key in ModelState.Keys.Where(k => k.StartsWith("Medicines")).ToList())
            ModelState.Remove(key);
    }
}