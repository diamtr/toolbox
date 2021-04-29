using IvarI.Plugins.FileSystem;
using NLog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ToolBox
{
  sealed class ToolBox
  {
    const string DefaultToolsDirectory = "tools";

    static ILogger log = LogManager.GetCurrentClassLogger();
    string baseToolsPath => Path.Join(AppDomain.CurrentDomain.BaseDirectory, DefaultToolsDirectory);
    ISourcesConfiguration configuration;
    List<object> tools;

    static void Main(string[] args)
    {
      var cli = CommandLineBuilder.GetInstance();
      cli.InitApplication();
      var toolBox = new ToolBox();
      toolBox.ConfigureToolsSources();
      toolBox.LoadTools();
      cli.Application.Execute(args);
    }

    private void ConfigureToolsSources()
    {
      this.CreateDefaultToolsDirectoryIfNotExists();
      this.configuration = new Configuration();
      this.configuration.AddSubDirectories(baseToolsPath, 1);
      if (this.configuration.GetPaths().Count() == 0)
        log.Warn("Sources configuration: Empty.");
      foreach (var path in this.configuration.GetPaths())
        log.Debug($"Sources configuration: {path}");
    }

    private void CreateDefaultToolsDirectoryIfNotExists()
    {
      if (Directory.Exists(baseToolsPath))
        return;
      log.Warn($"Default tools path does not exist: {baseToolsPath}");
      Directory.CreateDirectory(baseToolsPath);
      log.Info($"Create: {baseToolsPath}");
    }

    private void LoadTools()
    {
      log.Info("Load tools...");
      var loader = new Loader(this.configuration);
      var tools = loader.Load<object>();
      log.Info("Load tools. Done");
    }

    public ToolBox()
    {
      this.tools = new List<object>();
    }
  }
}
