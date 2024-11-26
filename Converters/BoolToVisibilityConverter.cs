using System.Globalization;
using System.Windows.Data;
using System.Windows;

namespace TestApp.Converters
{
    public class BoolToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool boolValue = (bool)value;
            if (boolValue) return Visibility.Visible;
            else return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Visibility visibilityValue = (Visibility)value;
            if (visibilityValue == Visibility.Visible) return true;
            else return false;
        }
    }
}
