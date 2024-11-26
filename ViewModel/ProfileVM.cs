using CommunityToolkit.Mvvm.Input;
using System.Windows;
using System.Windows.Input;
using TestApp.Model;
using TestApp.Services;
using TestApp.View.Authentication;
using TestApp.ViewModel.Base;

namespace TestApp.ViewModel
{
    public class ProfileVM : BaseUserControlViewModel
    {
        private string _email, _passwordCheck;
        private double _totalPoints;
        private int _completedTests;
        private double _averageScore;
        private string _oldPassword;
        private string _newPassword;

        public string OldPassword
        {
            get => _oldPassword;
            set => SetProperty(ref _oldPassword, value);
        }

        public string NewPassword
        {
            get => _newPassword;
            set => SetProperty(ref _newPassword, value);
        }
        private User _curUser;
        public User CurUser
        {
            get => _curUser;
            set
            {
                SetProperty(ref _curUser, value);
                UpdateMetrics();
            }
        }
        public ICommand ChangePasswordCommand => new RelayCommand(OnChangePasswordCommand);
        public ICommand ChangeNameCommand => new RelayCommand(OnChangeNameCommand);

        private Visibility _pasVisibility = Visibility.Collapsed;
        public Visibility PasUcVisibility { get => _pasVisibility; set => SetProperty(ref _pasVisibility, value); }
        private User _selUser;
        public User SelUser { get => _selUser; set => SetProperty(ref _selUser, value); }

        public string Email
        {
            get => _email;
            set => SetProperty(ref _email, value);
        }

        public string PasswordCheck
        {
            get => _passwordCheck;
            set => SetProperty(ref _passwordCheck, value);
        }

        public double TotalPoints
        {
            get => _totalPoints;
            set => SetProperty(ref _totalPoints, value);
        }

        public int CompletedTests
        {
            get => _completedTests;
            set => SetProperty(ref _completedTests, value);
        }

        public double AverageScore
        {
            get => _averageScore;
            set => SetProperty(ref _averageScore, value);
        }

        public ICommand ChangeName { get; }
        public ICommand ChangePassword { get; }
        public ICommand Logout { get; }
        public ICommand ClosePasUC { get; }

        public ProfileVM(int userId, NavigationService service)
        {
            if (service == null) MessageBox.Show("");
            this.service = service;
            this.userId = userId;
            ChangeName = new RelayCommand(OnChangeName);
            ChangePassword = new RelayCommand(OnChangePassword);
            Logout = new RelayCommand(OnLogout);
            ClosePasUC = new RelayCommand(OnClosePasUC);
            LoadUserData();
        }

        private void LoadUserData()
        {
            using var db = new TestDbContext();
            CurUser = db.Users.SingleOrDefault(u => u.Id == userId);
        }

        private void UpdateMetrics()
        {
            using var db = new TestDbContext();
            var userResults = db.Userresults.Where(u => u.User == CurUser.Id);
            TotalPoints = (double)userResults.Sum(u => u.Score);
            CompletedTests = userResults.Count();
            AverageScore = CompletedTests > 0 ? (double)TotalPoints / CompletedTests : 0; 
        }


        private void OnChangeName()
        {
            SelUser = new User
            {
                Id = CurUser.Id,
                Surname = CurUser.Surname,
                Name = CurUser.Name,
                Midname = CurUser.Midname,
                Password = CurUser.Password 
            };
            NewPassword = "";
            UCVisibility = Visibility.Visible;
        }
        protected void OnClosePasUC()
        {
            PasUcVisibility = Visibility.Collapsed;
            UCVisibility = Visibility.Collapsed;
        }

        protected override void OnClose()
        {
            UCVisibility = Visibility.Collapsed;
            PasUcVisibility = Visibility.Collapsed;
        }
        private void OnChangePassword()
        {
            SelUser = new User
            {
                Id = CurUser.Id,
                Surname = CurUser.Surname,
                Name = CurUser.Name,
                Midname = CurUser.Midname,
                Password = CurUser.Password
            };
            OldPassword = "";
            NewPassword = "";
            PasUcVisibility = Visibility.Visible;
        }

        private void OnChangePasswordCommand()
        {
            try
            {
                using var db = new TestDbContext();
                if (db.CheckPassword(CurUser.Login, OldPassword))
                {
                    CurUser.Password = NewPassword;
                    db.Update(CurUser);
                    db.SaveChanges();
                    ShowPopup("Сохранение прошло успешно");
                }
                else ShowMessage("Ошибка изменения", "Изменение пароля");
            }
            catch(Exception ex) { ShowMessage(ex.ToString()); }
        }
        private void OnChangeNameCommand()
        {
            try
            {
                using var db = new TestDbContext();
                if (db.CheckPassword(CurUser.Login, NewPassword))
                {
                    CurUser.Name = SelUser.Name;
                    CurUser.Surname = SelUser.Surname;
                    CurUser.Midname = SelUser.Midname;
                    db.Users.Update(CurUser);
                    db.SaveChanges();
                    LoadUserData();
                    ShowPopup("Сохранение прошло успешно");
                }
                else ShowError("Ошибка изменения", "Изменение ФИО");
            }
            catch(Exception ex)
            {
                ShowError(ex.ToString());
            }
        }
        private void OnLogout()
        {
            LoginVM vm = new();
            service.Navigate(typeof(LoginPage), vm);
        }
    }
}
