using Microsoft.Xaml.Behaviors;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
namespace TestApp.Helpers
{
    class NumericOnlyBehavior : Behavior<TextBox>
    {
        private const string NumericRegexPattern = @"^[0-9]+$"; // шаблон для проверки на числа
        protected override void OnAttached()
        {
            base.OnAttached();
            AssociatedObject.PreviewTextInput += OnPreviewTextInput; // подписываемся на событие ввода текста
            DataObject.AddPastingHandler(AssociatedObject, OnPasting); // подписываемся на событие вставки текста
        }
        private static void OnPasting(object sender, DataObjectPastingEventArgs e)
        {
            if (e.SourceDataObject.GetDataPresent(DataFormats.Text, true))
            {
                var pastedText = e.SourceDataObject.GetData(DataFormats.Text) as string;
                if (!IsTextNumeric(pastedText)) // если вставляемый текст не содержит только числа
                    e.CancelCommand(); // отменяем вставку
            }
            else e.CancelCommand(); // отменяем вставку
        }
        private static void OnPreviewTextInput(object sender, System.Windows.Input.TextCompositionEventArgs e)
        { e.Handled = !IsTextNumeric(e.Text); }// если вводимый текст не содержит только числа, то отменяем ввод
        private static bool IsTextNumeric(string input)
        { return Regex.IsMatch(input, NumericRegexPattern); } // проверяем, содержит ли текст только числа
        protected override void OnDetaching()
        {
            base.OnDetaching();
            AssociatedObject.PreviewTextInput -= OnPreviewTextInput; // отписываемся от события ввода текста
            DataObject.RemovePastingHandler(AssociatedObject, OnPasting); // отписываемся от события вставки текста
        }
    }
    public class RussianLettersOnlyBehavior : Behavior<TextBox>
    {
        private const string RussianLettersRegexPattern = @"^[а-яА-Я]+$"; // шаблон для проверки на русские буквы
        protected override void OnAttached()
        {
            base.OnAttached();
            AssociatedObject.PreviewTextInput += OnPreviewTextInput; // подписываемся на событие ввода текста
            DataObject.AddPastingHandler(AssociatedObject, OnPasting); // подписываемся на событие вставки текста
        }
        private static void OnPasting(object sender, DataObjectPastingEventArgs e)
        {
            if (e.SourceDataObject.GetDataPresent(DataFormats.Text, true))
            {
                var pastedText = e.SourceDataObject.GetData(DataFormats.Text) as string;
                if (!IsTextRussian(pastedText)) // если вставляемый текст не содержит только русские буквы
                    e.CancelCommand(); // отменяем вставку
            }
            else e.CancelCommand(); // отменяем вставку
        }
        private static void OnPreviewTextInput(object sender, System.Windows.Input.TextCompositionEventArgs e) =>
            e.Handled = !IsTextRussian(e.Text); // если вводимый текст не содержит только русские буквы, то отменяем ввод
        private static bool IsTextRussian(string input)
        { return Regex.IsMatch(input, RussianLettersRegexPattern); } // проверяем, содержит ли текст только русские буквы
        protected override void OnDetaching()
        {
            base.OnDetaching();
            AssociatedObject.PreviewTextInput -= OnPreviewTextInput; // отписываемся от события ввода текста
            DataObject.RemovePastingHandler(AssociatedObject, OnPasting); // отписываемся от события вставки текста
        }
    }
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