using BookStore.Web.Models;
using BookStore.Web.Services;
using Microsoft.AspNetCore.Mvc;

namespace BookStore.Web.Controllers;

public class AuthController : Controller
{
    private readonly ApiService _api;
    public AuthController(ApiService api) => _api = api;

    public IActionResult Login() => View();

    [HttpPost]
    public async Task<IActionResult> Login(LoginViewModel model)
    {
        if (!ModelState.IsValid) return View(model);
        var result = await _api.PostWithResponseAsync<object, AuthResult>("/api/v1/auth/login", new { model.Email, model.Password });
        if (result == null) { ModelState.AddModelError("", "Invalid credentials."); return View(model); }
        HttpContext.Session.SetString("JwtToken", result.Token);
        HttpContext.Session.SetString("UserName", result.FullName);
        HttpContext.Session.SetString("UserRole", result.Role);
        if (result.Role == "Admin") return RedirectToAction("Index", "Admin");
        return RedirectToAction("Index", "Books");
    }

    public IActionResult Register() => View();

    [HttpPost]
    public async Task<IActionResult> Register(RegisterViewModel model)
    {
        if (!ModelState.IsValid) return View(model);
        var response = await _api.PostAsync("/api/v1/auth/register", new { model.FullName, model.Email, model.Password, model.Phone });
        if (!response.Success) { ModelState.AddModelError("", response.Message); return View(model); }
        return RedirectToAction("Login");
    }

    public IActionResult Logout() { HttpContext.Session.Clear(); return RedirectToAction("Login"); }
}