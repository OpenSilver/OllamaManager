using System.Windows;
using System.Windows.Controls;

namespace OllamaHub.Support.UI.Units;

public class ModelListBox : ListBox
{
    public ModelListBox()
    {
        DefaultStyleKey = typeof(ModelListBox);
    }

    protected override DependencyObject GetContainerForItemOverride()
    {
        return new ModelListBoxItem();
    }
}