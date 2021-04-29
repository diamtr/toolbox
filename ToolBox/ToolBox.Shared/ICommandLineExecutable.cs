using McMaster.Extensions.CommandLineUtils;

namespace ToolBox.Shared
{
  public interface ICommandLineExecutable
  {
    CommandLineApplication Command { get; }
  }
}
