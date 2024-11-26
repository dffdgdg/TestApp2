using System.Windows;
using System.Windows.Controls;
using TestApp.ViewModel;
namespace TestApp.View.HistoricalSite;

public partial class SiteEditPage : Page
{
    public SiteEditPage() => InitializeComponent();
    private void RichTextBox_Loaded(object sender, RoutedEventArgs e)
    {
        if (DataContext is SiteEditVM viewModel)
        {
            RichTextBox.Document = viewModel.DescriptionDocument;
        }
    }
    private void RichTextBox_TextChanged(object sender, TextChangedEventArgs Args)
    {
        if (DataContext is SiteEditVM viewModel)
        {
            viewModel.UpdateDescriptionFromDocument(RichTextBox.Document);
        }
    }
}
