using NLog;

namespace ToolBox
{
  sealed class ToolBox
  {
    static ILogger log = LogManager.GetCurrentClassLogger();

    static void Main(string[] args)
    {
      log.Info("Hello World!");

      var toolBox = new ToolBox();
      toolBox.LoadTools();
    }

    private void LoadTools()
    {
      log.Info("Load tools...");
      log.Info("Load tools. Done");
    }
  }
}
