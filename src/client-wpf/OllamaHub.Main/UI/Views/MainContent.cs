using Jamesnet.Foundation;
using System.Windows;
using System.Windows.Controls;

namespace OllamaHub.Main.UI.Views;

public class MainContent : ContentControl, IView
{
    static MainContent()
    {
        DefaultStyleKeyProperty.OverrideMetadata(typeof(MainContent), new FrameworkPropertyMetadata(typeof(MainContent)));
    }
}
