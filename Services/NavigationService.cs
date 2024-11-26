using DocumentFormat.OpenXml.Office2010.Excel;
using System.Windows.Controls;
using TestApp.ViewModel;

namespace TestApp.Services;
public class NavigationService(Frame frame) : INavigationService
{
    private readonly Frame _frame = frame;

    public void Navigate(Type pageType)
    {
        ArgumentNullException.ThrowIfNull(pageType);
        try { _frame.Navigate(Activator.CreateInstance(pageType)); }
        catch (Exception) {}
    }
    public void Navigate(Type pageType, params object[] parameters)
    {
        ArgumentNullException.ThrowIfNull(pageType);
        if (Activator.CreateInstance(pageType, parameters) is Page page)
        {
            _frame.Navigate(page);
        }
        else
        {
            throw new InvalidOperationException("Не удалось создать экземпляр страницы.");
        }
    }
    public void Navigate(Type pageType, object viewModel)
    {
        ArgumentNullException.ThrowIfNull(pageType);
        try
        {
            var page = Activator.CreateInstance(pageType) as Page;
            page.DataContext = viewModel;
            _frame.Navigate(page);
        }
        catch (Exception) { }
    }

    public void GoBack()
    {
        if (_frame.CanGoBack) _frame.GoBack();
    }

    public void GoForward()
    {
        if (_frame.CanGoForward) _frame.GoForward();
    }
}
