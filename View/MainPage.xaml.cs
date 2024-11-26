using System.Windows.Controls;
using TestApp.Services;
using TestApp.ViewModel;

namespace TestApp.View;

public partial class MainPage : Page
{
    public MainPage(int userId, int usertType, NavigationService service)
    {
        InitializeComponent();
        DataContext = new MainPageVM(RootNavigation, userId, usertType, service);
    }
    //  " Scaffold-DbContext "Host=localhost;Port=5432;Database=TestDB;Username=postgres;Password=qwaszxedc1" Npgsql.EntityFrameworkCore.PostgreSQL -OutputDir Model -Context TestDbContext -f"
}