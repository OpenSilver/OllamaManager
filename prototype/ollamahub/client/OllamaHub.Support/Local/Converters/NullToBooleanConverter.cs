using System;
using System.Globalization;
using System.Windows.Data;

namespace OllamaHub.Support.Local.Converters;

public class NullToBooleanConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        // CurrentModel이 null이면 false, null이 아니면 true 반환
        return value != null;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}