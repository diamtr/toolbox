using McMaster.Extensions.CommandLineUtils;

namespace ToolBox
{
  class AddCommand : CommandLineApplication
  {
    private CommandArgument path;

    private int OnExecuteHandler()
    {
      return 0;
    }

    public AddCommand()
    {
      this.Name = "add";
      this.FullName = "add";
      this.Description = "Add tool in box.";
      this.path = this.Argument("path", "Tool directory path.").IsRequired();
      this.OnExecute(() => { this.OnExecuteHandler(); });
    }
  }
}
