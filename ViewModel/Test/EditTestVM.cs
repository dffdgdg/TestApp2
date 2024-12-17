using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using TestApp.Model;
using TestApp.Services;
using TestApp.View.Test.AddTest;

namespace TestApp.ViewModel
{
    public class EditTestVM : BaseViewModel
    {
        private TestDbContext db;
        private Test _test;
        private Question selectedItem, currentQuestion;

        public Test Test
        {
            get => _test;
            set => SetProperty(ref _test, value);
        }

        public Question SelectedItem
        {
            get => selectedItem;
            set => SetProperty(ref selectedItem, value);
        }

        public Question CurrentQuestion
        {
            get => currentQuestion;
            set => SetProperty(ref currentQuestion, value);
        }
        private ObservableCollection<Question> questions;
        public ObservableCollection<Question> Questions
        {
            get => questions;
            set => SetProperty(ref questions, value);
        }
        public RelayCommand Open { get; }
        public RelayCommand Add { get; }
        public RelayCommand Save { get; }
        public RelayCommand<Question> Edit { get; }
        public RelayCommand<Question> Remove { get; }

        public EditTestVM(Test test, INavigationService service)
        {
            this.service = service;
            db = new TestDbContext();
            Test = test;
            Add = new RelayCommand(AddItem);
            Save = new RelayCommand(SaveItem);
            Edit = new RelayCommand<Question>(EditItem);
            Remove = new RelayCommand<Question>(RemoveItem);
            Questions = new ObservableCollection<Question>([.. db.Questions.Where(u => u.Test == Test.Id).OrderBy(u => u.Name)]);
        }
        private void SaveItem()
        {
            using var db = new TestDbContext();
            db.Tests.Update(Test);
            db.SaveChanges();
            ShowPopup("Сохранение прошло успешно");
        }

        protected void RemoveItem(Question item)
        {
            if (item == null) return;
            if (ShowDialog("Вы точно хотите удалить вопрос?", "Подтверждение удаления"))
            {
                db.Questions.Remove(item);
                db.SaveChanges();
                Questions.Remove(item);
                Navigation.Show("Удаление прошло успешно");
            }
        }

        protected void AddItem()
        {
            CurrentQuestion = new Question { Test = Test.Id };
            QuestionVM vm = new(CurrentQuestion, service, Test);
            service.Navigate(typeof(QuestionAnswersPage), vm);
        }

        protected void EditItem(Question item)
        {
            if (item == null) return;
            CurrentQuestion = item;
            QuestionVM vm = new(item, service, Test);
            service.Navigate(typeof(QuestionAnswersPage), vm);
        }
    }
}