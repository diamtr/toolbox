using Avalonia.Controls;

namespace ToolBox.Shared
{
  public interface IExhibitable
  {
    string DisplayName { get; }
    UserControl UserControl { get; }
  }
}
