using CommunityToolkit.Mvvm.ComponentModel;
using System;

namespace OllamaHub.Support.Local.Models;

public partial class ModelItem : ObservableObject
{
    [ObservableProperty]
    private string _name;
    [ObservableProperty]
    private string _size;
    [ObservableProperty]
    private string _lastUsed;
    [ObservableProperty]
    private string _status;
}

public class UserMessage
{
    public string Content { get; set; }
    public DateTime Timestamp { get; set; }
}

public class AIMessage
{
    public string Content { get; set; }
    public DateTime Timestamp { get; set; }
    public string ModelName { get; set; }
    public bool IsCodeBlock { get; set; }
    public string CodeLanguage { get; set; }
}