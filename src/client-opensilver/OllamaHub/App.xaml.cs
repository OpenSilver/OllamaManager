using Jamesnet.Foundation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

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
