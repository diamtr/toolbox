using System.ComponentModel.Composition;
using System.Windows.Controls;
using ToolBox.Desktop.Base;

namespace RunForrest.Desktop
{
  [Export(typeof(IDesktopTool))]
  public class DesktopTool : IDesktopTool
  {
    private UserControl userControlInstance;
    private object dataContextInstance;

    public string DisplayName
    {
      get
      {
        return "Run, Forrest!";
      }
    }

    public UserControl GetUserControl()
    {
      if (this.userControlInstance == null)
        this.userControlInstance = new RunForrest();
      if (this.dataContextInstance == null)
        this.dataContextInstance = MainViewModel.GetInstance();
      this.userControlInstance.DataContext = this.dataContextInstance;
      return this.userControlInstance;
    }
  }
}
