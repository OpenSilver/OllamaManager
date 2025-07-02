using System.Globalization;
using System.Windows.Data;
using System.Windows;

namespace OllamaHub.Support.Local.Converters;

public class EqualToVisibilityConverter : IValueConverter
{
    public string Id { get; set; }

    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is bool isChecked && isChecked)
        {
            if (parameter.Equals(Id))
            {
                return Visibility.Visible;
            }
        }
        return Visibility.Hidden;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
