using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace RunForrestPlugin
{
  public class HasErrorToBrushConverter : IValueConverter
  {
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      if (targetType != typeof(Brush))
        throw new InvalidOperationException("Converter can only convert to value of type Brush.");

      if (value == null)
        return Brushes.Transparent;

      var hasError = System.Convert.ToBoolean(value, culture);
      if (Reverse)
        hasError = !hasError;

      // https://www.colorhexa.com/6495ed
      // Based on Major Button color CornflowerBlue (#6495ed = RGB(100,149,237))
      // Red is one of Split Complementary Color (#ed7864 = RGB(237,120,100))
      var redBrush = new BrushConverter().ConvertFrom("#ed7864");
      // Green is one of Triadic Color (#95ed64 = RGB(149,237,100))
      var greenBrush = new BrushConverter().ConvertFrom("#95ed64");
      return hasError ? redBrush : greenBrush;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
      throw new InvalidOperationException("Converter cannot convert back.");
    }

    public Boolean Reverse { get; set; }
  }
}