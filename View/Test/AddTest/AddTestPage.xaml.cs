using System;
using System.Windows;
using System.Windows.Controls;
using TestApp.Model;
using TestApp.View.Test.AddTest;

namespace TestApp.View
{
    /// <summary>
    /// Логика взаимодействия для AddTestPage.xaml
    /// </summary>
    public partial class AddTestPage : Page
    {
        public AddTestPage()
        {
            InitializeComponent();
        }

        private void AddQuestion_Click(object sender, RoutedEventArgs e)
        {
            var questionControl = new AnswerAdd();
            QuestionsList.Items.Add(questionControl);
        }

        private void SaveTest_Click(object sender, RoutedEventArgs e)
        {
            var newTest = new Model.Test
            {
                Title = TestTitle.Text,
                Description = TestDescription.Text,
                CreatedAt = DateTime.Now
            };

            if (IsTestValid(newTest))
            {
                foreach (AnswerAdd questionControl in QuestionsList.Items)
                {
                    var question = questionControl.GetQuestion();
                    if (question.Answers.Count == 0)
                    {
                        MessageBox.Show("Каждый вопрос должен иметь хотя бы один вариант ответа.");
                        return;
                    }

                    question.QuestionType = DetermineQuestionType(question);
                    newTest.Questions.Add(question);
                }

                SaveTestToDatabase(newTest);
                MessageBox.Show("Тест успешно сохранен!");
            }
        }

        private string DetermineQuestionType(Question question)
        {
            int answerCount = question.Answers.Count;
            if (answerCount == 1)
            {
                return "Text";
            }

            int correctAnswerCount = 0;
            foreach (var answer in question.Answers)
            {
                if ((bool)answer.IsCorrect)
                {
                    correctAnswerCount++;
                }
            }

            return correctAnswerCount == 1 ? "SingleChoice" : "MultipleChoice";
        }

        private bool IsTestValid(Model.Test test)
        {
            if (string.IsNullOrWhiteSpace(test.Title) ||
                string.IsNullOrWhiteSpace(test.Description) ||
                QuestionsList.Items.Count == 0)
            {
                MessageBox.Show("Пожалуйста, заполните все поля и добавьте хотя бы один вопрос.");
                return false;
            }
            return true;
        }

        private void SaveTestToDatabase(Model.Test test)
        {
            using var context = new TestDbContext();
            context.Tests.Add(test);
            context.SaveChanges();
        }

        private void DeleteQuestion_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button)
            {
                var questionPanel = (StackPanel)button.Parent;
                var listItem = QuestionsList.ItemContainerGenerator.ContainerFromItem(questionPanel.DataContext);
                if (listItem != null) QuestionsList.Items.Remove(questionPanel.DataContext);
            }
        }
    }
}