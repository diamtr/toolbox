using McMaster.Extensions.CommandLineUtils;

namespace ToolBox.Console.Base
{
  public interface IConsoleTool
  {
    CommandLineApplication Command { get; }
  }
}
