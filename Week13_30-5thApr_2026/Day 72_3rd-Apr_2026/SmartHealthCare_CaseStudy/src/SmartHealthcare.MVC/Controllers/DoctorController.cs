using Microsoft.AspNetCore.Mvc;
using SmartHealthcare.MVC.Services;
using SmartHealthcare.Models.DTOs;

namespace SmartHealthcare.MVC.Controllers;

public class DoctorController : Controller
{
    private readonly IApiService _apiService;

    public DoctorController(IApiService apiService)
    {
        _apiService = apiService;
    }

    public async Task<IActionResult> Index(string? specialization, string? name, int page = 1)
    {
        var token = HttpContext.Session.GetString("Token");
        var endpoint = $"doctors?pageNumber={page}&pageSize=20";

        if (!string.IsNullOrWhiteSpace(specialization))
        {
            endpoint = $"doctors/search?specialization={Uri.EscapeDataString(specialization)}&pageNumber={page}&pageSize=20";
        }

        var result = await _apiService.GetAsync<PagedResult<DoctorDTO>>(endpoint, token);
        ViewBag.Specialization = specialization;
        ViewBag.Name = name;
        return View(result ?? new PagedResult<DoctorDTO>());
    }

    public async Task<IActionResult> Details(int id)
    {
        var token = HttpContext.Session.GetString("Token");
        var doctor = await _apiService.GetAsync<DoctorDTO>($"doctors/{id}", token);
        if (doctor == null)
        {
            return NotFound();
        }

        return View(doctor);
    }

    public async Task<IActionResult> Profile()
    {
        var token = HttpContext.Session.GetString("Token");
        if (string.IsNullOrEmpty(token))
        {
            return RedirectToAction("Login", "Account");
        }

        var role = HttpContext.Session.GetString("Role");
        if (role != "Doctor")
        {
            TempData["Error"] = "You are not a doctor account and have been redirected to the appropriate profile page.";
            if (role == "Patient") return RedirectToAction("Profile", "Patient");
            return RedirectToAction("Index", "Dashboard");
        }

        var doctor = await _apiService.GetAsync<DoctorDTO>("doctors/my-profile", token);
        if (doctor == null)
        {
            var specializations = await _apiService.GetAsync<List<SpecializationDTO>>("admin/specializations", token);
            ViewBag.Specializations = specializations ?? new List<SpecializationDTO>();
            return View("CreateProfile", new CreateDoctorDTO { IsAvailable = true });
        }

        return View("Details", doctor);
    }

    public async Task<IActionResult> Edit()
    {
        var token = HttpContext.Session.GetString("Token");
        if (string.IsNullOrEmpty(token))
        {
            return RedirectToAction("Login", "Account");
        }

        var doctor = await _apiService.GetAsync<DoctorDTO>("doctors/my-profile", token);
        if (doctor == null)
        {
            TempData["Error"] = "Doctor profile not found. Please create your profile first.";
            return RedirectToAction("Profile");
        }

        var specializations = await _apiService.GetAsync<List<SpecializationDTO>>("admin/specializations", token);
        ViewBag.Specializations = specializations ?? new List<SpecializationDTO>();

        var selectedSpecIds = (doctor.Specializations ?? new List<string>())
            .Select(spec => spec.Trim())
            .Where(spec => !string.IsNullOrEmpty(spec))
            .ToList();

        var allSpecs = ViewBag.Specializations as List<SpecializationDTO> ?? new List<SpecializationDTO>();

        var model = new UpdateDoctorDTO
        {
            LicenseNumber = doctor.LicenseNumber,
            YearsOfExperience = doctor.YearsOfExperience,
            ConsultationFee = doctor.ConsultationFee,
            Phone = doctor.Phone,
            IsAvailable = doctor.IsAvailable,
            SpecializationIds = allSpecs
                .Where(s => selectedSpecIds.Any(selected => selected.Equals(s.Name, StringComparison.OrdinalIgnoreCase)))
                .Select(s => s.Id)
                .ToList()
        };

        return View("EditProfile", model);
    }

    [HttpPost]
    public async Task<IActionResult> Edit(UpdateDoctorDTO dto)
    {
        var token = HttpContext.Session.GetString("Token");
        if (string.IsNullOrEmpty(token))
        {
            return RedirectToAction("Login", "Account");
        }

        if (!ModelState.IsValid)
        {
            var specializations = await _apiService.GetAsync<List<SpecializationDTO>>("admin/specializations", token);
            ViewBag.Specializations = specializations ?? new List<SpecializationDTO>();
            return View("EditProfile", dto);
        }

        var myDoctor = await _apiService.GetAsync<DoctorDTO>("doctors/my-profile", token);
        if (myDoctor == null)
        {
            TempData["Error"] = "Doctor profile not found. Please create your profile first.";
            return RedirectToAction("Profile");
        }

        var updated = await _apiService.PutAsync<DoctorDTO>($"doctors/{myDoctor.Id}", dto, token);
        if (updated == null)
        {
            ModelState.AddModelError(string.Empty, "Failed to update doctor profile.");
            var specializations = await _apiService.GetAsync<List<SpecializationDTO>>("admin/specializations", token);
            ViewBag.Specializations = specializations ?? new List<SpecializationDTO>();
            return View("EditProfile", dto);
        }

        TempData["Success"] = "Doctor profile updated successfully!";
        return RedirectToAction("Profile");
    }

    [HttpPost]
    public async Task<IActionResult> CreateProfile(CreateDoctorDTO dto)
    {
        var token = HttpContext.Session.GetString("Token");
        if (string.IsNullOrEmpty(token))
        {
            return RedirectToAction("Login", "Account");
        }

        if (!ModelState.IsValid)
        {
            var specializations = await _apiService.GetAsync<List<SpecializationDTO>>("admin/specializations", token);
            ViewBag.Specializations = specializations ?? new List<SpecializationDTO>();
            return View("CreateProfile", dto);
        }

        var result = await _apiService.PostAsync<DoctorDTO>("doctors", dto, token);
        if (result == null)
        {
            ModelState.AddModelError(string.Empty, "Failed to create doctor profile. Please ensure your information is correct and complete.");
            var specializations = await _apiService.GetAsync<List<SpecializationDTO>>("admin/specializations", token);
            ViewBag.Specializations = specializations ?? new List<SpecializationDTO>();
            return View("CreateProfile", dto);
        }

        TempData["Success"] = "Doctor profile created successfully!";
        return RedirectToAction("Profile");
    }
}