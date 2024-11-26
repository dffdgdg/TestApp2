using TestApp.Services;
using TestApp.ViewModel;

namespace TestApp;

public partial class MainWindow
{
    public MainWindow()
    {
        InitializeComponent();
        DataContext = new MainWindowVM(MainFrame);
        Navigation.Initialize(SnackbarControl);
    }
}
