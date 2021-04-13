using IvarI.Plugins.FileSystem;
using System;
using System.IO;

namespace ToolBox.Core
{
  public class FileSystemConfiguration : Configuration
  {
    public static ISourcesConfiguration Empty => new FileSystemConfiguration();

    public static ISourcesConfiguration Default
    {
      get
      {
        var conf = new FileSystemConfiguration();
        conf.AddDirectory(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "tools"));
        return conf;
      }
    }
  }
}
