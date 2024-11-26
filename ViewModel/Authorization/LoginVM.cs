using CommunityToolkit.Mvvm.Input;
using Microsoft.EntityFrameworkCore;
using System.Windows.Input;
using TestApp.Model;
using TestApp.Services;
using TestApp.View;
using TestApp.View.Authentication;

namespace TestApp.ViewModel
{
    public class LoginVM : BaseViewModel
    {
        private string _username;
        private string _password;
        private bool savePassword;
        public string Username
        {
            get => _username;
            set => SetProperty(ref _username, value);
        }

        public string Password
        {
            get => _password;
            set => SetProperty(ref _password, value);
        }
        public bool SavePassword
        {
            get => savePassword;
            set => SetProperty(ref savePassword, value);
        }

        public ICommand Authorize { get; }
        public ICommand Register { get; }
        public LoginVM(INavigationService service)
        {
            this.service = service;
            Authorize = new RelayCommand(() => OnLoginAsync(showMessages: true));
            Register = new RelayCommand(OnRegister);
            if (!string.IsNullOrWhiteSpace(AppSettings.Default.Login) && !string.IsNullOrWhiteSpace(AppSettings.Default.Password))
            {
                Username = AppSettings.Default.Login;
                Password = AppSettings.Default.Password;
                OnLoginAsync(showMessages: false);
            }
        }
        public LoginVM()
        {
            Authorize = new RelayCommand(() => OnLoginAsync(showMessages: true));
            Register = new RelayCommand(OnRegister);
            if (!string.IsNullOrWhiteSpace(AppSettings.Default.Login) && !string.IsNullOrWhiteSpace(AppSettings.Default.Password))
            {
                Username = AppSettings.Default.Login;
                Password = AppSettings.Default.Password;
                OnLoginAsync(showMessages: false);
            }
        }

        private void OnRegister()
        {
            RegistrationVM vm = new(service);
            service.Navigate(typeof(RegistrationPage), vm);
        }

        private async void OnLoginAsync(bool showMessages)
        {
            if (string.IsNullOrWhiteSpace(Username))
            {
                if (showMessages) ShowMessage("Введите логин", "Авторизация");
                return;
            }

            if (string.IsNullOrWhiteSpace(Password))
            {
                if (showMessages) ShowMessage("Введите пароль", "Авторизация");
                return;
            }

            try
            {
                using var dc = new TestDbContext();
                var (userId, userType) = dc.AuthenticateUser(Username, Password);

                if (userId > 0)
                {
                    var user = await dc.Users.FirstOrDefaultAsync(u => u.Id == userId);
                    if (user != null)
                    {
                        if (SavePassword) 
                        { 
                            AppSettings.Default.Login = Username; 
                            AppSettings.Default.Password = Password;
                            AppSettings.Default.Save();
                        }
                        service.Navigate(typeof(MainPage), user.Id, user.Usertype, service);
                        if (showMessages) Navigation.Show($"Добро пожаловать, {user.Login}!");
                    }
                    else
                    {
                        if (showMessages) ShowMessage("Пользователь не найден", "Авторизация");
                    }
                }
                else
                {
                    if (showMessages) ShowMessage("Неверный логин или пароль", "Авторизация");
                }
            }
            catch (Exception ex)
            {
                if (showMessages) ShowMessage($"Ошибка авторизации: {ex.Message}", "Авторизация");
            }
        }
    }
}