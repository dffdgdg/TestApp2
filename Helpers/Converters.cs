using System.Globalization;
using System.Windows.Data;
using System.Windows;
using System.IO;
using System.Windows.Media.Imaging;

namespace TestApp.Helpers
{
    internal class BooleanConverter : IValueConverter
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
    internal class ReverseBooleanConverter : IValueConverter
    {
        // Метод Convert преобразует значение value в тип bool и возвращает true, если оно равно параметру parameter, иначе - false.
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null && parameter != null)
            {
                if (value.ToString() == parameter.ToString()) return false;
                return true;
            }
            return false;
        }
        // Метод ConvertBack преобразует значение value типа bool обратно в исходный тип данных.
        // Если значение равно true, то метод возвращает true, иначе - false.
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        { return value is bool boolean && boolean == true; }
    }
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
    public class ImageToBitmapImageConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is byte[] imageData)
            {
                using (var stream = new MemoryStream(imageData))
                {
                    var bitmap = new BitmapImage();
                    bitmap.BeginInit();
                    bitmap.StreamSource = stream;
                    bitmap.CacheOption = BitmapCacheOption.OnLoad;
                    bitmap.EndInit();
                    bitmap.Freeze(); // Чтобы избежать проблем с потоками
                    return bitmap;
                }
            }
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
    public class DateOnlyToDateTimeConverter : IValueConverter
    {
        // Преобразование DateOnly в DateTime для DatePicker
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
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
