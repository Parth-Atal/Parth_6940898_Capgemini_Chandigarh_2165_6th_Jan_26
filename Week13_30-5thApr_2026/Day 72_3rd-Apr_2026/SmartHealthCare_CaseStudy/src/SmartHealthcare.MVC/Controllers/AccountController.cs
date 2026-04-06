using Microsoft.AspNetCore.Mvc;
using SmartHealthcare.MVC.Services;
using SmartHealthcare.Models.DTOs;

namespace SmartHealthcare.MVC.Controllers;

public class AccountController : Controller
{
    private readonly IApiService _apiService;
    private readonly ILogger<AccountController> _logger;

    public AccountController(IApiService apiService, ILogger<AccountController> logger)
    {
        _apiService = apiService;
        _logger = logger;
    }

    [HttpGet]
    public IActionResult Login()
    {
        if (HttpContext.Session.GetString("Token") != null)
        {
            return RedirectToAction("Index", "Dashboard");
        }

        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Login(LoginDTO loginDto)
    {
        if (!ModelState.IsValid)
        {
            return View(loginDto);
        }

        var result = await _apiService.PostAsync<TokenDTO>("auth/login", loginDto);
        if (result == null)
        {
            ModelState.AddModelError(string.Empty, "Login failed. Check your credentials and make sure the API is running.");
            return View(loginDto);
        }

        HttpContext.Session.SetString("Token", result.AccessToken);
        HttpContext.Session.SetString("RefreshToken", result.RefreshToken);
        HttpContext.Session.SetString("Role", result.Role);
        HttpContext.Session.SetString("FullName", result.FullName);
        HttpContext.Session.SetInt32("UserId", result.UserId);

        _logger.LogInformation("User logged in: {Email}", loginDto.Email);
        return RedirectToAction("Index", "Dashboard");
    }

    [HttpGet]
    public IActionResult Register()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Register(RegisterDTO registerDto)
    {
        if (!ModelState.IsValid)
        {
            return View(registerDto);
        }

        var result = await _apiService.PostAsync<TokenDTO>("auth/register", registerDto);
        if (result == null)
        {
            ModelState.AddModelError(string.Empty, "Registration failed. Check the API connection or whether the email already exists.");
            return View(registerDto);
        }

        HttpContext.Session.SetString("Token", result.AccessToken);
        HttpContext.Session.SetString("RefreshToken", result.RefreshToken);
        HttpContext.Session.SetString("Role", result.Role);
        HttpContext.Session.SetString("FullName", result.FullName);
        HttpContext.Session.SetInt32("UserId", result.UserId);

        _logger.LogInformation("User registered: {Email}, Role: {Role}", registerDto.Email, registerDto.Role);
        return RedirectToAction("Index", "Dashboard");
    }

    public IActionResult Logout()
    {
        HttpContext.Session.Clear();
        _logger.LogInformation("User logged out");
        return RedirectToAction(nameof(Login));
    }
}