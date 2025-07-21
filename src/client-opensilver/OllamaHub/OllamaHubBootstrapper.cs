using Jamesnet.Foundation;
using Microsoft.AspNetCore.SignalR.Client;
using OllamaHub.Main.Local.ViewModels;
using OllamaHub.Main.UI.Views;
using OllamaHub.Support.Local.Services;

namespace OllamaHub
{
    internal class OllamaHubBootstrapper : AppBootstrapper
    {
        protected override void RegisterDependencies(IContainer container)
        {
            HubConnection _hubConnection = new HubConnectionBuilder()
                .WithUrl("https://localhost:7262/modelhub")
                .WithAutomaticReconnect()
                .Build();

            container.RegisterInstance(_hubConnection);
            container.RegisterSingleton<ApiClient, ApiClient>();
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
