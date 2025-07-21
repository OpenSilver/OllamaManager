using CommunityToolkit.Mvvm.ComponentModel;

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
