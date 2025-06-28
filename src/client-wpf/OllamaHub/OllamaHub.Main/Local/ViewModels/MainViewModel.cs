using Jamesnet.Foundation;
using Microsoft.AspNetCore.SignalR.Client;
using OllamaHub.Support.Local.Models;
using OllamaHub.Support.Local.Services;
using System.Collections.ObjectModel;
using System.Windows.Input;
using System.Linq;
using System.Windows;

namespace OllamaHub.Main.Local.ViewModels;

public partial class MainViewModel : ViewModelBase
{
    private readonly ApiClient<ModelItem> _apiClient;
    private HubConnection? _hubConnection;

    private ObservableCollection<ModelItem> models = new();
    private ObservableCollection<ModelItem> runningModels = new();
    private bool isLoading = false;
    private ObservableCollection<object> chatMessages = new();
    private string inputText = "";
    private bool isChatLoading = false;
    private ModelItem currentModel;

    public ObservableCollection<ModelItem> Models
    {
        get => models;
        set => SetProperty(ref models, value);
    }

    public ObservableCollection<ModelItem> RunningModels
    {
        get => runningModels;
        set => SetProperty(ref runningModels, value);
    }

    public bool IsLoading
    {
        get => isLoading;
        set => SetProperty(ref isLoading, value);
    }

    public ObservableCollection<object> ChatMessages
    {
        get => chatMessages;
        set => SetProperty(ref chatMessages, value);
    }

    public string InputText
    {
        get => inputText;
        set => SetProperty(ref inputText, value);
    }

    public bool IsChatLoading
    {
        get => isChatLoading;
        set => SetProperty(ref isChatLoading, value);
    }

    public ModelItem CurrentModel
    {
        get => currentModel;
        set => SetProperty(ref currentModel, value);
    }

    // ICommand 프로퍼티들
    public ICommand LoadModelsCommand { get; }
    public ICommand SendMessageCommand { get; }
    public ICommand ToggleModelCommand { get; }

    public MainViewModel()
    {
        _apiClient = new ApiClient<ModelItem>();

        // Command 초기화
        LoadModelsCommand = new RelayCommand(async () => await LoadModelsAsync());
        SendMessageCommand = new RelayCommand(async () => await SendMessageAsync());
        ToggleModelCommand = new RelayCommand<ModelItem>(OnPlayStop);

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

        var modelList = await _apiClient.GetAsync("https://localhost:7262/api/models");

        Models = new ObservableCollection<ModelItem>(modelList);
        RunningModels = new ObservableCollection<ModelItem>(modelList.Where(m => m.Status == "Running"));
        CurrentModel = RunningModels.FirstOrDefault();

        IsLoading = false;
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
                Application.Current.Dispatcher.BeginInvoke(() =>
                {
                    ChatMessages.Add(new AIMessage
                    {
                        Content = message,
                        Timestamp = DateTime.Now
                    });
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

    private async void OnPlayStop(ModelItem model)
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