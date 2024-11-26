using System.Windows.Controls;
using Wpf.Ui.Controls;

namespace TestApp.Services
{
    public static class Navigation
    {
        private static View.Snackbar? _snackbar;
        public static void Initialize(View.Snackbar snackbar)
        {
            _snackbar = snackbar;
        }

        public static void Show(string message)
        {
            _snackbar?.Show(message);
        }
    }
}