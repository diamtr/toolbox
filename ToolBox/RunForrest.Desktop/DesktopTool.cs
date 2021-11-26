using System.ComponentModel.Composition;
using System.Windows.Controls;
using ToolBox.Desktop.Base;

namespace RunForrest.Desktop
{
  [Export(typeof(IDesktopTool))]
  public class DesktopTool : IDesktopTool
  {
    private UserControl instance;

    public string DisplayName
    {
      get
      {
        return "Run, Forrest!";
      }
    }
    public UserControl GetUserControl()
    {
      if (this.instance == null)
        this.instance = new RunForrest();
      return this.instance;
    }
  }
}
