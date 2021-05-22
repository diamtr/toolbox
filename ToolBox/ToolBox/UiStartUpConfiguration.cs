using IvarI.Plugins.FileSystem;
using NLog;
using System;
using System.Collections.Generic;
using ToolBox.Shared;

namespace ToolBox
{
  class UiStartUpConfiguration : IStartUpCnfiguration
  {
    public List<IUiTool> Tools { get; private set; }
    private static ILogger log => LogManager.GetCurrentClassLogger();

    public void LoadTools(ISourcesConfiguration sourcesConfiguration)
    {
      log.Debug("Start loading tools");
      var loader = new Loader(sourcesConfiguration);
      this.Tools = loader.Load<IUiTool>();
      log.Debug($"Tools loaded: {this.Tools.Count}");
    }

    public void Run()
    {
      throw new NotImplementedException();
    }

    public UiStartUpConfiguration()
    {
      this.Tools = new List<IUiTool>();
    }
  }
}
