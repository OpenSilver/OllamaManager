using System.Windows;
using System.Windows.Controls;

namespace OllamaHub.Support.UI.Units;

public class NavigationListBox : ListBox
{
    public NavigationListBox()
    {
        DefaultStyleKey = typeof(NavigationListBox);
    }

    protected override DependencyObject GetContainerForItemOverride()
    {
        return new NavigationListBoxItem();
    }
}
