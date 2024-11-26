using System.Windows;
using System.Windows.Controls;
using TestApp.ViewModel;
namespace TestApp.View.District;
public partial class DistrictEditPage : Page
{
    public DistrictEditPage() => InitializeComponent();

    private void RichTextBox_Loaded(object sender, RoutedEventArgs e)
    {
        if (DataContext is DistrictEditViewModel viewModel)
        {
            RichTextBox.Document = viewModel.DescriptionDocument;
        }
    }
    private void RichTextBox_TextChanged(object sender, TextChangedEventArgs Args)
    {
        if (DataContext is DistrictEditViewModel viewModel)
        {
            viewModel.UpdateDescriptionFromDocument(RichTextBox.Document);
        }
    }
}
