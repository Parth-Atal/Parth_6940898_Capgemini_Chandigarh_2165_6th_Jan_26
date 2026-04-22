using SmartRetail.MVC.Models;
using System.Text;
using System.Text.Json;

namespace SmartRetail.MVC.Services
{
    public class ProductService
    {
        private readonly HttpClient _httpClient;
        private readonly string _baseUrl;

        public ProductService(HttpClient httpClient, IConfiguration config)
        {
            _httpClient = httpClient;
            _baseUrl = config["ApiSettings:BaseUrl"]!;
        }

        public async Task<List<Product>> GetAllAsync()
        {
            var response = await _httpClient.GetAsync($"{_baseUrl}/api/products");
            response.EnsureSuccessStatusCode();
            var json = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<List<Product>>(json,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new();
        }

        public async Task<Product?> GetByIdAsync(int id)
        {
            var response = await _httpClient.GetAsync($"{_baseUrl}/api/products/{id}");
            if (!response.IsSuccessStatusCode) return null;
            var json = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<Product>(json,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        }

        public async Task CreateAsync(Product product)
        {
            var json = JsonSerializer.Serialize(product);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync($"{_baseUrl}/api/products", content);
            response.EnsureSuccessStatusCode();
        }

        public async Task UpdateAsync(Product product)
        {
            var json = JsonSerializer.Serialize(product);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await _httpClient.PutAsync($"{_baseUrl}/api/products/{product.Id}", content);
            response.EnsureSuccessStatusCode();
        }

        public async Task DeleteAsync(int id)
        {
            var response = await _httpClient.DeleteAsync($"{_baseUrl}/api/products/{id}");
            response.EnsureSuccessStatusCode();
        }
    }
}