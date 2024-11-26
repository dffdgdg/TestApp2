using CommunityToolkit.Mvvm.Input;
using Microsoft.Win32;
using System.Windows;
using System.Windows.Documents;

namespace TestApp.ViewModel.Base;
public abstract class BaseEditsModel<T> : BaseViewModel
{
    private T item;
    public T Item
    {
        get => item;
        set { if (SetProperty(ref item, value)) UpdateDescriptionDocument(); }
    }
    public abstract FlowDocument DescriptionDocument { get; set; }

    protected abstract string Description { get; }

    public event Action ItemUpdated;

    private RelayCommand? save;
    public RelayCommand Save => save ??= new RelayCommand(OnSave);

    private RelayCommand? loadImage;
    public RelayCommand LoadImage => loadImage ??= new RelayCommand(OnLoadImage);

    protected abstract void OnLoadImage();
    protected abstract void OnSave();
    protected abstract string Validate();
    protected abstract void SetImage(string filePath);

    protected void NotifyItemUpdated() => ItemUpdated?.Invoke();

    protected void LoadImageFromFile()
    {
        var openFileDialog = new OpenFileDialog
        {
            Filter = "Image Files (*.jpg;*.jpeg;*.png;*.bmp)|*.jpg;*.jpeg;*.png;*.bmp",
            Title = "Выберите изображение"
        };

        if (openFileDialog.ShowDialog() == true) SetImage(openFileDialog.FileName);
    }
    protected virtual void UpdateDescriptionDocument()
    {
        var flowDoc = new FlowDocument();
        if (!string.IsNullOrWhiteSpace(Description))
        {
            var paragraphs = Description.Split(new[] { "\n", "\r\n" }, StringSplitOptions.RemoveEmptyEntries);

            foreach (var text in paragraphs)
            {
                var paragraph = new Paragraph(new Run(text)) { Margin = new Thickness(0, 5, 0, 5) };
                flowDoc.Blocks.Add(paragraph);
            }
        }
        else
        {
            var paragraph = new Paragraph(new Run("")) { Margin = new Thickness(0, 5, 0, 5) };
            flowDoc.Blocks.Add(paragraph);
        }
        DescriptionDocument = flowDoc;
    }
}