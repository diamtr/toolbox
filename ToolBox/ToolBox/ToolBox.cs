using NLog;
using System;

namespace ToolBox
{
  class ToolBox
  {
    static void Main(string[] args)
    {
      var log = LogManager.GetCurrentClassLogger();
      log.Info("Hello World!");
    }
  }
}
