using System.Windows;

namespace OllamaHub
{
    public partial class App : Application
    {
        public App()
        {
            OllamaHubBootstrapper bootstrapper = new OllamaHubBootstrapper();
            bootstrapper.Run();
        }
    }
}
