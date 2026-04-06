using Microsoft.AspNetCore.Mvc;
using SmartHealthcare.MVC.Services;
using SmartHealthcare.Models.DTOs;

namespace SmartHealthcare.MVC.Controllers;

public class BillingController : Controller
{
    private readonly IApiService _apiService;
    private readonly ILogger<BillingController> _logger;

    public BillingController(IApiService apiService, ILogger<BillingController> logger)
    {
        _apiService = apiService;
        _logger = logger;
    }

    // ── Helper: redirects to login if no session token
    private string? GetToken()
    {
        return HttpContext.Session.GetString("Token");
    }

    // ================================================================
    //  INDEX — billing history list
    //  Patient sees their own bills, Admin sees all bills
    // ================================================================

    public async Task<IActionResult> Index(int page = 1)
    {
        var token = GetToken();
        if (string.IsNullOrEmpty(token))
            return RedirectToAction("Login", "Account");

        var role = HttpContext.Session.GetString("Role");
        PagedResult<BillDTO>? result = null;

        if (role == "Patient")
        {
            result = await _apiService.GetAsync<PagedResult<BillDTO>>(
                $"billing/my-bills?pageNumber={page}&pageSize=10", token);
        }
        else if (role == "Admin")
        {
            result = await _apiService.GetAsync<PagedResult<BillDTO>>(
                $"billing?pageNumber={page}&pageSize=20", token);
        }
        else if (role == "Doctor")
        {
            // Doctors don't have a dedicated billing list endpoint
            // but we still let them reach this page and show a message
            TempData["Info"] = "Doctors can view individual bills from the appointment details page.";
            return View(new PagedResult<BillDTO>());
        }

        ViewBag.Role = role;
        ViewBag.CurrentPage = page;
        ViewBag.TotalCount = result?.TotalCount ?? 0;
        ViewBag.PageSize = role == "Admin" ? 20 : 10;

        return View(result ?? new PagedResult<BillDTO>());
    }

    // ================================================================
    //  DETAILS — full bill page with printable breakdown
    // ================================================================

    public async Task<IActionResult> Details(int id)
    {
        var token = GetToken();
        if (string.IsNullOrEmpty(token))
            return RedirectToAction("Login", "Account");

        var bill = await _apiService.GetAsync<BillDTO>($"billing/{id}", token);
        if (bill == null)
        {
            TempData["Error"] = "Bill not found.";
            return RedirectToAction(nameof(Index));
        }

        ViewBag.Role = HttpContext.Session.GetString("Role");
        return View(bill);
    }

    // ================================================================
    //  GET BY APPOINTMENT — redirect to the bill for a given appointment
    //  Used from the Appointment Details page
    // ================================================================

    public async Task<IActionResult> ForAppointment(int appointmentId)
    {
        var token = GetToken();
        if (string.IsNullOrEmpty(token))
            return RedirectToAction("Login", "Account");

        var bill = await _apiService.GetAsync<BillDTO>(
            $"billing/appointment/{appointmentId}", token);

        if (bill == null)
        {
            var role = HttpContext.Session.GetString("Role");

            // If no bill exists yet, and the user is a doctor or admin,
            // send them straight to the generate form
            if (role == "Doctor" || role == "Admin")
            {
                return RedirectToAction(nameof(Generate), new { appointmentId });
            }

            TempData["Info"] = "No bill has been generated for this appointment yet.";
            return RedirectToAction("Details", "Appointment", new { id = appointmentId });
        }

        return RedirectToAction(nameof(Details), new { id = bill.Id });
    }

    // ================================================================
    //  GENERATE — form for doctor/admin to create a bill
    // ================================================================

    [HttpGet]
    public async Task<IActionResult> Generate(int appointmentId)
    {
        var token = GetToken();
        if (string.IsNullOrEmpty(token))
            return RedirectToAction("Login", "Account");

        var role = HttpContext.Session.GetString("Role");
        if (role != "Doctor" && role != "Admin")
        {
            TempData["Error"] = "Only doctors and admins can generate bills.";
            return RedirectToAction("Index", "Appointment");
        }

        // Check a bill doesn't already exist
        var existingBill = await _apiService.GetAsync<BillDTO>(
            $"billing/appointment/{appointmentId}", token);

        if (existingBill != null)
        {
            TempData["Info"] = "A bill already exists for this appointment.";
            return RedirectToAction(nameof(Details), new { id = existingBill.Id });
        }

        // Load appointment so we can show context on the form
        var appointment = await _apiService.GetAsync<AppointmentDTO>(
            $"appointments/{appointmentId}", token);

        if (appointment == null)
        {
            TempData["Error"] = "Appointment not found.";
            return RedirectToAction("Index", "Appointment");
        }

        // Load the prescription so we can show the medicine list on the form
        // and auto-calculate a suggested medicine charge
        var prescription = await _apiService.GetAsync<PrescriptionDTO>(
            $"prescriptions/appointment/{appointmentId}", token);

        ViewBag.Appointment = appointment;
        ViewBag.Prescription = prescription;

        // Pre-fill with sensible defaults so the doctor can just hit Generate
        var dto = new GenerateBillDTO
        {
            AppointmentId = appointmentId,
            PerMedicineCharge = 50
        };

        return View(dto);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Generate(GenerateBillDTO dto)
    {
        var token = GetToken();
        if (string.IsNullOrEmpty(token))
            return RedirectToAction("Login", "Account");

        var role = HttpContext.Session.GetString("Role");
        if (role != "Doctor" && role != "Admin")
        {
            TempData["Error"] = "Only doctors and admins can generate bills.";
            return RedirectToAction("Index", "Appointment");
        }

        if (!ModelState.IsValid)
        {
            await ReloadGenerateViewBag(dto.AppointmentId, token);
            return View(dto);
        }

        try
        {
            var result = await _apiService.PostAsync<BillDTO>("billing/generate", dto, token);

            if (result == null)
            {
                TempData["Error"] = "Failed to generate bill. A bill may already exist for this appointment.";
                await ReloadGenerateViewBag(dto.AppointmentId, token);
                return View(dto);
            }

            _logger.LogInformation(
                "Bill {BillId} generated for AppointmentId {AppointmentId}",
                result.Id, dto.AppointmentId);

            TempData["Success"] = $"Bill generated successfully! Total amount: ₹{result.TotalAmount:N2}";
            return RedirectToAction(nameof(Details), new { id = result.Id });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating bill for AppointmentId {AppointmentId}",
                dto.AppointmentId);
            TempData["Error"] = "An unexpected error occurred while generating the bill.";
            await ReloadGenerateViewBag(dto.AppointmentId, token);
            return View(dto);
        }
    }

