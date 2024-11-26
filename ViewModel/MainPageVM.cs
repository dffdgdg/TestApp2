using System.Windows.Input;
using TestApp.View;
using CommunityToolkit.Mvvm.Input;
using System.Windows.Controls;
using TestApp.View.Account;
using TestApp.View.District;
using TestApp.Services;

namespace TestApp.ViewModel
{
    public class MainPageVM : BaseViewModel
    {
        public ICommand NavigateCommand { get; }
        public ICommand GoBackCommand { get; }
        public ICommand GoForwardCommand { get; }
        private NavigationService mainService;
        public MainPageVM(Frame frame, int userId, int userType, NavigationService mainService)
        {
            this.mainService = mainService;
            service = new NavigationService(frame);
            NavigateCommand = new RelayCommand<string>(Navigate);
            GoBackCommand = new RelayCommand(GoBack);
            GoForwardCommand = new RelayCommand(GoForward);
            this.userId = userId;
            this.UserType = userType;
            Navigate("Регионы");
        }

        private void Navigate(string parameter)
        {
            switch (parameter)
            {
                case "Регионы":
                    DistinctVM distinctVM = new(userId,UserType,service);
                    service.Navigate(typeof(DistrictsPage), distinctVM);
                    break;
                case "Аккаунты":
                    service.Navigate(typeof(AccountPage));
                    break;
                case "Отчеты":
                    service.Navigate(typeof(ReportPage));
                    break;
                case "Профиль":
                    ProfileVM profileVM = new(userId, mainService);
                    service.Navigate(typeof(ProfilePage), profileVM);
                    break;
                case "Настройки":
                    service.Navigate(typeof(SettingsPage));
                    break;
            }
        }

        private void GoBack() => service.GoBack();

        private void GoForward() => service.GoForward();
    }
}
