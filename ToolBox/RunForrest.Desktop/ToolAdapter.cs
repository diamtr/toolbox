using System.ComponentModel.Composition;
using System.Windows.Controls;
using ToolBox.Shared;

namespace RunForrestPlugin
{
  [Export(typeof(ITool))]
  public class ToolAdapter : ITool
  {
    private UserControl userControl;

    public string Name => "Run Forrest";
    public int? Priority => 10;
    public UserControl UserControl
    {
      get
      {
        if (this.userControl == null)
          this.userControl = new RunForrest();

        return this.userControl;
      }
    }
  }
}
