using Microsoft.AspNetCore.Mvc;
using SmartHealthcare.MVC.Services;
using SmartHealthcare.Models.DTOs;

namespace SmartHealthcare.MVC.Controllers;

public class AdminController : Controller
{
    private readonly IApiService _apiService;

    public AdminController(IApiService apiService)
    {
        _apiService = apiService;
    }

    public async Task<IActionResult> Users()
    {
        var token = HttpContext.Session.GetString("Token");
        var role = HttpContext.Session.GetString("Role");
        if (string.IsNullOrEmpty(token) || role != "Admin")
        {
            return RedirectToAction("Login", "Account");
        }

        var users = await _apiService.GetAsync<PagedResult<UserDTO>>("admin/users?pageNumber=1&pageSize=100", token);
        return View(users?.Items ?? Enumerable.Empty<UserDTO>());
    }

    [HttpPost]
    public async Task<IActionResult> UpdateRole(int id, string role)
    {
        var token = HttpContext.Session.GetString("Token");
        if (string.IsNullOrEmpty(token))
        {
            return RedirectToAction("Login", "Account");
        }

        await _apiService.PutAsync<object>($"admin/users/{id}/role", new UpdateUserRoleDTO { Role = role }, token);
        return RedirectToAction(nameof(Users));
    }

    [HttpPost]
    public async Task<IActionResult> DeleteUser(int id)
    {
        var token = HttpContext.Session.GetString("Token");
        if (string.IsNullOrEmpty(token))
        {
            return RedirectToAction("Login", "Account");
        }

        await _apiService.DeleteAsync($"admin/users/{id}", token);
        return RedirectToAction(nameof(Users));
    }

    public async Task<IActionResult> Reports()
    {
        var token = HttpContext.Session.GetString("Token");
        var role = HttpContext.Session.GetString("Role");
        if (string.IsNullOrEmpty(token) || role != "Admin")
        {
            return RedirectToAction("Login", "Account");
        }

        ViewBag.Report = await _apiService.GetAsync<object>("admin/reports", token);
        return View();
    }
}