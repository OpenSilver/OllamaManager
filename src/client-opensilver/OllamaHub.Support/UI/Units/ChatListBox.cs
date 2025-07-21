using OllamaHub.Support.Local.Models;
using System.Collections.Specialized;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace OllamaHub.Support.UI.Units;

public class ChatListBox : ListBox
{
    private int _currentItemIndex = -1; 

    public ChatListBox()
    { 
        DefaultStyleKey = typeof(ChatListBox);
        _currentItemIndex = -1;
    }

    protected override DependencyObject GetContainerForItemOverride()
    {
        _currentItemIndex++; 

        if (_currentItemIndex >= 0 && _currentItemIndex < Items.Count)
        {
            var item = Items.LastOrDefault();
            if (item is UserMessage)
            {
                return new UserMessageListBoxItem();
            }
            else if (item is AIMessage)
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
