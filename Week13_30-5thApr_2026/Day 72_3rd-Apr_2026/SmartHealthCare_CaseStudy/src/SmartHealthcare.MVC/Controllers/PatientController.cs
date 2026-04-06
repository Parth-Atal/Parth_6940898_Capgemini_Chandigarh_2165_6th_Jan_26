using Microsoft.AspNetCore.Mvc;
using SmartHealthcare.MVC.Services;
using SmartHealthcare.Models.DTOs;

namespace SmartHealthcare.MVC.Controllers;

public class PatientController : Controller
{
    private readonly IApiService _apiService;

    public PatientController(IApiService apiService)
    {
        _apiService = apiService;
    }

    public async Task<IActionResult> Profile()
    {
        var token = HttpContext.Session.GetString("Token");
        if (string.IsNullOrEmpty(token))
        {
            return RedirectToAction("Login", "Account");
        }

        var role = HttpContext.Session.GetString("Role");
        if (role == "Doctor")
        {
            return RedirectToAction("Profile", "Doctor");
        }
        if (role == "Admin")
        {
            return RedirectToAction("Index", "Dashboard");
        }

        var patient = await _apiService.GetAsync<PatientDTO>("patients/my-profile", token);
        if (patient == null)
        {
            ViewBag.NeedsProfile = true;
            return View(new PatientDTO());
        }

        return View(patient);
    }

    [HttpPost]
    public async Task<IActionResult> CreateProfile(CreatePatientDTO dto)
    {
        var token = HttpContext.Session.GetString("Token");
        if (string.IsNullOrEmpty(token))
        {
            return RedirectToAction("Login", "Account");
        }

        var role = HttpContext.Session.GetString("Role");
        if (role == "Doctor")
        {
            return RedirectToAction("Profile", "Doctor");
        }

        if (!ModelState.IsValid)
        if (string.IsNullOrEmpty(token))
        {
            return RedirectToAction("Login", "Account");
        }

        if (!ModelState.IsValid)
        {
            ViewBag.NeedsProfile = true;
            return View("Profile", new PatientDTO());
        }

        var result = await _apiService.PostAsync<PatientDTO>("patients", dto, token);
        if (result == null)
        {
            ModelState.AddModelError(string.Empty, "Failed to create profile");
            ViewBag.NeedsProfile = true;
            return View("Profile", new PatientDTO());
        }

        TempData["Success"] = "Profile created successfully!";
        return RedirectToAction(nameof(Profile));
    }
}