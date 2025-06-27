using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace OllamaHub.Support.Local.Services;

public class ApiClient<T>
{
    private readonly HttpClient _http;

    public ApiClient()
    {
        _http = new();
        _http.Timeout = TimeSpan.FromMinutes(5);
    }

    public async Task<List<T>> GetAsync(string url)
    {
        var json = await _http.GetStringAsync(url);
        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };
        var response = JsonSerializer.Deserialize<ApiResponse<List<T>>>(json, options);
        return response?.Models ?? new List<T>();
    }

    public async Task<string> PostAsync(string url)
    {
        var response = await _http.PostAsync(url, null);
        return await response.Content.ReadAsStringAsync();
    }

    public async Task<string> PostAsync(string url, object data)
    {
        var json = JsonSerializer.Serialize(data);
        var content = new StringContent(json, Encoding.UTF8, "application/json");
        var response = await _http.PostAsync(url, content);
        return await response.Content.ReadAsStringAsync();
    }

    public void Dispose()
    {
        _http?.Dispose();
    }
}

public class ApiResponse<T>
{
    public string Message { get; set; } = "";
    public int Count { get; set; }
    public T Models { get; set; } = default!;
    public bool Success { get; set; } = true;
}