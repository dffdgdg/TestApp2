using CommunityToolkit.Mvvm.Input;
using Microsoft.EntityFrameworkCore;
using Microsoft.Win32;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows;
using TestApp.Helpers;
using TestApp.Model;
using TestApp.Model.Classes;
using TestApp.View.District;

namespace TestApp.ViewModel
{
    public class DistinctVM : BaseViewModel
    {
        private readonly TestDbContext _db;

        public ObservableCollection<District> Districts { get; private set; }
        private District curDistrict, selectedItem;

        // Свойства для текущего района и выбранного элемента
        public District CurDistrict
        {
            get => curDistrict;
            set => SetProperty(ref curDistrict, value);
        }

        public District SelectedItem
        {
            get => selectedItem;
            set => SetProperty(ref selectedItem, value);
        }

        // Команды
        public RelayCommand Open { get; }
        public RelayCommand Add { get; }
        public RelayCommand LoadImage { get; }
        public RelayCommand<Model.District> Remove { get; }
        public RelayCommand<Model.District> Edit { get; }
        public RelayCommand Save { get; }
        public RelayCommand Close { get; }

        // Конструктор
        public DistinctVM(int userId, int userType, INavigationService service)
        {
            _db = new TestDbContext();
            Districts = new ObservableCollection<District>([.. _db.Districts]);
            this.service = service;
            this.userId = userId;
            this.UserType = userType;
            // Инициализация команд
            Open = new RelayCommand(OpenItem);
            Add = new RelayCommand(AddItem);
            Remove = new RelayCommand<Model.District>(RemoveItem);
            Save = new RelayCommand(SaveItem);
            Close = new RelayCommand(CloseUC);
            Edit = new RelayCommand<Model.District>(EditItem);
            LoadImage = new RelayCommand(LoadItemImage);
            
        }

        // Обновление данных о районах
        private void UpdateData()
        {
            Districts.Clear();
            foreach (var district in _db.Districts.ToList())
            {
                Districts.Add(district);
            }
        }

        // Открытие выбранного района
        private void OpenItem()
        {
            if (SelectedItem == null) return;

            Navigation.Show($"Открытие района: {SelectedItem.Name}");
            var vm = new DistrictDetailVM(SelectedItem, service, userId, UserType);
            service.Navigate(typeof(DistinctDetailPage), vm);
        }

        // Добавление нового района
        private void AddItem()
        {
            UCVisibility = Visibility.Visible;
            CurDistrict = new District();
        }

        // Удаление выбранного района
        private void RemoveItem(Model.District item)
        {
            if (item == null) return;

            string result = MessageBoxs.ShowDialog("Вы точно хотите удалить район?", "Подтверждение удаления", MessageBoxs.Buttons.YesNo);
            if (result == "1")
            {
                _db.Districts.Remove(item);
                _db.SaveChanges();
                Districts.Remove(item);
                MessageBoxs.Show("Удаление произошло успешно");
            }
            else
            {
                MessageBox.Show(result);
            }
        }

        // Закрытие пользовательского интерфейса
        private void CloseUC()
        {
            UpdateData();
            UCVisibility = Visibility.Collapsed;
        }

        // Редактирование выбранного района
        protected void EditItem(Model.District item)
        {
            if (item == null) return;
            UCVisibility = Visibility.Visible;
            CurDistrict = item;
        }

        // Загрузка изображения для района
        private void LoadItemImage()
        {
            var openFileDialog = new OpenFileDialog
            {
                Filter = "Image Files (*.jpg;*.jpeg;*.png;*.bmp)|*.jpg;*.jpeg;*.png;*.bmp",
                Title = "Выберите изображение"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                CurDistrict.Image = File.ReadAllBytes(openFileDialog.FileName);
            }
        }

        // Сохранение данных о районе
        private void SaveItem()
        {
            try
            {
                string? errorMessage = ValidateCurDistrict();

                if (errorMessage != null)
                {
                    ShowMessage(errorMessage);
                    return;
                }

                if (_db.Districts.Local.Any(d => d.Id == CurDistrict.Id))
                {
                    _db.Districts.Update(CurDistrict);
                }
                else
                {
                    _db.Districts.Add(CurDistrict);
                }

                _db.SaveChanges();
                UpdateData(); // Обновление списка после сохранения
                UCVisibility = Visibility.Collapsed; // Скрытие пользовательского интерфейса
            }
            catch (DbUpdateException dbEx)
            {
                var innerExceptionMessage = dbEx.InnerException != null ? dbEx.InnerException.Message : "Неизвестная ошибка.";
                ShowMessage($"Ошибка при сохранении данных: {innerExceptionMessage}");
            }
            catch (Exception ex)
            {
                ShowMessage($"Ошибка: {ex.Message}");
            }
        }

        // Валидация текущего района
        private string? ValidateCurDistrict()
        {
            if (string.IsNullOrWhiteSpace(CurDistrict?.Name))
                return "Введите название!";
            if (string.IsNullOrWhiteSpace(CurDistrict?.Description))
                return "Введите описание!";
            if (CurDistrict?.FoundingDate == null)
                return "Введите дату основания!";
            if (CurDistrict?.Image == null)
                return "Выберите изображение!";
            if (_db.Districts.Any(u => u.Name == CurDistrict.Name && u.Id != CurDistrict.Id))
                return "Регион с таким именем уже существует!";
            return null;
        }
    }
}