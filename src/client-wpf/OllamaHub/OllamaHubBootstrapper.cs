using Jamesnet.Foundation;
using OllamaHub.Main.Local.ViewModels;
using OllamaHub.Main.UI.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OllamaHub
{
    internal class OllamaHubBootstrapper : AppBootstrapper
    {
        protected override void RegisterDependencies(IContainer container)
        {
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
