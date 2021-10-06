using IvarI.Plugins.FileSystem;
using McMaster.Extensions.CommandLineUtils;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using ToolBox.Console.Base;

namespace ToolBox.Console
{
  public class StartUpConfiguration
  {
    public List<IConsoleTool> Tools { get; private set; }
    protected static ILogger log => LogManager.GetCurrentClassLogger();
    protected IEnumerable<string> args;

    public void LoadTools(ISourcesConfiguration sourcesConfiguration)
    {
      log.Debug("Start loading tools");
      var loader = new Loader(sourcesConfiguration);
      this.Tools = loader.Load<IConsoleTool>();
      log.Debug($"Tools loaded: {this.Tools.Count}");
    }

    public void Run()
    {
      if (this.args == null)
        throw new ApplicationException($"Console arguments are required in console mode.");
      var app = this.InitCommandLineApplication();
      app.Execute(this.args.ToArray());
    }

    protected CommandLineApplication InitCommandLineApplication()
    {
      log.Debug("Init CommandLineApplication");
      var app = new CommandLineApplication();
      app.HelpOption(inherited: true);
      app.OnExecute(() => {
        log.Warn("Have no tool to execute.");
        app.ShowHelp();
        return 1;
      });

      foreach (var tool in this.Tools)
        app.AddSubcommand(tool.Command);

      return app;
    }

    public StartUpConfiguration(IEnumerable<string> args)
    {
      this.Tools = new List<IConsoleTool>();
      this.args = args;
    }
  }
}
