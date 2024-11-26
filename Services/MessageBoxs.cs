using System.Windows.Media.Effects;
using System.Windows;
using TestApp.View;

namespace TestApp.Services
{
    public partial class MessageBoxs
    {
        // Перечисления для типов кнопок и иконок в MessageBox
        public enum Buttons { YesNo, OK }
        public enum Icon { Error, Info, Default, Question }
        // Метод для отображения MessageBox
        public static string Show(string text, string title = "", Buttons buttons = Buttons.OK, Icon icon = Icon.Default)
        {
            var messageBox = new MessageBoxW(text, title, buttons, icon); // Создание нового MessageBox
            messageBox.Show(); // Отображение MessageBox
            return messageBox.ReturnString; // Возврат результата
        }
        // Метод для отображения модального MessageBox
        public static bool ShowDialog(string text, string title = "", Buttons buttons = Buttons.OK, Icon icon = Icon.Question)
        {
            ShowBlurEffectAllWindow(); // Включение эффекта размытия
            var messageBox = new MessageBoxW(text, title, buttons, icon); // Создание нового MessageBox
            messageBox.ShowDialog(); // Отображение модального MessageBox
            StopBlurEffectAllWindow(); // Отключение эффекта размытия
            return messageBox.ReturnString == "1"; // Предполагаем, что "Yes" возвращается в случае выбора "Yes"
        }
        // Эффект размытия для окон
        static readonly BlurEffect MyBlur = new() { Radius = 15 };
        // Метод для включения эффекта размытия отображении MessageBox
        public static void ShowBlurEffectAllWindow() { foreach (Window window in Application.Current.Windows) window.Effect = MyBlur; }
        // Метод для отключения эффекта размытия
        public static void StopBlurEffectAllWindow() { foreach (Window window in Application.Current.Windows) window.Effect = null; }
    }
}
