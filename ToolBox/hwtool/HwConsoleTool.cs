using McMaster.Extensions.CommandLineUtils;
using System;
using System.ComponentModel.Composition;
using ToolBox.Shared;

namespace hwtool
{
  [Export(typeof(IConsoleTool))]
  public class HwConsoleTool : IConsoleTool
  {
    public CommandLineApplication Command { get; private set; }

    public HwConsoleTool()
    {
      this.Command = this.InitCommand();
    }

    private CommandLineApplication InitCommand()
    {
      var app = new CommandLineApplication()
      {
        Name = "hwt",
        Description = "Hello World console tool"
      };
      app.HelpOption(inherited: true);
      app.OnExecute(this.OnExecute);
      return app;
    }

    private void OnExecute()
    {
      Console.WriteLine("HELLO WORLD!");
    }
  }
}
