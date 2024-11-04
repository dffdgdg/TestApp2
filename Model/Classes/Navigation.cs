using System.Windows.Controls;
using Wpf.Ui.Controls;

namespace TestApp.Model.Classes
{
    public static class Navigation
    {
        public static Frame? AdminFrame { get; set; }
        public static Frame? ContainerFrame { get; set; }
        public static int Usertype { get; set; }
        public static int UserID { get; set; }
        public static int AdminNavType { get; set; }
        public static string AdminType { get; set; } = string.Empty;
        private static View.Snackbar _snackbar;
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