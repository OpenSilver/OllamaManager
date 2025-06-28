using System.Windows;
using System.Windows.Controls;

namespace OllamaHub.Support.UI.Units;

public class ModelStatusBadge : Control
{
    public ModelStatusBadge()
    {
        DefaultStyleKey = typeof(ModelStatusBadge);
    }

    public static readonly DependencyProperty CornerRadiusProperty =
        DependencyProperty.Register(
            nameof(CornerRadius),
            typeof(CornerRadius), 
            typeof(ModelStatusBadge), 
            new PropertyMetadata(new CornerRadius(0)));

    public static readonly DependencyProperty StatusProperty =
        DependencyProperty.Register(
            nameof(Status),
            typeof(ModelStatus),
            typeof(ModelStatusBadge),
            new PropertyMetadata(ModelStatus.Stopped, OnStatusChanged));

    public CornerRadius CornerRadius
    {
        get { return (CornerRadius)GetValue(CornerRadiusProperty); }
        set { SetValue(CornerRadiusProperty, value); }
    }

    public ModelStatus Status
    {
        get => (ModelStatus)GetValue(StatusProperty);
        set => SetValue(StatusProperty, value);
    }

    private static void OnStatusChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is ModelStatusBadge badge)
        {
            badge.UpdateVisualState();
        }
    }

    private void UpdateVisualState()
    {
        var state = Status.ToString();
        VisualStateManager.GoToState(this, state, true);
    }

    public override void OnApplyTemplate()
    {
        base.OnApplyTemplate();
        UpdateVisualState(); 
    }
}
