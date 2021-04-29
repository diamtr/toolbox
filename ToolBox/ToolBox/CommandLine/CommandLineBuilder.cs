using McMaster.Extensions.CommandLineUtils;
using System;

namespace ToolBox
{
  sealed public class CommandLineBuilder
  {
    #region Singletone impl

    private static readonly Lazy<CommandLineBuilder> instance = new Lazy<CommandLineBuilder>(() => new CommandLineBuilder());

    public static CommandLineBuilder GetInstance()
    {
      return instance.Value;
    }

    private CommandLineBuilder() { }

    #endregion

    public CommandLineApplication Application { get; private set; }

    public void InitApplication()
    {
      this.Application = new CommandLineApplication();
      this.Application.HelpOption(inherited: true);
      var toolsCommand = new ToolsCommand();
      toolsCommand.AddSubcommand(new AddCommand());
      toolsCommand.AddSubcommand(new ListCommand());
      this.Application.AddSubcommand(toolsCommand);
    }
  }
}
