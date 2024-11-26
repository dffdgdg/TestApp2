using Microsoft.Xaml.Behaviors;
using System.Windows;

namespace TestApp.Behaviors
{
    public class PasswordBehavior : Behavior<Wpf.Ui.Controls.PasswordBox>
    {
        public static readonly DependencyProperty PasswordProperty =
        DependencyProperty.Register("Password", typeof(string), typeof(PasswordBehavior), new PropertyMetadata(default(string)));
        // Определяем свойство зависимости Password, которое связывается с паролем в ViewModel
        private bool _skipUpdate; // Флаг для предотвращения бесконечной рекурсии при обновлении свойства Password
        public string Password // Определяем свойство Password, которое связывается с паролем в ViewModel
        { get { return (string)GetValue(PasswordProperty); } set { SetValue(PasswordProperty, value); } }
        protected override void OnAttached() => // Подписываемся на событие PasswordChanged элемента PasswordBox, чтобы обновлять свойство Password в ViewModel при изменении пароля
            AssociatedObject.PasswordChanged += PasswordBox_PasswordChanged;
        protected override void OnDetaching() =>  // Отписываемся от события PasswordChanged при удалении поведения
            AssociatedObject.PasswordChanged -= PasswordBox_PasswordChanged;
        protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e) // Обновляем значение пароля в элементе PasswordBox при изменении свойства Password в ViewModel
        {
            base.OnPropertyChanged(e);
            if (e.Property == PasswordProperty && !_skipUpdate && AssociatedObject != null)
            {
                _skipUpdate = true;
                AssociatedObject.Password = (string)e.NewValue;
                _skipUpdate = false;
            }
        }
        private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e) // Обновляем значение свойства Password в ViewModel при изменении пароля в элементе PasswordBox
        {
            _skipUpdate = true;
            Password = AssociatedObject.Password;
            _skipUpdate = false;
        }
    }
}
