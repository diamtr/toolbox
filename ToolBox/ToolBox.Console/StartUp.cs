using NLog;
using System;

namespace ToolBox.Console
{
  sealed class StartUp
  {
    static ILogger log = LogManager.GetCurrentClassLogger();

    static void Main(string[] args)
    {
      try
      {
        var configuration = new StartUpConfiguration(args);
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
