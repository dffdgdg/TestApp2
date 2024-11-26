using Microsoft.Xaml.Behaviors;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;

namespace TestApp.Behaviors
{
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

}
