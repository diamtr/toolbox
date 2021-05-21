using McMaster.Extensions.CommandLineUtils;

namespace ToolBox.Shared
{
  public interface IConsoleTool
  {
    CommandLineApplication Command { get; }
  }
}
