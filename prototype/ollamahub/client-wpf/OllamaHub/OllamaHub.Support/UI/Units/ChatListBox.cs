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
