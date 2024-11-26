using CommunityToolkit.Mvvm.Input;
using System.Windows.Input;
using TestApp.Model;
using TestApp.Services;
using TestApp.View.Authentication;

namespace TestApp.ViewModel;

public class RegistrationVM : BaseViewModel
{
    private string _lastName;
    private string _firstName;
    private string _middleName;
    private string _login;
    private string _password;
    private DateOnly _birthDate;

    public string LastName
    {
        get => _lastName;
        set => SetProperty(ref _lastName, value);
    }

    public string FirstName
    {
        get => _firstName;
        set => SetProperty(ref _firstName, value);
    }

    public string MiddleName
    {
        get => _middleName;
        set => SetProperty(ref _middleName, value);
    }

    public string Login
    {
        get => _login;
        set => SetProperty(ref _login, value);
    }

    public string Password
    {
        get => _password;
        set => SetProperty(ref _password, value);
    }

    public DateOnly BirthDate
    {
        get => _birthDate;
        set => SetProperty(ref _birthDate, value);
    }

    public ICommand Registration { get; private set; }
    public ICommand Logining { get; private set; }

    public RegistrationVM(INavigationService service)
    {
        this.service = service;
        Registration = new RelayCommand(OnRegistration);
        Logining = new RelayCommand(OnLogining);
    }
    private void OnLogining() => service.Navigate(typeof(LoginPage));
    private void OnRegistration()
    {
        try
        {
            if (!Validate()) return;
            using TestDbContext dc = new();
            if (dc.Users.Any(u => u.Login == Login))
            {
                ShowPopup("Пользователь с таким логином уже существует!");
                return;
            }
            var user = new User
            {
                Surname = LastName,
                Name = FirstName,
                Midname = MiddleName,
                Login = Login,
                Password = Password,
                Birthdate = BirthDate,
                Usertype = 1
            };
            dc.Users.Add(user);
            dc.SaveChanges();
            Navigation.Show("Регистрация успешно завершена!");
            service.Navigate(typeof(LoginPage));
        }
        catch (Exception ex)
        {
            ShowError($"Ошибка при регистрации: {ex}");

        }
    }

    private bool Validate()
    {
        if (string.IsNullOrEmpty(LastName))
        {
            ShowPopup("Введите фамилию!");
            return false;
        }
        else if (string.IsNullOrEmpty(FirstName))
        {
            ShowPopup("Введите имя!");
            return false;
        }
        else if (string.IsNullOrEmpty(MiddleName))
        {
            ShowPopup("Введите отчество!");
            return false;
        }
        else if (string.IsNullOrEmpty(Login))
        {
            ShowPopup("Введите логин!");
            return false;
        }
        else if (string.IsNullOrEmpty(Password))
        {
            ShowPopup("Введите пароль!");
            return false;
        }
        else return true;
    }
}
