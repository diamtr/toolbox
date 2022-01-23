using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace RunForrest.Desktop
{
  public class ObjectToVisibilityConverter : IValueConverter
  {
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      if (targetType != typeof(Visibility))
        throw new InvalidOperationException("Converter can only convert to value of type Visibility.");

      if (value == null)
        return Visibility.Collapsed;

      return Visibility.Visible;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
      throw new InvalidOperationException("Converter cannot convert back.");
    }
  }
}
