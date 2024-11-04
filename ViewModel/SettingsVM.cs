using CommunityToolkit.Mvvm.Input;
using System.Windows.Input;
using Wpf.Ui.Appearance;

namespace TestApp.ViewModel
{
    partial class SettingsVM : BaseViewModel
    {
        private int _selectedTheme;
        public int SelectedTheme
        {
            get => _selectedTheme;
            set
            {
                if (SetProperty(ref _selectedTheme, value))
                    UpdateTheme();
            }
        }

        public SettingsVM()
        {
            switch (AppSettings.Default.Theme)
            {
                case "Light": SelectedTheme = 0; break;
                case "Dark": SelectedTheme = 1; break;
            }
        }
        public ICommand ThemeSet => new RelayCommand(UpdateTheme); // Команда для изменения темы
        public ICommand ThemeChangedCommand => new RelayCommand(UpdateTheme);

        private void UpdateTheme()
        {
            switch (SelectedTheme)
            {
                case 0:
                    ApplicationThemeManager.Apply(ApplicationTheme.Light);
                    AppSettings.Default.Theme = "Light";
                    AppSettings.Default.Save();
                    break;
                case 1:
                    ApplicationThemeManager.Apply(ApplicationTheme.Dark);
                    AppSettings.Default.Theme = "Dark";
                    AppSettings.Default.Save();
                    break;
            }
        }

    }
}