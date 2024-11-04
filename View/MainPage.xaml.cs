using System.Windows.Controls;
using TestApp.ViewModel;

namespace TestApp.View
{
    /// <summary>
    /// Логика взаимодействия для MainPage.xaml
    /// </summary>
    public partial class MainPage : Page
    {
        public MainPage(int userId, int usertType)
        {
            InitializeComponent();
            DataContext = new MainPageVM(RootNavigation, userId, usertType);
        }
        //  " Scaffold-DbContext "Host=localhost;Port=5432;Database=TestDB;Username=postgres;Password=qwaszxedc1" Npgsql.EntityFrameworkCore.PostgreSQL -OutputDir Model -Context TestDbContext -f"
    }
}