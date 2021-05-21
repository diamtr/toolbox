using McMaster.Extensions.CommandLineUtils;

namespace ToolBox
{
  class ListCommand : CommandLineApplication
  {
    private int OnExecuteHandler()
    {
      return 0;
    }

    public ListCommand()
    {
      this.Name = "list";
      this.FullName = "list";
      this.Description = "List tools in box.";
      this.OnExecute(() => { this.OnExecuteHandler(); });
    }
  }
}
