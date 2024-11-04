using Microsoft.Extensions.Configuration;
using Wpf.Ui.Appearance;

namespace TestApp.ViewModel;

public class MainWindowVM : BaseViewModel
{
    public MainWindowVM()
    {
        switch (AppSettings.Default.Theme)
        {
            case "Light":
                ApplicationThemeManager.Apply(ApplicationTheme.Light);
                break;
            case "Dark":
                ApplicationThemeManager.Apply(ApplicationTheme.Dark);
                break;
        }
    }
}