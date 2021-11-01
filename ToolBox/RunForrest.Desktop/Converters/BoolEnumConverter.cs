using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Data;

namespace RunForrestPlugin
{
  public class BoolEnumConverter : IValueConverter
  {
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      string parameterString = parameter as string;
      if (parameterString == null)
        return DependencyProperty.UnsetValue;

      if (value == null || Enum.IsDefined(value.GetType(), value) == false)
        return DependencyProperty.UnsetValue;

      object parameterValue = Enum.Parse(value.GetType(), parameterString);

      return parameterValue.Equals(value);
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
      if (!(value is bool) || !(bool)value)
        return DependencyProperty.UnsetValue;

      string parameterString = parameter as string;
      if (parameterString == null)
        return DependencyProperty.UnsetValue;

      return Enum.Parse(targetType, parameterString);
    }
  }
}
