using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using TestApp.Model;

namespace TestApp.View.Test.AddTest
{
    public partial class AnswerAdd : UserControl
    {
        public AnswerAdd()
        {
            InitializeComponent();
            AddMultipleAnswerInputs();
        }

        private void AddMultipleAnswerInputs()
        {
            var answerPanel = CreateAnswerPanel();
            AnswersPanel.Children.Add(answerPanel);
            UpdateAnswerPanels();
        }

        private StackPanel CreateAnswerPanel()
        {
            var answerPanel = new StackPanel { Orientation = Orientation.Horizontal };
            var answerTextBox = new Wpf.Ui.Controls.TextBox { Width = 200, PlaceholderText = "Введите вариант ответа" };
            var checkBox = new CheckBox { Content = "Правильный ответ", IsChecked = true, Visibility = Visibility.Collapsed };
            var deleteButton = new Button { Content = "Удалить", Margin = new Thickness(5) };

            deleteButton.Click += (s, e) => DeleteAnswerPanel(answerPanel);

            answerPanel.Children.Add(answerTextBox);
            answerPanel.Children.Add(checkBox);
            answerPanel.Children.Add(deleteButton);

            return answerPanel;
        }

        public Question GetQuestion()
        {
            var question = new Question
            {
                QuestionText = QuestionTextBox.Text,
                Answers = []
            };

            foreach (var child in AnswersPanel.Children)
            {
                if (child is StackPanel answerPanel)
                {
                    var answer = CreateAnswerFromPanel(answerPanel);
                    question.Answers.Add(answer);
                }
            }

            return question;
        }

        private Answer CreateAnswerFromPanel(StackPanel answerPanel)
        {
            var answerTextBox = answerPanel.Children[0] as TextBox;
            var checkBox = answerPanel.Children[1] as CheckBox;

            return new Answer
            {
                AnswerText = answerTextBox?.Text,
                IsCorrect = checkBox.Visibility == Visibility.Visible ? checkBox.IsChecked == true : false
            };
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var answerPanel = CreateAnswerPanel();
            AnswersPanel.Children.Add(answerPanel);
            UpdateAnswerPanels();
        }

        private void UpdateAnswerPanels()
        {
            bool showCheckBox = AnswersPanel.Children.Count != 1;

            foreach (var child in AnswersPanel.Children)
            {
                if (child is StackPanel answerPanel)
                {
                    (answerPanel.Children[1] as CheckBox).Visibility = showCheckBox ? Visibility.Visible : Visibility.Collapsed;
                    if (!showCheckBox)
                        (answerPanel.Children[1] as CheckBox).IsChecked = true;
                }
            }
        }

        private void DeleteAnswerPanel(StackPanel answerPanel)
        {
            AnswersPanel.Children.Remove(answerPanel); 
            UpdateAnswerPanels();
        }
    }
}