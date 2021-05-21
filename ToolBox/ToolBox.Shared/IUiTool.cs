using Avalonia.Controls;

namespace ToolBox.Shared
{
  public interface IUiTool
  {
    string DisplayName { get; }
    UserControl UserControl { get; }
  }
}
