using Jamesnet.Foundation;
using Microsoft.AspNetCore.SignalR.Client;
using OllamaHub.Main.Local.ViewModels;
using OllamaHub.Main.UI.Views;
using OllamaHub.Support.Local.Services;
using System;

namespace OllamaHub
{
    internal class OllamaHubBootstrapper : AppBootstrapper
    {
        protected override void RegisterDependencies(IContainer container)
        {
            var hubConnection = new HubConnectionBuilder()
                .WithUrl("https://localhost:7262/modelhub")
                .WithAutomaticReconnect(new[] { TimeSpan.Zero, TimeSpan.FromSeconds(2), TimeSpan.FromSeconds(10) })
                .Build();

            container.RegisterInstance(hubConnection);

            var apiClient = new ApiClient("https://localhost:7262/api/");
            container.RegisterInstance(apiClient);

            container.RegisterSingleton<IView, MainContent>(nameof(MainContent));
        }

        protected override void RegisterViewModels(IViewModelMapper viewModelMapper)
        {
            viewModelMapper.Register<MainContent, MainViewModel>();
        }

        protected override void SettingsLayer(ILayerManager layer, IContainer container)
        {
            layer.Mapping("MAIN", container.Resolve<IView>(nameof(MainContent)));
        }
    }
}