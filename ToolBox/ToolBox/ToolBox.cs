using IvarI.Plugins.FileSystem;
using NLog;
using System;
using System.IO;
using System.Linq;

namespace ToolBox
{
  public class ToolBox
  {
    static ILogger log = LogManager.GetCurrentClassLogger();
    const string DefaultToolsDirectoryName = "tools";
    string defaultToolsDirectoryPath => Path.Join(AppDomain.CurrentDomain.BaseDirectory, DefaultToolsDirectoryName);
    IStartUpCnfiguration startUpConfiguration;

    public void CreateDefaultToolsDirectoryIfNotExists()
    {
      if (Directory.Exists(defaultToolsDirectoryPath))
        return;
      log.Warn($"Default tools path does not exist: {defaultToolsDirectoryPath}");
      Directory.CreateDirectory(defaultToolsDirectoryPath);
      log.Info($"Create: {defaultToolsDirectoryPath}");
    }

    public void LoadTools()
    {
      log.Debug($"Start loading tools: {defaultToolsDirectoryPath}");
      var loadConfiguration = new Configuration();
      loadConfiguration.AddSubDirectories(defaultToolsDirectoryPath, 1);
      if (loadConfiguration.GetPaths().Count() == 0)
        log.Warn($"Is empty: {defaultToolsDirectoryPath}");
      foreach (var path in loadConfiguration.GetPaths())
        log.Debug($"Found: {path}");

      this.startUpConfiguration.LoadTools(loadConfiguration);
    }

    public void Run()
    {
      log.Debug("Start run");
      this.startUpConfiguration.Run();
    }

    public ToolBox(IStartUpCnfiguration configuration)
    {
      this.startUpConfiguration = configuration;
    }
  }
}
