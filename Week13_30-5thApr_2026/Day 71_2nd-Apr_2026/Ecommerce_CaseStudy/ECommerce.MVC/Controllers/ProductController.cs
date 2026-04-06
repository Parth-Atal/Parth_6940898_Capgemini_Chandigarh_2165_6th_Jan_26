using Microsoft.AspNetCore.Mvc;

public class ProductController : Controller
{
    private readonly IHttpClientFactory _factory;

    public ProductController(IHttpClientFactory factory)
    {
        _factory = factory;
    }

    private void SetAuthHeader(System.Net.Http.HttpClient client)
    {
        var token = HttpContext.Session.GetString("AccessToken");
        if (!string.IsNullOrEmpty(token))
            client.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
    }

    public async Task<IActionResult> Index()
    {
        var client = _factory.CreateClient("API");
        SetAuthHeader(client);

        var response = await client.GetAsync("api/products");
        if (!response.IsSuccessStatusCode)
        {
            ViewBag.Error = $"Error fetching products: {response.StatusCode}";
            return View(new List<Product>());
        }

        var products = await response.Content.ReadFromJsonAsync<List<Product>>();
        return View(products ?? new List<Product>());
    }

    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Create(Product product)
    {
        if (!ModelState.IsValid)
            return View(product);

        var client = _factory.CreateClient("API");
        SetAuthHeader(client);

        var response = await client.PostAsJsonAsync("api/products", product);

        if (response.IsSuccessStatusCode)
            return RedirectToAction("Index");

        ViewBag.Error = "Unable to create product. Ensure you're an Admin and authenticated.";
        return View(product);
    }
}
