using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using OllamaHub.Support.Local.Models;
using OllamaHub.Support.Local.Services;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace OllamaHub
{
    public partial class MainViewModel2 : ObservableObject
    {
        private readonly ApiClient<ModelItem> _apiClient;

        [ObservableProperty]
        private List<ModelItem> models;

        public MainViewModel2()
        {
            _apiClient = new();
            InitList();
        }

        private async void InitList()
        {
            var modelList = await _apiClient.GetAsync("https://localhost:7070/api/models");
            Models = modelList;
        }

        [RelayCommand]
        private async Task StartAsync(ModelItem model)
        {
            await _apiClient.PostAsync($"https://localhost:7070/api/models/{model.Name}/start");
        }
    }
}
