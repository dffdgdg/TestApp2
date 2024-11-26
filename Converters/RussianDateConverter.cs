using System;
using System.Globalization;
using System.Windows.Data;

namespace TestApp.Converters
{
    public class RussianDateConverter : IValueConverter
    {
        public object? Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null)
            {
                if (DateTime.TryParse(value.ToString(), out DateTime date))
                {
                    if (date.Day == 1 && date.Month == 1)
                    {
                        return $"{date.Year} год";
                    }
                    else
                    {
                        // Создаем русскую культуру
                        CultureInfo russianCulture = new("ru-RU");

                        // Форматируем дату
                        return date.ToString("dd MMMM yyyy", russianCulture);
                    }
                }
                // Если не удалось преобразовать, возвращаем исходное значение
                return value.ToString();
            }
            return string.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}