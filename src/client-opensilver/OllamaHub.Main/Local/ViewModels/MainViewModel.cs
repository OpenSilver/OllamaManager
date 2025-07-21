using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Linq;
using System.Windows.Input;
using OllamaHub.Support.Local.Services;
using Microsoft.AspNetCore.SignalR.Client;
using OllamaHub.Support.Local.Models;
using Jamesnet.Foundation;

namespace OllamaHub.Main.Local.ViewModels;

public class MainViewModel : ViewModelBase, IDisposable
{
    private readonly ApiClient _apiClient;
    private readonly HubConnection _hubConnection;

    private ObservableCollection<ModelItem> _models;
    private ObservableCollection<ModelItem> _runningModels;
    private bool _isLoading;
    private ObservableCollection<object> _chatMessages;
    private string _inputText;
    private bool _isChatLoading;
    private ModelItem _currentModel;

    public ObservableCollection<ModelItem> Models
    {
        get => _models;
        set => SetProperty(ref _models, value);
    }

    public ObservableCollection<ModelItem> RunningModels
    {
        get => _runningModels;
        set => SetProperty(ref _runningModels, value);
    }

    public bool IsLoading
    {
        get => _isLoading;
        set => SetProperty(ref _isLoading, value);
    }

    public ObservableCollection<object> ChatMessages
    {
        get => _chatMessages;
        set => SetProperty(ref _chatMessages, value);
    }

    public string InputText
    {
        get => _inputText;
        set => SetProperty(ref _inputText, value);
    }

    public bool IsChatLoading
    {
        get => _isChatLoading;
        set => SetProperty(ref _isChatLoading, value);
    }

    public ModelItem CurrentModel
    {
        get => _currentModel;
        set => SetProperty(ref _currentModel, value);
    }

    public ICommand LoadModelsCommand { get; set; }
    public ICommand SendMessageCommand { get; set; }
    public ICommand ToggleModelCommand { get; set; }

    public MainViewModel(
        ApiClient apiClient,
        HubConnection hubConnection)
    {
        Models = [];
        RunningModels = [];
        ChatMessages = [];
        InputText = "";
        IsLoading = false;
        IsChatLoading = false;
        CurrentModel = null;

        _apiClient = apiClient;
        _hubConnection = hubConnection;

        LoadModelsCommand = new RelayCommand(async () => await LoadModelsAsync());
        SendMessageCommand = new RelayCommand(async () => await SendMessageAsync());
        ToggleModelCommand = new RelayCommand<ModelItem>(async (model) => await ToggleModelAsync(model));

        _ = InitializeAsync();
    }

    private async Task InitializeAsync()
    {
        await LoadModelsAsync();
        await ConnectToSignalRAsync();
    }

    private async Task LoadModelsAsync()
    {
        IsLoading = true;
        try
        {
            var modelList = await _apiClient.GetAsync<ModelItem>("models");
            Models.Clear();
            foreach (var model in modelList)
            {
                Models.Add(model);
            }
            UpdateRunningModels();
            CurrentModel = RunningModels.FirstOrDefault();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Load models error: {ex.Message}");
        }
        finally
        {
            IsLoading = false;
        }
    }

    private async Task SendMessageAsync()
    {
        if (string.IsNullOrWhiteSpace(InputText) || IsChatLoading) return;

        var userMessage = InputText.Trim();
        InputText = "";

        ChatMessages.Add(new UserMessage
        {
            Content = userMessage,
            Timestamp = DateTime.Now,
        });

        IsChatLoading = true;
        try
        {
            await _apiClient.PostAsync("chat", new
            {
                message = userMessage,
                model = CurrentModel?.Name
            });
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Send message error: {ex.Message}");
            IsChatLoading = false;
        }
    }

    private async Task ConnectToSignalRAsync()
    {
        try
        {
            _hubConnection.On<string, string>("ModelStatusChanged", (modelName, status) =>
            {
                var model = Models.FirstOrDefault(m => m.Name == modelName);
                if (model != null)
                {
                    model.Status = status;
                    UpdateRunningModels();
                    if (CurrentModel?.Status != "Running")
                    {
                        CurrentModel = RunningModels.FirstOrDefault();
                    }
                }
            });

            _hubConnection.On<string>("ChatMessageReceived", (message) =>
            {
                if (!string.IsNullOrEmpty(message))
                {
                    ChatMessages.Add(new AIMessage
                    {
                        Content = message,
                        Timestamp = DateTime.Now
                    });
                }
                IsChatLoading = false;
            });

            _hubConnection.Closed += async (error) =>
            {
                await Task.Delay(new Random().Next(0, 5) * 1000);
                await _hubConnection.StartAsync();
            };

            _hubConnection.Reconnecting += (error) => Task.CompletedTask;
            _hubConnection.Reconnected += (connectionId) => Task.CompletedTask;

            await _hubConnection.StartAsync();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"SignalR connection error: {ex.Message}");
        }
    }

    private async Task ToggleModelAsync(ModelItem model)
    {
        if (model == null) return;

        var action = model.Status == "Running" ? "stop" : "start";
        model.Status = model.Status == "Running" ? "Stopping" : "Starting";
        try
        {
            await _apiClient.PostAsync($"models/{model.Name}/{action}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Toggle model error: {ex.Message}");
        }
    }

    private void UpdateRunningModels()
    {
        var running = Models.Where(m => m.Status == "Running").ToList();
        RunningModels.Clear();
        foreach (var model in running)
        {
            RunningModels.Add(model);
        }
    }

    public async Task DisconnectAsync()
    {
        if (_hubConnection != null)
        {
            await _hubConnection.DisposeAsync();
        }
    }

    public void Dispose()
    {
        _apiClient.Dispose();
        DisconnectAsync().GetAwaiter().GetResult();
    }
}