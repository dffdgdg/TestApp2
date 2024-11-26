using Microsoft.Xaml.Behaviors;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;

namespace TestApp.Behaviors
{
    public class NumericOnlyBehavior : Behavior<TextBox>
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
}
