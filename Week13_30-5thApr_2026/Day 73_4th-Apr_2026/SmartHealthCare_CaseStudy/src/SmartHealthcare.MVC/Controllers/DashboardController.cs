using Microsoft.AspNetCore.Mvc;
using SmartHealthcare.MVC.Services;

namespace SmartHealthcare.MVC.Controllers;

public class DashboardController : Controller
{
    private readonly IApiService _apiService;

    public DashboardController(IApiService apiService)
    {
        _apiService = apiService;
    }

    public async Task<IActionResult> Index()
    {
        var token = HttpContext.Session.GetString("Token");
        if (string.IsNullOrEmpty(token))
        {
            return RedirectToAction("Login", "Account");
        }

        ViewBag.Role = HttpContext.Session.GetString("Role");
        ViewBag.FullName = HttpContext.Session.GetString("FullName");

        if ((ViewBag.Role as string) == "Admin")
        {
            ViewBag.Report = await _apiService.GetAsync<object>("admin/reports", token);
        }

        return View();
    }
}