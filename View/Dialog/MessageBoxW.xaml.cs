using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Animation;
using TestApp.Model.Classes;

namespace TestApp.View
{
    /// <summary>
    /// Логика взаимодействия для MessageBoxW.xaml
    /// </summary>
    public partial class MessageBoxW 
    {
        public string ReturnString { get; set; } //Переменная отвечающая за возврат выбора варианта
        public MessageBoxW(string Text, string Title, MessageBoxs.Buttons buttons, MessageBoxs.Icon icon)
        {
            InitializeComponent();
            txtText.Text = Text;
            txtTitle.Text = Title;
            switch (buttons)
            {
                case MessageBoxs.Buttons.OK: btnOK.Visibility = Visibility.Visible; break;
                case MessageBoxs.Buttons.YesNo: btnYes.Visibility = Visibility.Visible; btnNo.Visibility = Visibility.Visible; break;
            }
            switch (icon)
            {
                case MessageBoxs.Icon.Info: Info.Visibility = Visibility.Visible; System.Media.SystemSounds.Exclamation.Play(); break;
                case MessageBoxs.Icon.Error: Error.Visibility = Visibility.Visible; System.Media.SystemSounds.Hand.Play(); break;
                case MessageBoxs.Icon.Default: break;
            }
        }
        private void GBar_MouseDown(object sender, MouseButtonEventArgs e) //Данный метод отвечает за перетаскивание окна
        { try { DragMove(); } catch { } }
        private DoubleAnimation anim; //Анимация отображения окна
        private void Window_Closing(object? sender, System.ComponentModel.CancelEventArgs e)
        {
            Closing -= Window_Closing;
            e.Cancel = true;
            anim = new DoubleAnimation(0, (Duration)TimeSpan.FromSeconds(0.3));
            anim.Completed += (s, _) => this.Close();
            BeginAnimation(UIElement.OpacityProperty, anim);
        }
        private void BtnReturnValue_Click(object sender, RoutedEventArgs e) { ReturnString = ((Button)sender).Uid.ToString(); Close(); }
        private void CopyItem_Click(object sender, RoutedEventArgs e) => Clipboard.SetText(txtText.Text); //Копирование содержимого диалогового окна
    }
}
