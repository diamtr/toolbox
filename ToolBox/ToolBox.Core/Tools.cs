using IvarI.Plugins.FileSystem;
using System.Collections;
using System.Collections.Generic;

namespace ToolBox.Core
{
  public class Tools : IEnumerable<ITool>
  {
    private List<ITool> tools = new List<ITool>();

    #region IEnumerable<T> impl

    public IEnumerator<ITool> GetEnumerator()
    {
      foreach (var tool in this.tools)
        yield return tool;
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
      return GetEnumerator();
    }

    #endregion

    public void Load(ISourcesConfiguration configuration)
    {
      var loader = new Loader(configuration);
      this.tools.Clear();
      var a = loader.Load<ITool>();
      this.tools.AddRange(a);
    }

    public void Load()
    {
      this.Load(FileSystemConfiguration.Default);
    }
  }
}
