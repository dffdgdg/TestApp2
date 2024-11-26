using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Documents;
using TestApp.Model;
using TestApp.Services;
using TestApp.View;
using TestApp.View.HistoricalSite;
using TestApp.View.Test;
using TestApp.View.Test.AddTest;
using TestApp.ViewModel.Base;

namespace TestApp.ViewModel
{
    public class DistrictDetailVM : BaseUserControlViewModel
    {
        private int id;
        private readonly TestDbContext db;

        public byte[] Image { get; set; }
        public string Name { get; set; }
        public string Foundating_date { get; set; }
        private HashSet<int> completedTests;

        // Команды для исторических достопримечательностей
        public RelayCommand LoadImage { get; }
        public RelayCommand<HistoricalSite> OpenSite { get; }
        public RelayCommand AddSite { get; }
        public RelayCommand<HistoricalSite> RemoveSite { get; }
        public RelayCommand<HistoricalSite> EditSite { get; }
        public RelayCommand Save { get; }
        public RelayCommand Close { get; }
        public ObservableCollection<HistoricalSite> HistoricalSites { get; private set; }
        private HistoricalSite curSite;

        public HistoricalSite CurSite
        {
            get => curSite;
            set => SetProperty(ref curSite, value);
        }
        private string description;
        public string Description
        {
            get => description;
            set
            {
                if (SetProperty(ref description, value))
                {
                    UpdateDescriptionDocument();
                }
            }
        }

        private FlowDocument descriptionDocument;
        public FlowDocument DescriptionDocument
        {
            get => descriptionDocument;
            private set => SetProperty(ref descriptionDocument, value);
        }
        private void UpdateDescriptionDocument()
        {
            var flowDoc = new FlowDocument();
            var paragraph = new Paragraph(new Run(Description));
            flowDoc.Blocks.Add(paragraph);
            DescriptionDocument = flowDoc;
        }

        // Команды для тестов
        public RelayCommand OpenTest { get; }
        public RelayCommand AddTest { get; }
        public RelayCommand<Test> RemoveTest { get; }
        public ObservableCollection<Test> Tests { get; private set; }
        private Test curTest, selectedTest;

        public Test CurTest
        {
            get => curTest;
            set => SetProperty(ref curTest, value);
        }

        public Test SelectedTest
        {
            get => selectedTest;
            set => SetProperty(ref selectedTest, value);
        }

        public DistrictDetailVM(District district, INavigationService service, int userId, int userType)
        {
            db = new TestDbContext();
            Name = district.Name;
            Foundating_date = district.FoundingDate.ToString();
            Description = district.Description;
            Image = district.Image;
            id = district.Id;

            this.service = service;
            this.userId = userId;
            this.UserType = userType;
            // Загрузка исторических достопримечательностей
            HistoricalSites = new ObservableCollection<HistoricalSite>([.. db.HistoricalSites.Where(u => u.District == district.Id)]);
            OpenSite = new RelayCommand<HistoricalSite>(OpenSites);
            AddSite = new RelayCommand(AddSites);
            RemoveSite = new RelayCommand<HistoricalSite>(RemoveSites);
            EditSite = new RelayCommand<HistoricalSite>(EditSites);

            // Загрузка тестов для района
            Tests = new ObservableCollection<Test>([.. db.Tests.Where(u => u.District == district.Id)]);
            OpenTest = new RelayCommand(OpenTestItem);
            AddTest = new RelayCommand(AddTestItem);
            RemoveTest = new RelayCommand<Test>(RemoveTestItem);
            completedTests = new HashSet<int>([.. db.Userresults.Where(r => r.User == userId).Select(r => r.Test)]);
        }

        private void UpdateData()
        {
            HistoricalSites.Clear();
            foreach (var site in db.HistoricalSites.Where(u => u.District == id).ToList())
                HistoricalSites.Add(site);

            Tests.Clear();
            foreach (var test in db.Tests.Where(u => u.District == id).ToList())
                Tests.Add(test);
        }

        private void OpenSites(HistoricalSite item)
        {
            if (item == null) return;
            ShowPopup($"Открытие достопримечательности: {item.Name}");
            var vm = new HistoricalSiteVM(item);
            service.Navigate(typeof(SitePage), vm);
        }

        private void AddSites()
        {
            CurSite = new HistoricalSite { District = id };
            var vm = new SiteEditVM(CurSite, service);
            vm.ItemUpdated += UpdateData;
            service.Navigate(typeof(SiteEditPage), vm);
        }

        private void RemoveSites(HistoricalSite item)
        {
            if (item == null) return;
            if (ShowDialog("Вы точно хотите удалить достопримечательность?", "Подтверждение удаления"))
            {
                db.HistoricalSites.Remove(item);
                db.SaveChanges();
                HistoricalSites.Remove(item);
                Navigation.Show("Удаление прошло успешно");
            }
        }

        protected override void OnClose()
        {
            UpdateData();
            UCVisibility = Visibility.Collapsed;
        }

        protected void EditSites(HistoricalSite item)
        {
            if (item == null) return;
            var vm = new SiteEditVM(item, service);
            vm.ItemUpdated += UpdateData;
            service.Navigate(typeof(SiteEditPage), vm);
        }

        // Реализация для тестов
        public bool IsTestCompleted(int testId)
        {
            return completedTests?.Contains(testId) ?? false;
        }
        private void OpenTestItem()
        {
            if (SelectedTest == null) return;
            ShowPopup($"Открытие теста: {SelectedTest.Title}");
            if (UserType == 5)
            {
                if (completedTests.Contains(SelectedTest.Id))
                {
                    ShowMessage("Вы уже прошли этот тест!");
                    return;
                }
                var vm = new TestQuestionsVM(SelectedTest, userId, service);
                service.Navigate(typeof(TestQuestionsPage), vm);
            }
            else
            {
                var vm = new EditTestVM(SelectedTest, service);
                service.Navigate(typeof(EditTestPage),vm);
            }
        }

        private void AddTestItem()
        {
            var vm = new AddTestViewModel(id, service);
            service.Navigate(typeof(TestAddPage), vm);
        }

        private void RemoveTestItem(Test item)
        {
            if (item == null) return;
            if (ShowDialog("Вы точно хотите удалить тест?", "Подтверждение удаления"))
            {
                db.Tests.Remove(item);
                db.SaveChanges();
                Tests.Remove(item);
            }
        }
    }
}