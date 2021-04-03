namespace ToolBox.Core
{
  public interface ITool
  {
    string GetCommandLineName();
    int Execute(params string[] args);
  }
}
