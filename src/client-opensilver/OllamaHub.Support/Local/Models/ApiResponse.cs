namespace OllamaHub.Support.Local.Models;

public class ApiResponse<T>
{
    public string Message { get; set; } = "";
    public int Count { get; set; }
    public T Models { get; set; } = default!;
    public bool Success { get; set; } = true;
}