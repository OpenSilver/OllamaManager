using System.Windows;
using System.Windows.Controls;

namespace OllamaHub.Support.UI.Units;

public enum ModelButtonState
{
    Stopped,
    Starting,
    Running,
    Stopping
}

public class PlayStopButton : Button
{
    public static readonly DependencyProperty ModelStatusProperty =
        DependencyProperty.Register(
            nameof(ModelStatus),
            typeof(string),
            typeof(PlayStopButton),
            new PropertyMetadata(string.Empty, OnModelStatusChanged));

    public string ModelStatus
    {
        get => (string)GetValue(ModelStatusProperty);
        set => SetValue(ModelStatusProperty, value);
    }

    public PlayStopButton()
    {
        DefaultStyleKey = typeof(PlayStopButton);
    }

    private static void OnModelStatusChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is PlayStopButton button)
        {
            button.UpdateVisualState((string)e.NewValue);
        }
    }

    public override void OnApplyTemplate()
    {
        base.OnApplyTemplate();
        UpdateVisualState(ModelStatus);
    }

    private void UpdateVisualState(string status)
    {
        var stateName = status switch
        {
            "Running" => "Running",
            "Starting" => "Starting",
            "Stopping" => "Stopping",
            _ => "Stopped"
        };

        VisualStateManager.GoToState(this, stateName, false);
    }
}
