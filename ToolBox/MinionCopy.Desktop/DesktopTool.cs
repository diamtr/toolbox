using System.ComponentModel.Composition;
using System.Windows.Controls;
using ToolBox.Desktop.Base;

namespace MinionCopy.Desktop
{
  [Export(typeof(IDesktopTool))]
  public class DesktopTool : IDesktopTool
  {
    private UserControl instance;

    public string DisplayName
    {
      get
      {
        return "Minion Copy";
      }
    }
    public UserControl GetUserControl()
    {
      if (this.instance == null)
        this.instance = new MinionCopy();
      return this.instance;
    }
  }
}
