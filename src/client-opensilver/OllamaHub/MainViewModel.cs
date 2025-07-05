using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using OllamaHub.Support.Local.Services;
using Microsoft.AspNetCore.SignalR.Client;
using OllamaHub.Support.UI.Units;
using OllamaHub.Support.Local.Models;

namespace OllamaHub;

public partial class MainViewModel : ObservableObject
{
    private readonly ApiClient<ModelItem> _apiClient;
    private HubConnection? _hubConnection;

    [ObservableProperty]
    private ObservableCollection<ModelItem> models = new();

    [ObservableProperty]
    private ObservableCollection<ModelItem> runningModels = new();

    [ObservableProperty]
    private bool isLoading = false;

    [ObservableProperty]
    private ObservableCollection<object> chatMessages = new();

    [ObservableProperty]
    private string inputText = "";

    [ObservableProperty]
    private bool isChatLoading = false;

    [ObservableProperty]
    private ModelItem currentModel;

    public MainViewModel()
    {
        _apiClient = new ApiClient<ModelItem>();
        _ = InitializeAsync();
    }

    private async Task InitializeAsync()
    {
        await LoadModelsAsync();
        await ConnectToSignalRAsync();
    }

    [RelayCommand]
    private async Task LoadModelsAsync()
    {
        IsLoading = true;

        var modelList = await _apiClient.GetAsync("https://localhost:7262/api/models");

        Models = new ObservableCollection<ModelItem>(modelList);
        RunningModels = new ObservableCollection<ModelItem>(modelList.Where(m => m.Status == "Running"));
        CurrentModel = RunningModels.FirstOrDefault();

        IsLoading = false;
    }

    [RelayCommand]
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
        await _apiClient.PostAsync("https://localhost:7262/api/chat", new
        {
            message = userMessage,
            model = CurrentModel?.Name
        });
    }

    private async Task ConnectToSignalRAsync()
    {
        _hubConnection = new HubConnectionBuilder()
            .WithUrl("https://localhost:7262/modelhub")
            .WithAutomaticReconnect()
            .Build();

        _hubConnection.On<string, string>("ModelStatusChanged", (modelName, status) =>
        {
            var model = Models.FirstOrDefault(m => m.Name == modelName);
            if (model != null)
            {
                model.Status = status;

                // RunningModels 업데이트
                RunningModels = new ObservableCollection<ModelItem>(Models.Where(m => m.Status == "Running"));

                // CurrentModel이 실행중이 아니면 새로 설정
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

    private bool CanToggleModel(ModelItem model)
    {
        return model != null && (model.Status == "Stopped" || model.Status == "Running");
    }

    [RelayCommand(CanExecute = nameof(CanToggleModel))]
    private async Task ToggleModelAsync(ModelItem model)
    {
        if (model == null) return;

        var action = model.Status == "Running" ? "stop" : "start";
        model.Status = model.Status == "Running" ? "Stopping" : "Starting";
        await _apiClient.PostAsync($"https://localhost:7262/api/models/{model.Name}/{action}");
    }

    public async Task DisconnectAsync()
    {
        if (_hubConnection != null)
        {
            await _hubConnection.DisposeAsync();
        }
    }
}