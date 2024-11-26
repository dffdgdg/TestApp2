using System.Collections.ObjectModel;
using TestApp.Model;
using System.Windows;
using Microsoft.EntityFrameworkCore;
using TestApp.ViewModel.Base;

namespace TestApp.ViewModel
{
    public class HistoricalSiteVM : BaseEditModel<SiteHistory>
    {
        private readonly TestDbContext _db;
        private HistoricalSite _currentHistoricalSite;
        private SiteHistory _selectedHistory;

        public ObservableCollection<SiteHistory> SiteHistories { get; private set; } = [];

        public string Name { get; set; }
        public string ConstructionDate { get; set; }
        public string Description { get; set; }
        public byte[] Image { get; set; }

        private DateOnly _hisDate;
        public DateOnly HisDate
        {
            get => _hisDate;
            set => SetProperty(ref _hisDate, value);
        }

        private string _hisDescription;
        public string HisDescription
        {
            get => _hisDescription;
            set => SetProperty(ref _hisDescription, value);
        }

        public HistoricalSiteVM(HistoricalSite site)
        {
            _db = new TestDbContext();
            _currentHistoricalSite = site;
            LoadHistoricalSiteData(site);
            LoadSiteHistoriesAsync();
        }

        private void LoadHistoricalSiteData(HistoricalSite site)
        {
            Name = site.Name;
            ConstructionDate = site.ConstructionDate.ToString();
            Description = site.Description;
            Image = site.Image;
        }

        private async void LoadSiteHistoriesAsync()
        {
            var histories = await _db.SiteHistories.Where(u => u.Site == _currentHistoricalSite.Id).OrderBy(u=>u.Date).ToListAsync();
            SiteHistories.Clear();
            foreach (var history in histories)
            {
                SiteHistories.Add(history);
            }
        }

        protected override void OnClose()
        {
            UCVisibility = Visibility.Collapsed;
        }

        protected override void OnAdd()
        {
            UCVisibility = Visibility.Visible;
            _selectedHistory = new SiteHistory { Site = _currentHistoricalSite.Id };
            HisDate = default;
            HisDescription = string.Empty;
        }

        protected override void OnEdit(SiteHistory item)
        {
            if (item == null) return;
            UCVisibility = Visibility.Visible;
            _selectedHistory = item;
            UpdateHistoryFields();
        }

        protected override async void OnRemove(SiteHistory item)
        {
            if (item == null) return;
            if (ShowDialog("Вы точно хотите удалить событие?", "Подтверждение удаления"))
            {
                _db.SiteHistories.Remove(item);
                await _db.SaveChangesAsync();
                SiteHistories.Remove(item);
                ShowPopup("Удаление прошло успешно");
            }
        }

        protected override async void OnSave()
        {
            try
            {
                _selectedHistory ??= new SiteHistory { Site = _currentHistoricalSite.Id };

                _selectedHistory.Date = HisDate;
                _selectedHistory.Description = HisDescription;

                string? errorMessage = Validate();
                if (errorMessage != null)
                {
                    ShowError(errorMessage);
                    return;
                }

                if (_db.SiteHistories.Local.Any(s => s.Id == _selectedHistory.Id))
                    _db.SiteHistories.Update(_selectedHistory);
                else
                    _db.SiteHistories.Add(_selectedHistory);

                await _db.SaveChangesAsync();
                LoadSiteHistoriesAsync();
                ShowPopup("Добавление прошло успешно");
                UCVisibility = Visibility.Collapsed;
            }
            catch (DbUpdateException dbEx)
            {
                var innerExceptionMessage = dbEx.InnerException?.Message ?? "Неизвестная ошибка.";
                ShowError($"Ошибка при сохранении данных: {innerExceptionMessage}");
            }
        }

        private string? Validate()
        {
            if (HisDate == default)
                return "Введите дату!";
            if (string.IsNullOrWhiteSpace(HisDescription))
                return "Введите описание!";
            return null;
        }

        private void UpdateHistoryFields()
        {
            if (_selectedHistory != null)
            {
                HisDate = _selectedHistory.Date;
                HisDescription = _selectedHistory.Description;
            }
        }

        protected override void OnOpen(SiteHistory item)
        {
        }
    }
}