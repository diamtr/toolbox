using IvarI.Plugins.FileSystem;
using NLog;
using System;
using System.Collections.Generic;

namespace ToolBox
{
  sealed class StartUp
  {
    static ILogger log = LogManager.GetCurrentClassLogger();

    static void Main(string[] args)
    {
      try
      {
        var configuration = Configurations.Create(args);
        var toolBox = new ToolBox(configuration);
        toolBox.CreateDefaultToolsDirectoryIfNotExists();
        toolBox.LoadTools();
        toolBox.Run();
      }
      catch (Exception ex)
      {
        log.Error(ex);
      }
    }
  }
}
