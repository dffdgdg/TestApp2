using Microsoft.EntityFrameworkCore;
using System.Collections.ObjectModel;
using TestApp.Model;
using TestApp.Services;
using TestApp.View.District;
using TestApp.ViewModel.Base;

namespace TestApp.ViewModel;

public class DistinctVM : BaseEditModel<Model.District>
{
    private readonly TestDbContext _db;
    public ObservableCollection<District> Districts { get; private set; }
    private District curDistrict;

    public District CurDistrict
    {
        get => curDistrict;
        set => SetProperty(ref curDistrict, value);
    }

    public DistinctVM(int userId, int userType, INavigationService service)
    {
        _db = new TestDbContext();
        Districts = [];
        this.service = service;
        this.userId = userId;
        UserType = userType;
        LoadDistrictsAsync();
    }

    private async void LoadDistrictsAsync()
    {
        var districts = await _db.Districts.ToListAsync();
        foreach (var district in districts)
        {
            Districts.Add(district);
        }
    }

    protected override void OnOpen(District item)
    {
        if (item == null) return;
        ShowPopup($"Открытие района: {item.Name}");
        var vm = new DistrictDetailVM(item, service, userId, UserType);
        service.Navigate(typeof(DistinctDetailPage), vm);
    }

    protected override void OnAdd()
    {
        CurDistrict = new District();
        var vm = new DistrictEditViewModel(CurDistrict, service);
        vm.ItemUpdated += UpdateData;
        service.Navigate(typeof(DistrictEditPage), vm);
    }

    protected override async void OnRemove(District item)
    {
        if (item == null) return;
        if (ShowDialog("Вы точно хотите удалить район?", "Подтверждение удаления"))
        {
            _db.Districts.Remove(item);
            await _db.SaveChangesAsync();
            Districts.Remove(item);
            Navigation.Show("Удаление прошло успешно");
        }
    }

    protected override void OnEdit(District item)
    {
        if (item == null) return; 
        var vm = new DistrictEditViewModel(item, service);
        vm.ItemUpdated += UpdateData;
        service.Navigate(typeof(DistrictEditPage), vm);
    }

    private void UpdateData()
    {
        Districts.Clear();
        LoadDistrictsAsync();
    }

    protected override void OnSave()
    {
    }

    protected override void OnClose()
    {
        throw new NotImplementedException();
    }
}