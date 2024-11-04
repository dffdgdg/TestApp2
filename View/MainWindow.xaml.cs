using TestApp.Model.Classes;
using TestApp.ViewModel;
using Wpf.Ui.Appearance;

namespace TestApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = new MainWindowVM();
            Navigation.ContainerFrame = MainFrame;
            Navigation.Initialize(SnackbarControl);
        }
    }
}
