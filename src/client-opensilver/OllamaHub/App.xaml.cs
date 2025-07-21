using System.Windows;

namespace OllamaHub
{
    public sealed partial class App : Application
    {
        public App()
        {
            this.InitializeComponent();

            OllamaHubBootstrapper bootstrapper = new OllamaHubBootstrapper();
            bootstrapper.Run();

            var mainPage = new MainPage();
            Window.Current.Content = mainPage;
        }
    }
}
