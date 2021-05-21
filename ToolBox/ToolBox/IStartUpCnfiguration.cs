using IvarI.Plugins.FileSystem;

namespace ToolBox
{
  public interface IStartUpCnfiguration
  {
    void LoadTools(ISourcesConfiguration sourcesConfiguration);

    void Run();
  }
}
