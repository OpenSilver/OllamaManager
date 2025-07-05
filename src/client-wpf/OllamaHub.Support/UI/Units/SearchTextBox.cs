using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace OllamaHub.Support.UI.Units;

public class SearchTextBox : TextBox
{
    public static readonly DependencyProperty EnterCommandProperty =
        DependencyProperty.Register (
            "EnterCommand",
            typeof (ICommand),
            typeof (SearchTextBox),
            new PropertyMetadata (null));

    public ICommand EnterCommand
    {
        get => (ICommand)GetValue (EnterCommandProperty);
        set => SetValue (EnterCommandProperty, value);
    }

    protected override void OnPreviewKeyDown(KeyEventArgs e)
    {
        base.OnPreviewKeyDown (e);

        if (e.Key == Key.Enter && Keyboard.Modifiers == ModifierKeys.None)
        {
            if (EnterCommand?.CanExecute (null) == true)
            {
                EnterCommand.Execute (null);
                e.Handled = true; // 기본 줄바꿈 막기
            }
        }
    }
    protected override void OnPreviewMouseDown(MouseButtonEventArgs e)
    {
        base.OnPreviewMouseDown (e);

        if(e.LeftButton == MouseButtonState.Pressed)
        {
            this.Focus ();
        }
    }

    public SearchTextBox()
    {
        DefaultStyleKey = typeof(SearchTextBox);
    }
}
