using OllamaHub.Support.Local.Models;
using System.Collections.Specialized;
using System.Windows;
using System.Windows.Controls;

namespace OllamaHub.Support.UI.Units;

public class ChatListBox : ListBox
{
    private int _currentItemIndex = -1; // 생성 중인 아이템 인덱스 추적

    public ChatListBox()
    { 
        DefaultStyleKey = typeof(ChatListBox);
        _currentItemIndex = -1;

        ListBoxAutoScrollBehavior.SetAutoScroll (this, true);
    }

    protected override DependencyObject GetContainerForItemOverride()
    {
        if (Items.Count > 0)
        {
            var currentItem = Items[Items.Count - 1];
            if (currentItem is UserMessage)
            {
                return new UserMessageListBoxItem();
            }
            else if (currentItem is AIMessage)
            {
                return new AIMessageListBoxItem();
            }
        }
        return base.GetContainerForItemOverride();
    }

    protected override void OnItemsChanged(NotifyCollectionChangedEventArgs e)
    {
        base.OnItemsChanged(e);
        _currentItemIndex = -1; 
    }
}

public static class ListBoxAutoScrollBehavior
{
    public static readonly DependencyProperty AutoScrollProperty =
        DependencyProperty.RegisterAttached (
            "AutoScroll",
            typeof (bool),
            typeof (ListBoxAutoScrollBehavior),
            new PropertyMetadata (false, OnAutoScrollChanged));

    public static bool GetAutoScroll(DependencyObject obj) =>
        (bool)obj.GetValue (AutoScrollProperty);

    public static void SetAutoScroll(DependencyObject obj, bool value) =>
        obj.SetValue (AutoScrollProperty, value);

    private static void OnAutoScrollChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is ListBox listBox)
        {
            if ((bool)e.NewValue)
            {
                var items = listBox.Items;
                var ic = items as INotifyCollectionChanged;
                if (ic != null)
                {
                    ic.CollectionChanged += (sender, args) =>
                    {
                        if (args.Action == NotifyCollectionChangedAction.Add)
                        {
                            if (listBox.Items.Count > 0)
                            {
                                listBox.ScrollIntoView (listBox.Items[listBox.Items.Count - 1]);
                            }
                        }
                    };
                }
            }
        }
    }
}
