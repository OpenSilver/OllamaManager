using System.Windows;
using System.Windows.Controls;

namespace OllamaHub.Support.UI.Units;

public class GlobalStatusBadge : Control
{
    public GlobalStatusBadge()
    {
        DefaultStyleKey = typeof(GlobalStatusBadge);
    }

    public static readonly DependencyProperty CornerRadiusProperty =
        DependencyProperty.Register(
            "CornerRadius", 
            typeof(CornerRadius), 
            typeof(GlobalStatusBadge), 
            new PropertyMetadata(new CornerRadius(0)));



    public static readonly DependencyProperty StatusProperty =
        DependencyProperty.Register(
            nameof(Status),
            typeof(GlobalStatus),
            typeof(GlobalStatusBadge),
            new PropertyMetadata(GlobalStatus.NoModelsRunning, OnStatusChanged));

    public CornerRadius CornerRadius
    {
        get { return (CornerRadius)GetValue(CornerRadiusProperty); }
        set { SetValue(CornerRadiusProperty, value); }
    }

    public GlobalStatus Status
    {
        get => (GlobalStatus)GetValue(StatusProperty);
        set => SetValue(StatusProperty, value);
    }

    public static readonly DependencyProperty ModelNameProperty =
        DependencyProperty.Register(
            nameof(ModelName),
            typeof(string),
            typeof(GlobalStatusBadge),
            new PropertyMetadata(string.Empty, OnModelNameChanged));

    public string ModelName
    {
        get => (string)GetValue(ModelNameProperty);
        set => SetValue(ModelNameProperty, value);
    }

    public static readonly DependencyProperty RunningCountProperty =
        DependencyProperty.Register(
            nameof(RunningCount),
            typeof(int),
            typeof(GlobalStatusBadge),
            new PropertyMetadata(0, OnRunningCountChanged));

    public int RunningCount
    {
        get => (int)GetValue(RunningCountProperty);
        set => SetValue(RunningCountProperty, value);
    }

    private static void OnStatusChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is GlobalStatusBadge badge)
        {
            badge.UpdateVisualState();
        }
    }

    private static void OnModelNameChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is GlobalStatusBadge badge)
        {
            badge.UpdateVisualState();
        }
    }

    private static void OnRunningCountChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is GlobalStatusBadge badge)
        {
            badge.UpdateVisualState();
        }
    }

    private void UpdateVisualState()
    {
        var state = Status.ToString(); 
        VisualStateManager.GoToState(this, state, true);

        UpdateStatusText();
    }

    private void UpdateStatusText()
    {
        if (GetTemplateChild("StatusText") is TextBlock statusText)
        {
            switch (Status)
            {
                case GlobalStatus.NoModelsRunning:
                    statusText.Text = "No models running";
                    break;
                case GlobalStatus.SingleModelRunning:
                    statusText.Text = string.IsNullOrEmpty(ModelName) ? "Model running" : $"{ModelName} Running";
                    break;
                case GlobalStatus.MultipleModelsRunning:
                    statusText.Text = $"{RunningCount} models running";
                    break;
                case GlobalStatus.SystemLoading:
                    statusText.Text = "System loading";
                    break;
                case GlobalStatus.SystemError:
                    statusText.Text = "System error";
                    break;
                case GlobalStatus.SystemIdle:
                    statusText.Text = "System idle";
                    break;
                case GlobalStatus.AllModelsDownloading:
                    statusText.Text = "Downloading models";
                    break;
            }
        }
    }

    public override void OnApplyTemplate()
    {
        base.OnApplyTemplate();
        UpdateVisualState();
        UpdateStatusText(); 
    }
}
