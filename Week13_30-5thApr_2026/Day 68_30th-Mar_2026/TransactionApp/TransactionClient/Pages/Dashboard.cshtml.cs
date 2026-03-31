using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Net.Http.Headers;
using System.Text.Json;
using TransactionClient.DTO;

public class DashboardModel : PageModel
{
    private readonly IHttpClientFactory _clientFactory;

    public DashboardModel(IHttpClientFactory clientFactory)
    {
        _clientFactory = clientFactory;
    }

    public List<TransactionDTO> Transactions { get; set; } = new();

    public async Task<IActionResult> OnGetAsync()
    {
        var token = HttpContext.Session.GetString("JWT");

        if (string.IsNullOrEmpty(token))
            return RedirectToPage("/Login");

        var client = _clientFactory.CreateClient();
        client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", token);

        var response = await client.GetAsync("http://localhost:5001/api/transactions");

        if (!response.IsSuccessStatusCode)
            return RedirectToPage("/Login");

        var data = await response.Content.ReadAsStringAsync();

        Transactions = JsonSerializer.Deserialize<List<TransactionDTO>>(data,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        return Page();
    }

    public IActionResult OnPostLogout()
    {
        HttpContext.Session.Clear();
        return RedirectToPage("/Login");
    }
}