using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using TestApp.Model;
using TestApp.Model.Classes;
using System.Windows;
using Microsoft.EntityFrameworkCore;

namespace TestApp.ViewModel
{
    public class HistoricalSiteVM : BaseViewModel
    {
        private readonly TestDbContext _db;
        private string name;
        private string constructionDate;
        private string description;
        private byte[] image;
        private int id;
        private ObservableCollection<SiteHistory> siteHistories;
        private SiteHistory selectedSiteHistory;

        public byte[] Image
        {
            get => image;
            set => SetProperty(ref image, value);
        }

        public string Name
        {
            get => name;
            set => SetProperty(ref name, value);
        }

        public string ConstructionDate
        {
            get => constructionDate;
            set => SetProperty(ref constructionDate, value);
        }

        public string Description
        {
            get => description;
            set => SetProperty(ref description, value);
        }

        public ObservableCollection<SiteHistory> SiteHistories
        {
            get => siteHistories;
            set => SetProperty(ref siteHistories, value);
        }


        public SiteHistory SelectedSiteHistory
        {
            get => selectedSiteHistory;
            set => SetProperty(ref selectedSiteHistory, value);
        }

        public RelayCommand Open { get; }
        public RelayCommand Add { get; }
        public RelayCommand<SiteHistory> Remove { get; }
        public RelayCommand<SiteHistory> Edit { get; }
        public RelayCommand Save { get; }

        private HistoricalSite CurrentHistoricalSite;
        private SiteHistory selHistory;
        private SiteHistory SelHistory
        {
            get => selHistory;
            set => SetProperty(ref selHistory, value);
        }
        public HistoricalSiteVM(HistoricalSite site)
        {
            _db = new TestDbContext();
            CurrentHistoricalSite = site;
            Name = site.Name;
            ConstructionDate = site.ConstructionDate.ToString();
            Description = site.Description;
            Image = site.Image;
            id = site.Id;
            SiteHistories = new ObservableCollection<SiteHistory>([.. _db.SiteHistories.Where(u => u.Site == site.Id)]); 
            Add = new RelayCommand(OnAdd);
            Edit = new RelayCommand<SiteHistory>(OnEdit);
            Remove = new RelayCommand<SiteHistory>(OnRemove);
            Save = new RelayCommand(OnSave);
        }
        private void UpdateData()
        {
            SiteHistories.Clear();
            foreach (var item in _db.SiteHistories.Where(u => u.Site == id).ToList())
                SiteHistories.Add(item); 
        }
        private void CloseUC()
        {
            UpdateData();
            UCVisibility = Visibility.Collapsed;
        }

        private void OnAdd()
        {
            UCVisibility = Visibility.Visible;
            SelHistory = new SiteHistory { Site = id };
        }

        private void OnEdit(SiteHistory item)
        {
            if (item == null) return;
            UCVisibility = Visibility.Visible;
            SelHistory = item;
        }

        private void OnRemove(SiteHistory item)
        {
            if (item == null) return;
            string result = MessageBoxs.ShowDialog("Вы точно хотите удалить событие?", "Подтверждение удаления", MessageBoxs.Buttons.YesNo);
            if (result == "1")
            {
                _db.SiteHistories.Remove(item);
                _db.SaveChanges();
                SiteHistories.Remove(item);
                MessageBoxs.Show("Удаление произошло успешно");
            }
            else
                MessageBoxs.Show(result);
        }

        private void OnSave()
        {
            try
            {
                string? errorMessage = Validate();
                if (errorMessage != null)
                {
                    MessageBox.Show(errorMessage);
                    return;
                }
                if (_db.SiteHistories.Local.Any(s => s.Id == SelHistory.Id)) 
                    _db.SiteHistories.Update(SelHistory);
                else _db.SiteHistories.Add(SelHistory);
                _db.SaveChanges();
                UpdateData();
                UCVisibility = Visibility.Collapsed;
            }
            catch (DbUpdateException dbEx)
            {
                var innerExceptionMessage = dbEx.InnerException != null ? dbEx.InnerException.Message : "Неизвестная ошибка.";
                MessageBoxs.Show($"Ошибка при сохранении данных: {innerExceptionMessage}");
            }
        }

        private string? Validate()
        {
            if (SelHistory?.EventDate == default)
                return "Введите дату!";
            if (string.IsNullOrWhiteSpace(SelHistory?.EventDescription))
                return "Введите описание!";
            return null;
        }
    }
}
