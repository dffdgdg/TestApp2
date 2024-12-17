using System.Windows.Controls;
using TestApp.Services;
using TestApp.View.Authentication;
using TestApp.View.District;
using Wpf.Ui.Appearance;

namespace TestApp.ViewModel;

public class MainWindowVM : BaseViewModel
{
    public MainWindowVM(Frame frame)
    {
        service = new NavigationService(frame);
        switch (AppSettings.Default.Theme)
        {
            case "Light":
                ApplicationThemeManager.Apply(ApplicationTheme.Light, Wpf.Ui.Controls.WindowBackdropType.Tabbed);
                break;
            case "Dark":
                ApplicationThemeManager.Apply(ApplicationTheme.Dark, Wpf.Ui.Controls.WindowBackdropType.Mica);
                break;
        }
        var vm = new LoginVM(service);
        service.Navigate(typeof(LoginPage), vm);
    }
}