using CommunityToolkit.Mvvm.Input;
using System.Windows;

namespace TestApp.ViewModel.Base;

public abstract class BaseUserControlViewModel : BaseViewModel
{
    // Свойство UCVisibility определяет видимость элемента управления.
    private Visibility _uCVisibility = Visibility.Collapsed;
    public Visibility UCVisibility { get => _uCVisibility; set { _uCVisibility = value; OnPropertyChanged(); } }
    protected abstract void OnClose();
    private RelayCommand? close;
    public RelayCommand Close => close ??= new RelayCommand(OnClose);
}
