namespace LocalLLMServer.Endpoint.Dto;

public class ChatRequest
{
    public string Message { get; set; } = "";
    public string Model { get; set; } = "";
}
