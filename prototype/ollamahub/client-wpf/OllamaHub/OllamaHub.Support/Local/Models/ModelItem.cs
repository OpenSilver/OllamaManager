using Jamesnet.Foundation;

namespace OllamaHub.Support.Local.Models;

public partial class ModelItem : ViewModelBase
{
    private string _name;
    private string _size;
    private string _lastUsed;
    private string _status;

    public string Name
    {
        get => _name;
        set => SetProperty(ref _name, value);
    }

    public string Size
    {
        get => _size;
        set => SetProperty(ref _size, value);
    }

    public string LastUsed
    {
        get => _lastUsed;
        set => SetProperty(ref _lastUsed, value);
    }

    public string Status
    {
        get => _status;
        set => SetProperty(ref _status, value);
    }
}
