using CommunityToolkit.Mvvm.Input;
using System.Windows;
using System.Windows.Input;
using TestApp.Model;
using TestApp.Model.Classes;
using TestApp.View.Authentication;

namespace TestApp.ViewModel
{
    public class ProfileVM : BaseViewModel
    {
        private string _email, _passwordCheck;
        private double _totalPoints;
        private int _completedTests;
        private double _averageScore;

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

        public ProfileVM(int userId)
        {
            this.userId = userId;
            ChangeName = new RelayCommand(onChangeName);
            ChangePassword = new RelayCommand(onChangePassword);
            Logout = new RelayCommand(onLogout);
            ClosePasUC = new RelayCommand(onClosePasUC);
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
            var userResults = db.Userresults.Where(u => u.Userid == CurUser.Id);
            TotalPoints = (double)userResults.Sum(u => u.Score);
            CompletedTests = userResults.Count();
            AverageScore = CompletedTests > 0 ? (double)TotalPoints / CompletedTests : 0; 
        }


        private void onChangeName()
        {
            SelUser = new User
            {
                Id = CurUser.Id,
                Surname = CurUser.Surname,
                Name = CurUser.Name,
                Midname = CurUser.Midname,
                Password = CurUser.Password 
            };
            UCVisibility = Visibility.Visible;
        }
        private void onClosePasUC()
        {
            UCVisibility = Visibility.Collapsed;
        }
        private void onChangePassword()
        {
            SelUser = new User
            {
                Id = CurUser.Id,
                Surname = CurUser.Surname,
                Name = CurUser.Name,
                Midname = CurUser.Midname,
                Password = CurUser.Password
            };
            PasUcVisibility = Visibility.Visible;
        }

        private void SaveChanges(bool isNameChange)
        {
            using var db = new TestDbContext();
            if (SelUser.Password == PasswordCheck)
            {
                db.Users.Update(SelUser);
                CurUser = SelUser;
                db.SaveChanges();
                ShowMessage("Изменение прошло успешно!", isNameChange ? "Изменение ФИО" : "Изменение пароля");
            }
            else
                ShowMessage("Пароли не совпадают", isNameChange ? "Изменение ФИО" : "Изменение пароля");
        }

        private void onLogout()
        {
            Navigation.ContainerFrame.Navigate(typeof(LoginPage));
        }
    }
}
