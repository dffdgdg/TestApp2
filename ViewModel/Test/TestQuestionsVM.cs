using CommunityToolkit.Mvvm.Input;
using System.Windows;
using TestApp.Model;
using System.Windows.Controls;
using TestApp.View;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Windows.Media.Imaging;
using TestApp.Services;

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
        private BitmapImage _questionImage;
        public BitmapImage QuestionImage
        {
            get => _questionImage;
            set => SetProperty(ref _questionImage, value);
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
            _questions = [.. _db.Questions.Where(u=> u.Test == testId)];
            NextCommand = new RelayCommand(NextQuestion);
            _stopwatch = new Stopwatch(); 
            _stopwatch.Start();
            LoadQuestion();
        }
        private static BitmapImage ByteArrayToBitmapImage(byte[] byteArray)
        {
            using var ms = new System.IO.MemoryStream(byteArray);
            var image = new BitmapImage();
            image.BeginInit();
            image.CacheOption = BitmapCacheOption.OnLoad;
            image.StreamSource = ms;
            image.EndInit();
            return image;
        }
        private void LoadQuestion()
        {
            if (_currentQuestionIndex < _questions.Count)
            {
                var currentQuestion = _questions[_currentQuestionIndex];
                QuestionText = currentQuestion.Name;

                if (currentQuestion.Image != null && currentQuestion.Image.Length > 0)
                {
                    QuestionImage = ByteArrayToBitmapImage(currentQuestion.Image);
                }
                else
                {
                    QuestionImage = null;
                }

                QuestionElements = [];
                switch (currentQuestion.Type)
                {
                    case 1:
                        var textBox = new TextBox();
                        QuestionElements.Add(textBox);
                        break;

                    case 2:
                        foreach (var answer in _db.Answers.Where(u => u.Question == currentQuestion.Id))
                        {
                            var radioButton = new RadioButton { Content = answer.Name, GroupName = "Answers" };
                            QuestionElements.Add(radioButton);
                        }
                        break;

                    case 3:
                        foreach (var answer in _db.Answers.Where(u => u.Question == currentQuestion.Id))
                        {
                            var checkBox = new CheckBox { Content = answer.Name};
                            QuestionElements.Add(checkBox);
                        }
                        break;

                    default:
                        foreach (var answer in _db.Answers.Where(u => u.Question == currentQuestion.Id))
                        {
                            var radioButton = new RadioButton { Content = answer.Name, GroupName = "Answers" };
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
                        Test = testId,
                        User = userId,
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
                    ShowError($"Ошибка при сохранении данных: {innerExceptionMessage}");
                } 
            }
        }

        private void NextQuestion()
        {
            if (_currentQuestionIndex < _questions.Count)
            {
                var currentQuestion = _questions[_currentQuestionIndex];

                if (!IsAnswerSelected(currentQuestion))
                {
                    ShowMessage("Пожалуйста, выберите ответ перед тем, как перейти к следующему вопросу.");
                    return;
                }

                score += CalculateScore(currentQuestion);
                _currentQuestionIndex++;
                ShowMessage(score.ToString());
                LoadQuestion();
            }
        }

        private bool IsAnswerSelected(Question question)
        {
            switch (question.Type)
            {
                case 1: 
                    var textBox = QuestionElements.OfType<TextBox>().FirstOrDefault();
                    return !string.IsNullOrWhiteSpace(textBox?.Text);

                case 2: 
                    return QuestionElements.OfType<RadioButton>().Any(rb => rb.IsChecked == true);

                case 3:
                    return QuestionElements.OfType<CheckBox>().Any(cb => cb.IsChecked == true);

                default:
                    return false;
            }
        }

        private double CalculateScore(Question currentQuestion)
        {
            double scores = 0;

            var correctAnswers = _db.Answers.Where(a => a.Question == currentQuestion.Id && a.IsCorrect == true).ToList();

            switch (currentQuestion.Type)
            {
                case 2:
                    var selectedRadioButton = QuestionElements.OfType<RadioButton>().FirstOrDefault(rb => rb.IsChecked == true);
                    if (selectedRadioButton != null)
                    {
                        var selectedAnswerText = selectedRadioButton.Content.ToString();
                        scores = correctAnswers.Any(a => a.Name == selectedAnswerText) ? 1 : 0;
                    }
                    break;

                case 3:
                    var selectedCheckBoxes = QuestionElements.OfType<CheckBox>().Where(cb => cb.IsChecked == true).ToList();
                    int totalCorrectAnswers = correctAnswers.Count;
                    int selectedCorrectCount = selectedCheckBoxes.Count(cb => correctAnswers.Any(a => a.Name == cb.Content.ToString()));

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

                case 1:
                    var textBox = QuestionElements.OfType<TextBox>().FirstOrDefault();
                    if (textBox != null)
                    {
                        var userAnswerText = textBox.Text;
                        scores = correctAnswers.Any(a => a.Name == userAnswerText) ? 1 : 0;
                    }
                    break;

                default:
                    ShowMessage("Неизвестный тип вопроса.");
                    break;
            }

            return scores;
        }
    }
}