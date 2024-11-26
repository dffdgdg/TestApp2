using CommunityToolkit.Mvvm.Input;
using Microsoft.EntityFrameworkCore;
using System.Collections.ObjectModel;
using System.Windows;
using TestApp.Model;
using TestApp.ViewModel.Base;

namespace TestApp.ViewModel
{
    /// <summary>ViewModel страницы управления аккаунтами.</summary>
    public class AccountVM : BaseUserControlViewModel
    {
        #region Переменные

        private ObservableCollection<User> users;
        private ObservableCollection<Usertype> roles;
        private User curUser;
        private User selectedUser;
        private string curLogin, curPas;

        public ObservableCollection<User> Users
        {
            get => users;
            set => SetProperty(ref users, value);
        }

        public User CurUser
        {
            get => curUser;
            set => SetProperty(ref curUser, value);
        }
        public string CurPas
        {
            get => curPas;
            set => SetProperty(ref curPas, value);
        }
        public User SelectedUser
        {
            get => selectedUser;
            set => SetProperty(ref selectedUser, value);
        }

        public ObservableCollection<Usertype> Roles
        {
            get => roles;
            set => SetProperty(ref roles, value);
        }

        #endregion

        #region Конструктор

        public AccountVM()
        {
            using TestDbContext dc = new();
            Roles = new ObservableCollection<Usertype>(dc.Usertypes);
            UpdateData();
        }

        #endregion

        #region Методы управления аккаунтами

        // Открытие формы для добавления нового пользователя
        protected void AddItem()
        {
            UCVisibility = Visibility.Visible;
            CurUser = new User { Usertype = 1 };
        }

        // Открытие выбранного пользователя для редактирования
        protected void OpenItem()
        {
            if (SelectedUser != null)
            {
                UCVisibility = Visibility.Visible;
                CurUser = selectedUser;
                curLogin = CurUser.Login;
            }
        }

        // Сохранение данных пользователя
        protected void SaveItem()
        {
            try
            {
                string? errorMessage = ValidateUser();
                if (errorMessage != null)
                {
                    ShowMessage(errorMessage);
                    return;
                }
                using TestDbContext dc = new();
                if (CurUser.Id > 0)
                {
                    var existingUser = dc.Users.Find(CurUser.Id);
                    if (existingUser != null)
                    {
                        if (!string.IsNullOrWhiteSpace(CurPas))
                        {
                            existingUser.Password = CurUser.Password;
                        }
                        existingUser.Password = CurPas;
                        existingUser.Surname = CurUser.Surname;
                        existingUser.Name = CurUser.Name;
                        existingUser.Usertype = CurUser.Usertype;

                        dc.Users.Update(existingUser);
                        ShowPopup("Изменение прошло успешно");
                    }
                }
                else 
                {
                    var newUser = new User
                    {
                        Login = CurUser.Login,
                        Password = CurPas,
                        Surname = CurUser.Surname,
                        Name = CurUser.Name,
                        Usertype = CurUser.Usertype,
                        Birthdate = CurUser.Birthdate
                    };
                    dc.Users.Add(newUser);
                    ShowPopup("Добавление прошло успешно");
                }

                dc.SaveChanges();
                UpdateData();
                UCVisibility = Visibility.Collapsed;
            }
            catch (Exception ex)
            {
                ShowError($"Ошибка: {ex.Message}");
            }
        }
        protected override void OnClose()
        {
            UpdateData();
            UCVisibility = Visibility.Collapsed;
        }

        private void UpdateData()
        {
            using TestDbContext dc = new();
            Users = new ObservableCollection<User>(dc.Users.Include(u => u.UsertypeNavigation));
        }

        private string? ValidateUser()
        {
            if (string.IsNullOrWhiteSpace(CurUser.Surname))
                return "Введите фамилию!";
            else if (string.IsNullOrWhiteSpace(CurUser.Name))
                return "Введите имя!";
            else if (string.IsNullOrWhiteSpace(CurUser.Password))
                return "Введите пароль!";
            else if (string.IsNullOrWhiteSpace(CurUser.Login))
                return "Введите логин!";
            else if (Users.Any(u => u.Login == CurUser.Login && u.Id != CurUser.Id))
                return "Аккаунт с таким логином уже существует!";
            return null;
        }

        #endregion
        private RelayCommand? _add, _open, _save;

        public RelayCommand Open => _open ??= new RelayCommand(OpenItem);
        public RelayCommand Add => _add ??= new RelayCommand(AddItem);
        public RelayCommand Save => _save ??= new RelayCommand(SaveItem);
    }
}