using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using BookStore.Shared;

namespace BookStore.Web.Services;

public class ApiService
{
    private readonly HttpClient _http;
    private readonly IHttpContextAccessor _accessor;
    private static readonly JsonSerializerOptions JsonOpts = new() { PropertyNameCaseInsensitive = true };

    public ApiService(HttpClient http, IHttpContextAccessor accessor) { _http = http; _accessor = accessor; }

    private void AttachToken()
    {
        var token = _accessor.HttpContext?.Session.GetString("JwtToken");
        if (!string.IsNullOrEmpty(token))
            _http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
    }

    public async Task<T?> GetAsync<T>(string url)
    {
        AttachToken();
        var response = await _http.GetAsync(url);
        if (!response.IsSuccessStatusCode) return default;
        var json = await response.Content.ReadAsStringAsync();
        var wrapper = JsonSerializer.Deserialize<ApiResponse<T>>(json, JsonOpts);
        return wrapper is { Success: true } ? wrapper.Data : default;
    }

    public async Task<ApiResponse> PostAsync<T>(string url, T data)
    {
        AttachToken();
        var content = new StringContent(JsonSerializer.Serialize(data), Encoding.UTF8, "application/json");
        var response = await _http.PostAsync(url, content);
        var json = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<ApiResponse>(json, JsonOpts) ?? ApiResponse.Fail("Unexpected error.");
    }

    public async Task<TResponse?> PostWithResponseAsync<TRequest, TResponse>(string url, TRequest data)
    {
        AttachToken();
        var content = new StringContent(JsonSerializer.Serialize(data), Encoding.UTF8, "application/json");
        var response = await _http.PostAsync(url, content);
        var json = await response.Content.ReadAsStringAsync();
        var wrapper = JsonSerializer.Deserialize<ApiResponse<TResponse>>(json, JsonOpts);
        return wrapper is { Success: true } ? wrapper.Data : default;
    }

    public async Task<ApiResponse> PutAsync<T>(string url, T data)
    {
        AttachToken();
        var content = new StringContent(JsonSerializer.Serialize(data), Encoding.UTF8, "application/json");
        var response = await _http.PutAsync(url, content);
        var json = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<ApiResponse>(json, JsonOpts) ?? ApiResponse.Fail("Unexpected error.");
    }

    public async Task<ApiResponse> DeleteAsync(string url)
    {
        AttachToken();
        var response = await _http.DeleteAsync(url);
        var json = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<ApiResponse>(json, JsonOpts) ?? ApiResponse.Fail("Unexpected error.");
    }

    public async Task<ApiResponse> PatchAsync(string url, string body)
    {
        AttachToken();
        var content = new StringContent(JsonSerializer.Serialize(body), Encoding.UTF8, "application/json");
        var request = new HttpRequestMessage(HttpMethod.Patch, url) { Content = content };
        var response = await _http.SendAsync(request);
        var json = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<ApiResponse>(json, JsonOpts) ?? ApiResponse.Fail("Unexpected error.");
    }
}
