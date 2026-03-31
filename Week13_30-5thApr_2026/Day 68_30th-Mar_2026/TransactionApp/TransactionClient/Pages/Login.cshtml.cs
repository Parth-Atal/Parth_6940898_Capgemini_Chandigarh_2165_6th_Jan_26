using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Text;
using System.Text.Json;

public class LoginModel : PageModel
{
    private readonly IHttpClientFactory _clientFactory;

    public LoginModel(IHttpClientFactory clientFactory)
    {
        _clientFactory = clientFactory;
    }

    [BindProperty]
    public string Username { get; set; }

    [BindProperty]
    public string Password { get; set; }

    public string ErrorMessage { get; set; }

    public async Task<IActionResult> OnPostAsync()
    {
        var client = _clientFactory.CreateClient();

        var content = new StringContent(
            JsonSerializer.Serialize(new { Username, Password }),
            Encoding.UTF8,
            "application/json"
        );

        var response = await client.PostAsync("http://localhost:5001/api/auth/login", content);

        if (!response.IsSuccessStatusCode)
        {
            ErrorMessage = "Invalid Credentials";
            return Page();
        }

        var result = await response.Content.ReadAsStringAsync();
        var json = JsonDocument.Parse(result);

        var token = json.RootElement.GetProperty("token").GetString();

        HttpContext.Session.SetString("JWT", token);

        return RedirectToPage("/Dashboard");
    }
}