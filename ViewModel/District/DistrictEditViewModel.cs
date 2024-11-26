using Microsoft.EntityFrameworkCore;
using System.IO;
using System.Windows;
using System.Windows.Documents;
using TestApp.Model;
using TestApp.Services;
using TestApp.ViewModel.Base;

namespace TestApp.ViewModel;

class DistrictEditViewModel : BaseEditsModel<District>
{
    private readonly TestDbContext db;
    private FlowDocument descriptionDocument;
    protected override string Description => Item.Description;
    public override FlowDocument DescriptionDocument
    {
        get => descriptionDocument;
        set => SetProperty(ref descriptionDocument, value);
    }

    internal static readonly string[] separator = ["\n", "\r\n"];

    public DistrictEditViewModel(District item, INavigationService service)
    {
        db = new TestDbContext();
        Item = item;
        this.service = service;
        UpdateDescriptionDocument();
    }

    protected override void OnLoadImage() => LoadImageFromFile();

    protected override async void OnSave()
    {
        try
        {
            string? errorMessage = Validate();

            if (errorMessage != null)
            {
                ShowMessage(errorMessage);
                return;
            }
            await SaveItemAsync();
            NotifyItemUpdated();
            service.GoBack();
        }
        catch (DbUpdateException dbEx)
        {
            var innerExceptionMessage = dbEx.InnerException?.Message ?? "Неизвестная ошибка.";
            ShowError($"Ошибка при сохранении данных: {innerExceptionMessage}");
        }
        catch (Exception ex) { ShowError($"Ошибка: {ex.Message}"); }
    }

    private async Task SaveItemAsync()
    {
        if (db.Districts.Any(d => d.Id == Item.Id)) db.Districts.Update(Item);
        else db.Districts.Add(Item);
        await db.SaveChangesAsync();
    }

    protected override string? Validate()
    {
        if (string.IsNullOrWhiteSpace(Item?.Name))
            return "Введите название!";
        if (string.IsNullOrWhiteSpace(Item?.Description))
            return "Введите описание!";
        if (Item?.FoundingDate == null)
            return "Введите дату основания!";
        if (Item?.Image == null)
            return "Выберите изображение!";
        if (db.Districts.Any(u => u.Name == Item.Name && u.Id != Item.Id))
            return "Регион с таким именем уже существует!";
        return null;
    }

    protected override void SetImage(string filePath)
    {
        Item.Image = File.ReadAllBytes(filePath);
    }

    public void UpdateDescriptionFromDocument(FlowDocument document)
    {
        var text = new TextRange(document.ContentStart, document.ContentEnd).Text;
        Item.Description = text;
    }
}