using System.ComponentModel.Composition;
using System.Windows.Controls;
using ToolBox.Desktop.Base;

namespace hwtool.Desktop
{
  [Export(typeof(IDesktopTool))]
  public class DesktopTool : IDesktopTool
  {
    public string DisplayName
    {
      get
      {
        return "Hello, World!";
      }
    }

    public UserControl GetUserControl()
    {
      return new Hwtool();
    }
  }
}
