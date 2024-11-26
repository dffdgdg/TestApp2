using System.Globalization;
using System.Windows.Data;

namespace TestApp.Converters
{
    public class DateOnlyToDateTimeConverter : IValueConverter
    {
        // Преобразование DateOnly в DateTime для DatePicker
        public object? Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is DateOnly dateOnly)
            {
                // Преобразуем DateOnly в DateTime
                return dateOnly.ToDateTime(new TimeOnly(0)); // Временная часть установлена в полночь
            }
            return null; // Если значение null, возвращаем null
        }

        // Преобразование DateTime обратно в DateOnly
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is DateTime dateTime)
            {
                // Преобразуем DateTime в DateOnly
                return DateOnly.FromDateTime(dateTime);
            }
            return default(DateOnly); // Возвращаем DateOnly.Default, если значение null
        }
    }
}
