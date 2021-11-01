using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace RunForrestPlugin
{
  public class IsRunningToBrushConverter : IValueConverter
  {
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      if (targetType != typeof(Brush))
        throw new InvalidOperationException("Converter can only convert to value of type Brush.");

      if (value == null)
        return Brushes.Transparent;

      var isRunning = System.Convert.ToBoolean(value, culture);
      if (Reverse)
        isRunning = !isRunning;

      var cornflowerBlue = new BrushConverter().ConvertFrom("#b55400");

      return isRunning ? cornflowerBlue : Brushes.Transparent;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
      throw new InvalidOperationException("Converter cannot convert back.");
    }

    public Boolean Reverse { get; set; }
  }
}