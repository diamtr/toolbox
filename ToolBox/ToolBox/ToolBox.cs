using NLog;
using System;

namespace ToolBox
{
  class ToolBox
  {
    static ILogger log = LogManager.GetCurrentClassLogger();

    static void Main(string[] args)
    {
      log.Info("Hello World!");
    }
  }
}
