using System;
using System.Globalization;
using System.Windows.Data;

namespace RunForrestPlugin
{
  public class DoubleToDoubleConverter : IValueConverter
  {
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      if (targetType != typeof(double))
        throw new InvalidOperationException("Converter can only convert to value of type int.");

      if (value == null)
        return null;

      var intValue = System.Convert.ToDouble(value, culture);
      if (this.Offset.HasValue)
        intValue += this.Offset.Value;

      return intValue;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
      throw new InvalidOperationException("Converter cannot convert back.");
    }

    public double? Offset { get; set; }
  }
}
