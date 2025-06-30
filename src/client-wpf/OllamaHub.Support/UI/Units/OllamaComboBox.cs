using System.Windows;
using System.Windows.Controls;

namespace OllamaHub.Support.UI.Units;

public class OllamaComboBox : ComboBox
{
    public OllamaComboBox()
    {
        DefaultStyleKey = typeof(OllamaComboBox);
    }

    protected override DependencyObject GetContainerForItemOverride()
    {
        return new OllamaComboBoxItem();
    }
}
