using NUnit.Framework;
using System;
using System.IO;
using System.Linq;

namespace ToolBox.Core.Tests
{
  [TestFixture]
  public class ToolsTests
  {
    [Test]
    public void CreateTools()
    {
      var tools = new Tools();
      Assert.IsEmpty(tools);
    }

    [Test]
    public void LoadDefaultConf()
    {
      var source = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..\\..\\..", "tools"); 
      var dest = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "tools");
      this.CopyDirectory(new DirectoryInfo(source), new DirectoryInfo(dest));
      var tools = new Tools();
      tools.Load();
      Assert.AreEqual(1, tools.Count());
    }

    private void CopyDirectory(DirectoryInfo source, DirectoryInfo target)
    {
      if (!source.Exists)
        return;
      if (!target.Exists)
        Directory.CreateDirectory(target.FullName);
      foreach (var dir in source.GetDirectories())
        CopyDirectory(dir, target.CreateSubdirectory(dir.Name));
      foreach (var file in source.GetFiles())
        file.CopyTo(Path.Combine(target.FullName, file.Name), overwrite: true);
    }
  }
}