using Microsoft.AspNetCore.Mvc;

public class AuthController : Controller
{
    private readonly IHttpClientFactory _factory;

    public AuthController(IHttpClientFactory factory)
    {
        _factory = factory;
    }

    public IActionResult Login()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Login(LoginViewModel model)
    {
        var client = _factory.CreateClient("API");

        var response = await client.PostAsJsonAsync("api/auth/login", new
        {
            username = model.Username,
            password = model.Password
        });

        if (response.IsSuccessStatusCode)
        {
            var result = await response.Content.ReadFromJsonAsync<TokenResponse>();
            HttpContext.Session.SetString("AccessToken", result!.AccessToken!);
            HttpContext.Session.SetString("RefreshToken", result!.RefreshToken!);
            return RedirectToAction("Index", "Order");
        }

        ViewBag.Error = "Invalid username or password";
        return View();
    }

    public IActionResult Logout()
    {
        HttpContext.Session.Clear();
        return RedirectToAction("Login");
    }
}