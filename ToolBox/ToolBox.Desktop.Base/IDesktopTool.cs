using System.Windows.Controls;

namespace ToolBox.Desktop.Base
{
  public interface IDesktopTool
  {
    public string DisplayName { get; }

    public UserControl GetUserControl();
  }
}
