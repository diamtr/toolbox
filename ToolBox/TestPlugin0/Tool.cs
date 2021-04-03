using System.ComponentModel.Composition;
using ToolBox.Core;

namespace TestTool0
{
  [Export(typeof(ITool))]
  public class Tool : ITool
  {
    public string GetCommandLineName()
    {
      return "tool0";
    }

    public int Execute(params string[] args)
    {
      System.Console.WriteLine(@"Execute tool0 command.");
      return 0;
    }
  }
}
