using System;

namespace OllamaHub.Support.Local.Models;

public class AIMessage
{
    public string Content { get; set; }
    public DateTime Timestamp { get; set; }
    public string ModelName { get; set; }
    public bool IsCodeBlock { get; set; }
    public string CodeLanguage { get; set; }
}
