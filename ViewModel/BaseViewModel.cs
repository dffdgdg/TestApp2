using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using TestApp.Helpers;
using TestApp.Model.Classes;

namespace TestApp.ViewModel
{
    public abstract class BaseViewModel : INotifyPropertyChanged
    {
        protected INavigationService service;
        protected int userId;

        private int _userType; // Используйте поле для хранения значения.
        protected int UserType
        {
            get => _userType;
            set
            {
                if (SetProperty(ref _userType, value))
                {
                    OnPropertyChanged(nameof(IsWorker)); // Уведомляем об изменении IsWorker.
                }
            }
        }
        public bool IsWorker => UserType != 1;

        protected bool SetProperty<T>(ref T storage, T value, [CallerMemberName] string propertyName = null)
        {
            if (Equals(storage, value)) return false;

            storage = value;
            OnPropertyChanged(propertyName);
            return true;
        }
        public event PropertyChangedEventHandler? PropertyChanged;
        // Метод OnPropertyChanged вызывает событие PropertyChanged, если свойство изменилось.
        // [CallerMemberName] позволяет получить имя свойства, вызвавшего метод, без явного указания его имени.
        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        // Метод ShowMessage отображает диалоговое окно с сообщением об ошибке.
        private protected static void ShowMessage(string message) => MessageBoxs.ShowDialog(message, "Внимание", MessageBoxs.Buttons.OK, MessageBoxs.Icon.Error);
        // Перегруженный метод ShowMessage отображает диалоговое окно с сообщением и заголовком.
        private protected static void ShowMessage(string messasge, string title) => MessageBoxs.ShowDialog(messasge, title, MessageBoxs.Buttons.OK, MessageBoxs.Icon.Error);
        // Свойство UCVisibility определяет видимость элемента управления.
        private Visibility _uCVisibility = Visibility.Collapsed;
        public Visibility UCVisibility { get => _uCVisibility; set { _uCVisibility = value; OnPropertyChanged(); } }
    }
}
