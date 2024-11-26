using CommunityToolkit.Mvvm.Input;
using Microsoft.Win32;
using System.Collections.ObjectModel;
using System.IO;
using TestApp.Model;
using TestApp.Services;
using TestApp.View.Test.AddTest;

namespace TestApp.ViewModel;
public class QuestionVM : BaseViewModel
{
    private readonly TestDbContext db;
    private Question? currentQuestion;
    public Question? CurrentQuestion
    {
        get => currentQuestion;
        set => SetProperty(ref currentQuestion, value);
    }
    public ObservableCollection<Answer> Answers { get; }
    public RelayCommand Add { get; }
    public RelayCommand<Answer> Remove { get; }
    public RelayCommand Save { get; }
    public RelayCommand LoadImage { get; }
    private Question question;
    private Test test;

    public QuestionVM(Question question, INavigationService service, Test test)
    {
        this.test = test;
        this.question = question;
        this.service = service;
        db = new TestDbContext();
        CurrentQuestion = question;
        Answers = new ObservableCollection<Answer>([.. db.Answers.Where(u => u.Question == question.Id)]);
        Add = new RelayCommand(AddItem);
        Remove = new RelayCommand<Answer>(RemoveItem);
        Save = new RelayCommand(SaveItem);
        LoadImage = new RelayCommand(LoadItemImage);
        Answers.CollectionChanged += (s, e) => OnPropertyChanged(nameof(AnswersCount));
    }
    public int AnswersCount => Answers.Count;

    private void AddItem()
    {
        Answer answer = new() { Question = question.Id, IsCorrect = true };
        Answers.Add(answer);
    }

    private void RemoveItem(Answer item)
    {
        if (item == null) return;
        if (ShowDialog("Вы точно хотите удалить этот вариант ответа?", "Подтверждение удаления"))
        {
            Answers.Remove(item);
            if (db.Answers.Any(u => u.Id == item.Id))
            {
                db.Answers.Remove(item);
                db.SaveChanges();
            }
            ShowPopup("Удаление произошло успешно");
        }
    }
    private void DetermineQuestionType()
    {
        int answersCount = Answers.Count;
        int correctAnswersCount = Answers.Count(a => (bool)a.IsCorrect);

        if (answersCount == 1) CurrentQuestion.Type = 1;
        else if (answersCount >= 2)
        {
            if (correctAnswersCount == 1) CurrentQuestion.Type = 2;
            else if (correctAnswersCount > 1) CurrentQuestion.Type = 3;
            else throw new InvalidOperationException("Должен быть хотя бы один правильный ответ");
        }
        else throw new InvalidOperationException("Необходимо добавить хотя бы один вариант ответа");
    }
    private void LoadItemImage()
    {
        var openFileDialog = new OpenFileDialog
        {
            Filter = "Image Files (*.jpg;*.jpeg;*.png;*.bmp)|*.jpg;*.jpeg;*.png;*.bmp",
            Title = "Выберите изображение"
        };

        if (openFileDialog.ShowDialog() == true) CurrentQuestion.Image = File.ReadAllBytes(openFileDialog.FileName);
    }
    private void SaveItem()
    {
        if (CurrentQuestion != null)
        {
            if (Answers.Count == 0)
            {
                ShowError("Необходимо добавить хотя бы один вариант ответа");
                return;
            }
            DetermineQuestionType();
            using var transaction = db.Database.BeginTransaction();
            try
            {
                if (CurrentQuestion.Id == 0) db.Questions.Add(CurrentQuestion);
                else db.Questions.Update(CurrentQuestion);

                db.SaveChanges();

                var existingAnswerIds = db.Answers.Where(a => a.Question == CurrentQuestion.Id).Select(a => a.Id).ToList();

                foreach (var answer in Answers)
                {
                    if (answer.Id == 0)
                    {
                        answer.Question = CurrentQuestion.Id;
                        db.Answers.Add(answer);
                    }
                    else db.Answers.Update(answer);
                    existingAnswerIds.Remove(answer.Id);
                }

                foreach (var answerId in existingAnswerIds)
                {
                    var answerToRemove = db.Answers.Find(answerId);
                    if (answerToRemove != null)
                        db.Answers.Remove(answerToRemove);
                }

                db.SaveChanges();
                transaction.Commit();
                ShowPopup("Сохранение прошло успешно");
                EditTestVM vm = new(test, service);
                service.Navigate(typeof(EditTestPage), vm);
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                ShowError($"Произошла ошибка при сохранении: {ex.Message}");
            }
        }
    }
}