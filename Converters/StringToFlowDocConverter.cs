using System.Globalization;
using System.Windows.Data;
using System.Windows.Documents;

namespace TestApp.Converters;

public class StringToFlowDocConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is string text)
        {
            var flowDoc = new FlowDocument();
            if (!string.IsNullOrWhiteSpace(text))
            {
                var paragraph = new Paragraph(new Run(text));
                flowDoc.Blocks.Add(paragraph);
            }
            return flowDoc;
        }
        return null;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is FlowDocument flowDoc)
        {
            var textRange = new TextRange(flowDoc.ContentStart, flowDoc.ContentEnd);
            return textRange.Text.Trim();
        }
        return null;
    }
}