using IvarI.Plugins.FileSystem;
using System;
using System.Collections.Generic;
using ToolBox.Desktop.Base;

namespace ToolBox.Desktop
{
  internal class ToolsProvider
  {
    internal static ToolsProvider Instance
    {
      get
      {
        if (instance == null)
          instance = new ToolsProvider();
        return instance;
      }
    }
    internal static bool HasLoadException
    {
      get
      {
        return Instance.hasLoadException;
      }
    }
    internal static ToolsLoadException ToolsLoadException
    {
      get
      {
        return Instance.toolsLoadException;
      }
    }

    private static ToolsProvider instance;
    private bool hasLoadException;
    private ToolsLoadException toolsLoadException;

    public static List<IDesktopTool> LoadFrom(string path)
    {
      Instance.hasLoadException = false;
      Instance.toolsLoadException = null;

      try
      {
        var configuration = new Configuration();
        configuration.AddSubDirectories(path);
        var loader = new Loader(configuration);
        return loader.Load<IDesktopTool>();
      }
      catch (Exception ex)
      {
        Instance.hasLoadException = true;
        Instance.toolsLoadException = new ToolsLoadException("Tools loading failed.", ex);
      }

      return new List<IDesktopTool>();
    }
  }
}
