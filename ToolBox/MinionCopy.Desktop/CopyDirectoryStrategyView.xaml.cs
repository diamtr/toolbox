using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MinionCopy.Desktop
{
  /// <summary>
  /// Interaction logic for CopyDirectoryStrategyView.xaml
  /// </summary>
  public partial class CopyDirectoryStrategyView : UserControl
  {
    public CopyDirectoryStrategyView()
    {
      InitializeComponent();
    }

    private void TextBox_Drop(object sender, DragEventArgs e)
    {
      // https://stackoverflow.com/a/45946696/8878639
      var textBox = (TextBox)sender;
      if (!e.Data.GetDataPresent(DataFormats.FileDrop) ||
          textBox == null)
        return;

      var files = (string[])e.Data.GetData(DataFormats.FileDrop);
      if (files == null || files.Length == 0)
        return;
      textBox.SetCurrentValue(TextBox.TextProperty, files[0]);
      var bindingExpression = textBox.GetBindingExpression(TextBox.TextProperty);
      if (bindingExpression != null && bindingExpression.ParentBinding != null &&
          (bindingExpression.ParentBinding.Mode == BindingMode.Default ||
           bindingExpression.ParentBinding.Mode == BindingMode.TwoWay ||
           bindingExpression.ParentBinding.Mode == BindingMode.OneWayToSource))
        bindingExpression.UpdateSource();
    }

    private void TextBox_PreviewDragOver(object sender, DragEventArgs e)
    {
      // https://stackoverflow.com/a/37415941/8878639
      e.Handled = true;
    }
  }
}
