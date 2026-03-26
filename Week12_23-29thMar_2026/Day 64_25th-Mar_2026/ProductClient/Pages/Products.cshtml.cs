using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Text.Json;

namespace ProductClient.Pages
{
    public class ProductsModel : PageModel
    {
        private readonly IHttpClientFactory _clientFactory;

        public ProductsModel(IHttpClientFactory clientFactory)
        {
            _clientFactory = clientFactory;
        }

        public List<Product> Products { get; set; } = new();

        public async Task OnGetAsync()
        {
            var client = _clientFactory.CreateClient("MyWebApi");

            var response = await client.GetAsync("products");

            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();

                Products = JsonSerializer.Deserialize<List<Product>>(json,
                    new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    }) ?? new();
            }
        }
    }

    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
    }
}