using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace MinionCopyTests
{
  class TestDirectories
  {
    public List<DirectoryInfo> Directories { get; private set; }

    public TestDirectories()
    {
      this.Directories = new List<DirectoryInfo>();
    }

    public DirectoryInfo Create(string path)
    {
      var di = new DirectoryInfo(path);
      if (!di.Exists)
        di.Create();
      this.Directories.Add(di);
      return di;
    }

    public DirectoryInfo Track(DirectoryInfo di)
    {
      this.Directories.Add(di);
      return di;
    }

    public DirectoryInfo Track(string path)
    {
      var di = new DirectoryInfo(path);
      this.Directories.Add(di);
      return di;
    }

    public void TryClear(int attempts = 3)
    {
      if (attempts <= 0)
        return;

      try
      {
        foreach (var di in this.Directories)
        {
          di.Refresh();
          if (di.Exists)
            di.Delete(true);
          di.Refresh();
          if (!di.Exists)
            this.Directories.Remove(di);
        }
      }
      catch { }

      if (this.Directories.Any())
        TryClear(attempts--);
    }
  }
}
