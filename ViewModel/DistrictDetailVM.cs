using CommunityToolkit.Mvvm.Input;
using Microsoft.EntityFrameworkCore;
using Microsoft.Win32;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows;
using TestApp.Helpers;
using TestApp.Model;
using TestApp.Model.Classes;
using TestApp.View;
using TestApp.View.HistoricalSite;
using TestApp.View.Test;

namespace TestApp.ViewModel
{
    public class DistrictDetailVM : BaseViewModel
    {
        // Свойства для данных района
        private readonly TestDbContext _db;
        private int id;

        public byte[] Image { get; set; }
        public string Name { get; set; }
        public string Foundating_date { get; set; }
        public string Description { get; set; }

        // Команды для исторических достопримечательностей
        public RelayCommand LoadImage { get; }
        public RelayCommand OpenSite { get; }
        public RelayCommand AddSite { get; }
        public RelayCommand<HistoricalSite> RemoveSite { get; }
        public RelayCommand<HistoricalSite> EditSite { get; }
        public RelayCommand Save { get; }
        public RelayCommand Close { get; }
        public ObservableCollection<HistoricalSite> HistoricalSites { get; private set; }
        private HistoricalSite curSite, selectedSite;

        public HistoricalSite CurSite
        {
            get => curSite;
            set => SetProperty(ref curSite, value);
        }

        public HistoricalSite SelectedSite
        {
            get => selectedSite;
            set => SetProperty(ref selectedSite, value);        
        }

        // Команды для тестов
        public RelayCommand OpenTest { get; }
        public RelayCommand AddTest { get; }
        public RelayCommand<Test> RemoveTest { get; }
        public RelayCommand<Test> EditTest { get; }
        public ObservableCollection<Test> Tests { get; private set; }
        private Test curTest, selectedTest;

        public Test CurTest
        {
            get => curTest;
            set => SetProperty(ref curTest, value);
        }

        public Test SelectedTest
        {
            get => selectedTest;
            set => SetProperty(ref selectedTest, value);
        }

        public DistrictDetailVM(District district, INavigationService service, int userId, int userType)
        {
            _db = new TestDbContext();
            Name = district.Name;
            Foundating_date = district.FoundingDate.ToString();
            Description = district.Description;
            Image = district.Image;
            id = district.Id;

            this.service = service;
            this.userId = userId;
            this.UserType = userType;
            // Загрузка исторических достопримечательностей
            HistoricalSites = new ObservableCollection<HistoricalSite>(_db.HistoricalSites.Where(u => u.District == district.Id).ToList());
            OpenSite = new RelayCommand(OpenSites);
            AddSite = new RelayCommand(AddSites);
            RemoveSite = new RelayCommand<HistoricalSite>(RemoveSites);
            Save = new RelayCommand(SaveSites);
            Close = new RelayCommand(CloseUC);
            EditSite = new RelayCommand<HistoricalSite>(EditSites);
            LoadImage = new RelayCommand(LoadSiteImage);

            // Загрузка тестов для района
            Tests = new ObservableCollection<Test>(_db.Tests.Where(u => u.District == district.Id).ToList());
            OpenTest = new RelayCommand(OpenTestItem);
            AddTest = new RelayCommand(AddTestItem);
            RemoveTest = new RelayCommand<Test>(RemoveTestItem);
            EditTest = new RelayCommand<Test>(EditTestItem);
        }

        private void UpdateData()
        {
            HistoricalSites.Clear();
            foreach (var site in _db.HistoricalSites.Where(u => u.District == id).ToList())
                HistoricalSites.Add(site);

            Tests.Clear();
            foreach (var test in _db.Tests.Where(u => u.District == id).ToList())
                Tests.Add(test);
        }

        private void OpenSites()
        {
            if (SelectedSite == null) return;
            Navigation.Show($"Открытие достопримечательности: {SelectedSite.Name}");
            var vm = new HistoricalSiteVM(SelectedSite);
            service.Navigate(typeof(SitePage), vm);
        }

        private void AddSites()
        {
            UCVisibility = Visibility.Visible;
            CurSite = new HistoricalSite { District = id };
        }

        private void RemoveSites(HistoricalSite item)
        {
            if (item == null) return;

            var result = MessageBox.Show("Вы точно хотите удалить достопримечательность?", "Подтверждение удаления", MessageBoxButton.YesNo);
            if (result == MessageBoxResult.Yes)
            {
                _db.HistoricalSites.Remove(item);
                _db.SaveChanges();
                HistoricalSites.Remove(item);
            }
        }

        private void CloseUC()
        {
            UpdateData();
            UCVisibility = Visibility.Collapsed;
        }

        protected void EditSites(HistoricalSite item)
        {
            if (item == null) return;
            UCVisibility = Visibility.Visible;
            CurSite = item;
        }

        private void LoadSiteImage()
        {
            var openFileDialog = new OpenFileDialog
            {
                Filter = "Image Files (*.jpg;*.jpeg;*.png;*.bmp)|*.jpg;*.jpeg;*.png;*.bmp",
                Title = "Выберите изображение"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                CurSite.Image = File.ReadAllBytes(openFileDialog.FileName);
            }
        }

        private void SaveSites()
        {
            try
            {
                string? errorMessage = ValidateCurSite();
                if (errorMessage != null)
                {
                    MessageBox.Show(errorMessage);
                    return;
                }

                if (_db.HistoricalSites.Local.Any(s => s.Id == CurSite.Id))
                    _db.HistoricalSites.Update(CurSite);
                else 
                    _db.HistoricalSites.Add(CurSite);

                _db.SaveChanges();
                UpdateData();
                UCVisibility = Visibility.Collapsed;
            }
            catch (DbUpdateException dbEx)
            {
                var innerExceptionMessage = dbEx.InnerException != null ? dbEx.InnerException.Message : "Неизвестная ошибка.";
                MessageBoxs.Show($"Ошибка при сохранении данных: {innerExceptionMessage}");
            } 
        }

        private string? ValidateCurSite()
        {
            if (string.IsNullOrWhiteSpace(CurSite?.Name))
                return "Введите название!";
            if (string.IsNullOrWhiteSpace(CurSite?.Description))
                return "Введите описание!";
            if (CurSite?.ConstructionDate == null)
                return "Введите дату постройки!";
            if (CurSite?.Image == null)
                return "Выберите изображение!";
            if (_db.HistoricalSites.Any(s => s.Name == CurSite.Name && s.Id != curSite.Id))
                return "Достопримечательность с таким именем уже существует!";
            return null;
        }

        // Реализация для тестов

        private void OpenTestItem()
        {
            if (SelectedTest == null) return;
            Navigation.Show($"Открытие теста: {SelectedTest.Title}");
            if (UserType == 0)
            {
                var vm = new TestQuestionsVM(SelectedTest, userId, service);
                service.Navigate(typeof(TestQuestionsPage), vm);
            }
            else
            {
                service.Navigate(typeof(AddTestPage));
            }
        }

        private void AddTestItem()
        {
            service.Navigate(typeof(AddTestPage));
        }

        private void RemoveTestItem(Test item)
        {
            if (item == null) return;

            var result = MessageBox.Show("Вы точно хотите удалить тест?", "Подтверждение удаления", MessageBoxButton.YesNo);
            if (result == MessageBoxResult.Yes)
            {
                _db.Tests.Remove(item);
                _db.SaveChanges();
                Tests.Remove(item);
            }
        }

        protected void EditTestItem(Test item)
        {
           
        }
    }
}