    // ================================================================
    //  MARK AS PAID — quick action button on the details page
    // ================================================================

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> MarkAsPaid(int id, string? notes)
    {
        var token = GetToken();
        if (string.IsNullOrEmpty(token))
            return RedirectToAction("Login", "Account");

        var dto = new UpdateBillDTO
        {
            PaymentStatus = "Paid",
            Notes = notes
        };

        var result = await _apiService.PutAsync<object>($"billing/{id}", dto, token);
        if (result == null)
        {
            TempData["Error"] = "Failed to record payment. Please try again.";
            return RedirectToAction(nameof(Details), new { id });
        }

        _logger.LogInformation("Bill {BillId} marked as Paid", id);
        TempData["Success"] = "Payment confirmed successfully! Your bill is now marked as Paid.";
        return RedirectToAction(nameof(Details), new { id });
    }

    // ================================================================
    //  MARK AS UNPAID — in case payment needs to be reversed
    // ================================================================

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> MarkAsUnpaid(int id)
    {
        var token = GetToken();
        if (string.IsNullOrEmpty(token))
            return RedirectToAction("Login", "Account");

        var role = HttpContext.Session.GetString("Role");
        if (role != "Admin")
        {
            TempData["Error"] = "Only admins can revert a payment.";
            return RedirectToAction(nameof(Details), new { id });
        }

        var dto = new UpdateBillDTO { PaymentStatus = "Unpaid" };
        var result = await _apiService.PutAsync<object>($"billing/{id}", dto, token);

        if (result == null)
        {
            TempData["Error"] = "Failed to revert payment status.";
            return RedirectToAction(nameof(Details), new { id });
        }

        TempData["Success"] = "Bill status reverted to Unpaid.";
        return RedirectToAction(nameof(Details), new { id });
    }

    // ================================================================
    //  WAIVE — admin can waive a bill entirely
    // ================================================================

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Waive(int id, string? notes)
    {
        var token = GetToken();
        if (string.IsNullOrEmpty(token))
            return RedirectToAction("Login", "Account");

        var role = HttpContext.Session.GetString("Role");
        if (role != "Admin")
        {
            TempData["Error"] = "Only admins can waive bills.";
            return RedirectToAction(nameof(Details), new { id });
        }

        var dto = new UpdateBillDTO
        {
            PaymentStatus = "Waived",
            Notes = notes
        };

        var result = await _apiService.PutAsync<object>($"billing/{id}", dto, token);
        if (result == null)
        {
            TempData["Error"] = "Failed to waive bill.";
            return RedirectToAction(nameof(Details), new { id });
        }

        TempData["Success"] = "Bill has been waived successfully.";
        return RedirectToAction(nameof(Details), new { id });
    }

    // ================================================================
    //  DELETE — admin only
    // ================================================================

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id, int appointmentId)
    {
        var token = GetToken();
        if (string.IsNullOrEmpty(token))
            return RedirectToAction("Login", "Account");

        var role = HttpContext.Session.GetString("Role");
        if (role != "Admin")
        {
            TempData["Error"] = "Only admins can delete bills.";
            return RedirectToAction(nameof(Details), new { id });
        }

        await _apiService.DeleteAsync($"billing/{id}", token);

        _logger.LogInformation("Bill {BillId} deleted by admin", id);
        TempData["Success"] = "Bill deleted successfully.";
        return RedirectToAction("Details", "Appointment", new { id = appointmentId });
    }

    // ── Reloads ViewBag data when the Generate form fails
    private async Task ReloadGenerateViewBag(int appointmentId, string token)
    {
        ViewBag.Appointment = await _apiService.GetAsync<AppointmentDTO>(
            $"appointments/{appointmentId}", token);
        ViewBag.Prescription = await _apiService.GetAsync<PrescriptionDTO>(
            $"prescriptions/appointment/{appointmentId}", token);
    }
}