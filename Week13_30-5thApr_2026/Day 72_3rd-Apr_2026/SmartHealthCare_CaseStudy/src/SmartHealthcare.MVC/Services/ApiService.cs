using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Http;

namespace SmartHealthcare.MVC.Services;

public class ApiService : IApiService
{
	private readonly HttpClient _httpClient;
	private readonly ILogger<ApiService> _logger;
	private readonly IHttpContextAccessor _httpContextAccessor;
	private readonly JsonSerializerOptions _jsonOptions;

	public ApiService(HttpClient httpClient, ILogger<ApiService> logger, IHttpContextAccessor httpContextAccessor)
	{
		_httpClient = httpClient;
		_logger = logger;
		_httpContextAccessor = httpContextAccessor;
		_jsonOptions = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
	}

	private void AttachToken(string? token)
	{
		var resolvedToken = token ?? _httpContextAccessor.HttpContext?.Session.GetString("Token") ?? _httpContextAccessor.HttpContext?.Session.GetString("JWTToken");
		if (!string.IsNullOrEmpty(resolvedToken))
		{
			_httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", resolvedToken);
		}
		else
		{
			_httpClient.DefaultRequestHeaders.Authorization = null;
		}
	}

	public async Task<T?> GetAsync<T>(string endpoint, string? token = null)
	{
		try
		{
			AttachToken(token);
			var response = await _httpClient.GetAsync(endpoint);
			if (!response.IsSuccessStatusCode)
			{
				_logger.LogWarning("GET {Endpoint} returned {StatusCode}", endpoint, response.StatusCode);
				return default;
			}

			var content = await response.Content.ReadAsStringAsync();
			return JsonSerializer.Deserialize<T>(content, _jsonOptions);
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "Error calling GET {Endpoint}", endpoint);
			return default;
		}
	}

	public async Task<T?> PostAsync<T>(string endpoint, object data, string? token = null)
	{
		try
		{
			AttachToken(token);
			var json = JsonSerializer.Serialize(data, _jsonOptions);
			var content = new StringContent(json, Encoding.UTF8, "application/json");
			var response = await _httpClient.PostAsync(endpoint, content);
			var responseContent = await response.Content.ReadAsStringAsync();

			if (response.IsSuccessStatusCode)
			{
				return JsonSerializer.Deserialize<T>(responseContent, _jsonOptions);
			}

			_logger.LogWarning("POST {Endpoint} returned {StatusCode}: {Response}", endpoint, response.StatusCode, responseContent);
			return default;
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "Error calling POST {Endpoint}", endpoint);
			return default;
		}
	}

	public async Task<T?> PutAsync<T>(string endpoint, object data, string? token = null)
	{
		try
		{
			AttachToken(token);
			var json = JsonSerializer.Serialize(data, _jsonOptions);
			var content = new StringContent(json, Encoding.UTF8, "application/json");
			var response = await _httpClient.PutAsync(endpoint, content);
			if (!response.IsSuccessStatusCode)
			{
				return default;
			}

			var responseContent = await response.Content.ReadAsStringAsync();
			return JsonSerializer.Deserialize<T>(responseContent, _jsonOptions);
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "Error calling PUT {Endpoint}", endpoint);
			return default;
		}
	}

	public async Task<T?> PatchAsync<T>(string endpoint, object patchData, string? token = null)
	{
		try
		{
			AttachToken(token);
			var json = JsonSerializer.Serialize(patchData, _jsonOptions);
			var content = new StringContent(json, Encoding.UTF8, "application/json");
			var request = new HttpRequestMessage(HttpMethod.Patch, endpoint) { Content = content };
			var response = await _httpClient.SendAsync(request);
			if (!response.IsSuccessStatusCode)
			{
				return default;
			}

			var responseContent = await response.Content.ReadAsStringAsync();
			return JsonSerializer.Deserialize<T>(responseContent, _jsonOptions);
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "Error calling PATCH {Endpoint}", endpoint);
			return default;
		}
	}

	public async Task<bool> DeleteAsync(string endpoint, string? token = null)
	{
		try
		{
			AttachToken(token);
			var response = await _httpClient.DeleteAsync(endpoint);
			return response.IsSuccessStatusCode;
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "Error calling DELETE {Endpoint}", endpoint);
			return false;
		}
	}
}
