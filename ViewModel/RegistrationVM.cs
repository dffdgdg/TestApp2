using CommunityToolkit.Mvvm.Input;
using System.Windows.Input;
using TestApp.Helpers;
using TestApp.Model;
using TestApp.Model.Classes;
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

    public RegistrationVM()
    {
        Registration = new RelayCommand(OnRegistration);
        Logining = new RelayCommand(OnLogining);
    }
    private void OnLogining() => Navigation.ContainerFrame.Navigate(new LoginPage());
    private void OnRegistration()
    {
        try
        {
            if (!Validate()) return;
            using TestDbContext dc = new();
            if (dc.Users.Any(u => u.Login == Login))
            {
                Navigation.Show("Пользователь с таким логином уже существует!");
                return;
            }
            var user = new User
            {
                Surname = LastName,
                Name = FirstName,
                Midname = MiddleName,
                Login = Login,
                Password = Password,
                Birthdate = BirthDate
            };
            dc.Users.AddAsync(user);
            dc.SaveChangesAsync();
            Navigation.Show("Регистрация успешно завершена!");
            Navigation.ContainerFrame.Navigate(new LoginPage());
        }
        catch (Exception ex)
        {
            MessageBoxs.Show($"Ошибка при регистрации: {ex.Message}");
        }
    }

    private bool Validate()
    {
        if (string.IsNullOrEmpty(LastName))
        {
            Navigation.Show("Введите фамилию!");
            return false;
        }
        else if (string.IsNullOrEmpty(FirstName))
        {
            Navigation.Show("Введите имя!");
            return false;
        }
        else if (string.IsNullOrEmpty(MiddleName))
        {
            Navigation.Show("Введите отчество!");
            return false;
        }
        else if (string.IsNullOrEmpty(Login))
        {
            Navigation.Show("Введите логин!");
            return false;
        }
        else if (string.IsNullOrEmpty(Password))
        {
            Navigation.Show("Введите пароль!");
            return false;
        }
        else return true;
    }
}
