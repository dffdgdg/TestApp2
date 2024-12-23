using CommunityToolkit.Mvvm.Input;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using TestApp.Services;

namespace TestApp.ViewModel
{
    public abstract class BaseViewModel : INotifyPropertyChanged
    {
        protected INavigationService service;
        protected int userId;

        private int _userType;
        protected int UserType
        {
            get => _userType;
            set
            {
                if (SetProperty(ref _userType, value))
                {
                    OnPropertyChanged(nameof(IsWorker));
                    OnPropertyChanged(nameof(IsAdmin));
                }
            }
        }
        public bool IsWorker => UserType != 1;
        public bool IsAdmin => UserType == 3;

        protected bool SetProperty<T>(ref T storage, T value, [CallerMemberName] string propertyName = null)
        {
            if (Equals(storage, value)) return false;

            storage = value;
            OnPropertyChanged(propertyName);
            return true;
        }
        protected void ShowPopup(string message) => Navigation.Show(message);
        protected bool ShowDialog(string text, string title = "Подтверждение")
        {
            return MessageBoxs.ShowDialog(text, title, MessageBoxs.Buttons.YesNo);
        }
        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        // Метод ShowMessage отображает диалоговое окно с сообщением об ошибке.
        protected internal void ShowMessage(string message) => MessageBoxs.ShowDialog(message, "Внимание", MessageBoxs.Buttons.OK, MessageBoxs.Icon.Info);
        protected internal void ShowMessage(string messasge, string title) => MessageBoxs.ShowDialog(messasge, title, MessageBoxs.Buttons.OK, MessageBoxs.Icon.Info);
        protected internal void ShowError(string message) => MessageBoxs.ShowDialog(message, "Внимание", MessageBoxs.Buttons.OK, MessageBoxs.Icon.Error);
        protected internal void ShowError(string messasge, string title) => MessageBoxs.ShowDialog(messasge, title, MessageBoxs.Buttons.OK, MessageBoxs.Icon.Error);
    }
}
