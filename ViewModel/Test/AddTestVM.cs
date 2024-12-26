using CommunityToolkit.Mvvm.Input;
using Microsoft.EntityFrameworkCore;
using System.Collections.ObjectModel;
using System.Windows.Documents;
using System.Windows.Input;
using TestApp.Model;
using TestApp.Services;
using TestApp.ViewModel.Base;

namespace TestApp.ViewModel;

public class AddTestViewModel : BaseEditsModel<Test>
{
    private Test test;
    public Test Test
    {
        get => test;
        set => SetProperty(ref test, value);
    }

    public ObservableCollection<Question> Questions { get; set; }

    private Question _newQuestion;
    public Question NewQuestion
    {
        get => _newQuestion;
        set => SetProperty(ref _newQuestion, value);
    }

    private Answer _newAnswer;
    public Answer NewAnswer
    {
        get => _newAnswer;
        set => SetProperty(ref _newAnswer, value);
    }

    private Answer _selectedAnswer;
    public Answer SelectedAnswer
    {
        get => _selectedAnswer;
        set
        {
            _selectedAnswer = value;
            OnPropertyChanged(nameof(SelectedAnswer));
        }
    }

    private Question _selectedQuestion;
    public Question SelectedQuestion
    {
        get => _selectedQuestion;
        set
        {
            _selectedQuestion = value;
            OnPropertyChanged(nameof(SelectedQuestion));
            OnPropertyChanged(nameof(SelectedQuestionAnswers));
        }
    }

    public ObservableCollection<Answer> SelectedQuestionAnswers
    {
        get
        {
            if (SelectedQuestion != null)
                return new ObservableCollection<Answer>(SelectedQuestion.Answers);
            return [];
        }
    }
    public ICommand AddQuestionCommand { get; set; }
    public ICommand SaveTestCommand { get; set; }

    public AddTestViewModel(int district, INavigationService service)
    {
        this.service = service;
        Questions = [];
        Test = new Test() { District = district };
        NewQuestion = new Question();
        NewAnswer = new Answer();
        AddQuestionCommand = new RelayCommand(AddQuestion);
        AddQuestion();
        SaveTestCommand = new RelayCommand(OnSave);
    }

    private void AddQuestion()
    {
        var question = new Question
        {
            Name = NewQuestion.Name,
            Test = Test.Id,
            Answers = new ObservableCollection<Answer>() // Инициализация пустого списка ответов
        };

        Questions.Add(question);
        NewQuestion = new Question(); // Сбрасываем новый вопрос
    }

    private ICommand _addAnswerCommand;
    public ICommand AddAnswerCommand => _addAnswerCommand ??= new RelayCommand<Question>(AddAnswer);

    private void AddAnswer(Question question)
    {
        if (question != null)
        {
            var newAnswer = new Answer() { Question = question.Id};
            question.Answers.Add(newAnswer);
        }
    }
    private ICommand _removeAnswerCommand;
    public ICommand RemoveAnswerCommand => _removeAnswerCommand ??= new RelayCommand<Answer>(RemoveAnswer);

    private void RemoveAnswer(Answer answer)
    {
        if (answer != null)
        {
            var question = Questions.FirstOrDefault(q => q.Answers.Contains(answer));
            question?.Answers.Remove(answer);
        }
    }
    private ICommand _removeQuestionCommand;
    public ICommand RemoveQuestionCommand => _removeQuestionCommand ??= new RelayCommand<Question>(RemoveQuestion);

    public override FlowDocument DescriptionDocument { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

    protected override string Description => throw new NotImplementedException();

    private void RemoveQuestion(Question question) => Questions.Remove(question);

    protected override string? Validate()
    {
        if (string.IsNullOrWhiteSpace(Test?.Title))
            return "Введите наименование теста!";

        if (string.IsNullOrWhiteSpace(Test?.Description))
            return "Введите описание теста!";

        foreach (var question in Questions)
        {
            if (question.Answers == null || question.Answers.Count == 0)
                return $"Вопрос '{question.Name}' должен иметь хотя бы один вариант ответа.";
            if (!question.Answers.Any(a => a.IsCorrect))
                return $"Вопрос '{question.Name}' должен иметь хотя бы один правильный ответ.";
        }

        return null;
    }

    protected override void OnSave()
    {
        using var context = new TestDbContext();
        try
        {
            string? errorMessage = Validate();
            if (errorMessage != null) { ShowError(errorMessage); return; }
            context.Tests.Add(Test);
            context.SaveChanges();
            foreach (var question in Questions)
            {
                question.Test = Test.Id;
                DetermineQuestionType(question);
                context.Questions.Add(question);
            }
            context.SaveChanges();
            foreach (var question in Questions)
            {
                foreach (var answer in question.Answers)
                {
                    answer.Question = question.Id;
                    context.Answers.Add(answer);
                }
            }
            context.SaveChanges();
            NotifyItemUpdated();
            ShowPopup("Добавление прошло успешно!");
            service.GoBack();
        }
        catch (DbUpdateException dbEx)
        {
            var innerExceptionMessage = dbEx.InnerException?.Message ?? "Неизвестная ошибка.";
            ShowError($"Ошибка при сохранении данных: {innerExceptionMessage}");
        }
        catch (Exception ex)
        {
            ShowError($"Ошибка: {ex.Message}");
        }
    }

    private void DetermineQuestionType(Question question)
    {
        int answersCount = question.Answers.Count;
        int correctAnswersCount = question.Answers.Count(a => a.IsCorrect);

        if (answersCount == 1) question.Type = 1; // Тип 1: Один вариант ответа
        else if (answersCount >= 2)
        {
            if (correctAnswersCount == 1) question.Type = 2; // Тип 2: Несколько вариантов, один правильный
            else if (correctAnswersCount > 1) question.Type = 3; // Тип 3: Несколько вариантов, несколько правильных
            else throw new InvalidOperationException($"Вопрос '{question.Name}' должен иметь хотя бы один правильный ответ.");
        }
        else
            throw new InvalidOperationException($"Вопрос '{question.Name}' должен иметь хотя бы один вариант ответа.");
    }

    protected override void OnLoadImage()
    {
        throw new NotImplementedException();
    }

    protected override void SetImage(string filePath)
    {
        throw new NotImplementedException();
    }
}