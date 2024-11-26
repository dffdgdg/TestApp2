using System.Globalization;
using System.Windows.Data;

namespace TestApp.Converters
{
    public class BooleanConverter : IValueConverter
    {
        // Метод Convert преобразует значение value в тип bool и возвращает true, если оно равно параметру parameter, иначе - false.
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null && parameter != null)
            {
                if (value.ToString() == parameter.ToString()) return true;
                return false;
            }
            return false;
        }
        // Метод ConvertBack преобразует значение value типа bool обратно в исходный тип данных.
        // Если значение равно true, то метод возвращает true, иначе - false.
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        { return value is bool boolean && boolean == true; }
    }
}
