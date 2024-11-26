using System.Globalization;
using System.Windows.Data;
using System.Windows;
using TestApp.ViewModel;

namespace TestApp.Converters;

public class CompletedTestConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (Application.Current.MainWindow?.DataContext is DistrictDetailVM vm && value is int testId)
            return vm.IsTestCompleted(testId) ? Visibility.Visible : Visibility.Collapsed;
        return Visibility.Collapsed;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    { 
        throw new NotImplementedException(); 
    }
}
