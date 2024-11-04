using CommunityToolkit.Mvvm.Input;
using System.Windows;
using TestApp.Model;
using System.Windows.Controls;
using TestApp.View;
using Microsoft.EntityFrameworkCore;
using TestApp.Model.Classes;
using System.Diagnostics;
using TestApp.Helpers;

namespace TestApp.ViewModel
{
    public class TestQuestionsVM : BaseViewModel
    {
        private readonly List<Question> _questions;
        private int _currentQuestionIndex;
        private readonly TestDbContext _db;
        private readonly Stopwatch _stopwatch;

        private string _questionText;
        public string QuestionText
        {
            get => _questionText;
            set => SetProperty(ref _questionText, value);
        }

        public TimeSpan TimeTaken => _stopwatch.Elapsed;
        public List<UIElement> QuestionElements { get; private set; }

        public RelayCommand NextCommand { get; }
        private readonly int testId;
        private double score;
        public TestQuestionsVM(Test test, int userId, INavigationService service)
        {
            this.service = service;
            this.userId = userId;
            score = 0;
            testId = test.Id;
            _db = new TestDbContext();
            _questions = [.. _db.Questions.Where(u=> u.TestId == testId)];
            NextCommand = new RelayCommand(NextQuestion);
            _stopwatch = new Stopwatch(); 
            _stopwatch.Start();
            LoadQuestion();
        }

        private void LoadQuestion()
        {
            if (_currentQuestionIndex < _questions.Count)
            {
                var currentQuestion = _questions[_currentQuestionIndex];
                QuestionText = currentQuestion.QuestionText;

                QuestionElements = [];
                switch (currentQuestion.QuestionType)
                {
                    case "choise":
                        var textBox = new TextBox();
                        QuestionElements.Add(textBox);
                        break;

                    case "text":
                        foreach (var answer in _db.Answers.Where(u => u.QuestionId == currentQuestion.Id))
                        {
                            var radioButton = new RadioButton { Content = answer.AnswerText, GroupName = "Answers" };
                            QuestionElements.Add(radioButton);
                        }
                        break;

                    case "multiplechoise":
                        foreach (var answer in _db.Answers.Where(u => u.QuestionId == currentQuestion.Id))
                        {
                            var checkBox = new CheckBox { Content = answer.AnswerText };
                            QuestionElements.Add(checkBox);
                        }
                        break;

                    default:
                        foreach (var answer in _db.Answers.Where(u => u.QuestionId == currentQuestion.Id))
                        {
                            var radioButton = new RadioButton { Content = answer.AnswerText, GroupName = "Answers" };
                            QuestionElements.Add(radioButton);
                        }
                        break;
                }

                OnPropertyChanged(nameof(QuestionElements));
            }
            else
            {
                try
                {
                    _stopwatch.Stop();
                    Model.Userresult result = new()
                    {
                        Testid = testId,
                        Userid = userId,
                        Score = _questions.Count > 0 ? (score / (double)_questions.Count) * 100 : 0
                    };
                    _db.Userresults.Add(result);
                    _db.SaveChanges();
                    TestResultVM vm = new(_questions.Count, result, DateTime.Now, TimeTaken);
                    service.Navigate(typeof(TestResultPage), vm);
                }
                catch (DbUpdateException dbEx)
                {
                    var innerExceptionMessage = dbEx.InnerException != null ? dbEx.InnerException.Message : "Неизвестная ошибка.";
                    MessageBoxs.Show($"Ошибка при сохранении данных: {innerExceptionMessage}");
                } 
            }
        }

        private void NextQuestion()
        {
            if (_currentQuestionIndex < _questions.Count)
            {
                var currentQuestion = _questions[_currentQuestionIndex];
                score += CalculateScore(currentQuestion);
                _currentQuestionIndex++;
                LoadQuestion();
            }
        }

        private double CalculateScore(Question currentQuestion)
        {
            double scores = 0;

            var correctAnswers = _db.Answers.Where(a => a.QuestionId == currentQuestion.Id && a.IsCorrect == true).ToList();

            switch (currentQuestion.QuestionType)
            {
                case "text":
                    var selectedRadioButton = QuestionElements.OfType<RadioButton>().FirstOrDefault(rb => rb.IsChecked == true);
                    if (selectedRadioButton != null)
                    {
                        var selectedAnswerText = selectedRadioButton.Content.ToString();
                        scores = correctAnswers.Any(a => a.AnswerText == selectedAnswerText) ? 1 : 0;
                    }
                    break;

                case "multiplechoise":
                    var selectedCheckBoxes = QuestionElements.OfType<CheckBox>().Where(cb => cb.IsChecked == true).ToList();
                    int totalCorrectAnswers = correctAnswers.Count;
                    int selectedCorrectCount = selectedCheckBoxes.Count(cb => correctAnswers.Any(a => a.AnswerText == cb.Content.ToString()));

                    if (selectedCorrectCount == totalCorrectAnswers)
                        scores = 1.0;
                    else if (selectedCorrectCount > 0)
                        scores = (double)selectedCorrectCount / totalCorrectAnswers;
                    else
                        scores = 0;

                    int totalSelectedCount = selectedCheckBoxes.Count;
                    if (totalSelectedCount > totalCorrectAnswers)
                        scores -= (totalSelectedCount - selectedCorrectCount) / (double)QuestionElements.OfType<CheckBox>().Count();
                    break;

                case "choise":
                    var textBox = QuestionElements.OfType<TextBox>().FirstOrDefault();
                    if (textBox != null)
                    {
                        var userAnswerText = textBox.Text;
                        scores = correctAnswers.Any(a => a.AnswerText == userAnswerText) ? 1 : 0;
                    }
                    break;

                default:
                    MessageBox.Show("Неизвестный тип вопроса.");
                    break;
            }

            return scores;
        }
    }
}