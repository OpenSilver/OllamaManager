using OllamaHub.Support.Local.Models;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace OllamaHub.Support.Local.Services;

public class ApiClient : IDisposable
{
    private readonly HttpClient _http;
    private readonly JsonSerializerOptions _jsonOptions;

    public ApiClient(string baseUrl)
    {
        _http = new HttpClient
        {
            BaseAddress = new Uri(baseUrl),
            Timeout = TimeSpan.FromMinutes(5)
        };

        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };
    }

    public async Task<List<T>> GetAsync<T>(string endpoint)
    {
        try
        {
            var response = await _http.GetAsync(endpoint);
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();
            var apiResponse = JsonSerializer.Deserialize<ApiResponse<List<T>>>(json, _jsonOptions);
            return apiResponse?.Models ?? new List<T>();
        }
        catch (HttpRequestException ex)
        {
            Console.WriteLine($"API GET 오류: {ex.Message}");
            return new List<T>();
        }
    }

    public async Task<string> PostAsync(string endpoint, object data = null)
    {
        try
        {
            HttpContent content = null;
            if (data != null)
            {
                var json = JsonSerializer.Serialize(data);
                content = new StringContent(json, Encoding.UTF8, "application/json");
            }

            var response = await _http.PostAsync(endpoint, content);
            response.EnsureSuccessStatusCode();

            return await response.Content.ReadAsStringAsync();
        }
        catch (HttpRequestException ex)
        {
            Console.WriteLine($"API POST 오류: {ex.Message}");
            return string.Empty;
        }
    }

    public void Dispose()
    {
        _http?.Dispose();
    }
}