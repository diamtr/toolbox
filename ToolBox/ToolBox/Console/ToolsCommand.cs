using McMaster.Extensions.CommandLineUtils;

namespace ToolBox
{
  class ToolsCommand : CommandLineApplication
  {
    private int OnExecuteHandler()
    {
      this.ShowHelp();
      return 1;
    }

    public ToolsCommand()
    {
      this.Name = "tools";
      this.FullName = "tools";
      this.Description = "Set of commands to manage tools.";
      this.OnExecute(() => this.OnExecuteHandler()); ;
    }
  }
}
