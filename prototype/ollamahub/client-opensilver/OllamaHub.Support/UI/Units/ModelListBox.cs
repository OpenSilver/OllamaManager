using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

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