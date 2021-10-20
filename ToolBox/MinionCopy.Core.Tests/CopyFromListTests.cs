using MinionCopy;
using Newtonsoft.Json;
using NUnit.Framework;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace MinionCopyTests
{
  public class CopyFromListTests
  {
    [Test]
    public void GetChildren()
    {
      var ls = new CopyFromListStrategy();
      ls.Items.Add(new CopyFileStrategy());
      ls.Items.Add(new CopyDirectoryStrategy());
      ls.Items.Add(new CopyFromListStrategy());
      var children = ls.GetChildren();
      Assert.AreEqual(2, children.Count());
      this.IsCopyFileStrategy(children.First());
      this.IsCopyDirectoryStrategy(children.Last());
    }

    [Test]
    public void SerializeList()
    {
      var ls = new CopyFromListStrategy();
      ls.Items.Add(new CopyFileStrategy());
      ls.Items.Add(new CopyDirectoryStrategy());
      ls.Items.Add(new CopyFromListStrategy());
      this.MakeCopyListFile("test_list", ls);
      var etalon = File.ReadAllText("serialization_etalon.json");
      var current = File.ReadAllText("test_list");
      Assert.AreEqual(etalon, current);
    }

    [Test]
    public void DeserializeList()
    {
      var settings = CopyStrategy.Json.GetDefaultSerializerSettings();
      var mainStrategy = JsonConvert.DeserializeObject<CopyFromListStrategy>(File.ReadAllText("deserialization_etalon.json"), settings);
      Assert.AreEqual(3, mainStrategy.Items.Count);
      this.IsCopyFileStrategy(mainStrategy.Items[0]);
      this.AreSame(new CopyFileStrategy() { Source = "tsf1", Destination = "tdf1" }, mainStrategy.Items[0] as CopyFileStrategy);
      this.IsCopyDirectoryStrategy(mainStrategy.Items[1]);
      this.AreSame(new CopyDirectoryStrategy() { Source = "tsd1", Destination = "tdd1", Replace = true }, mainStrategy.Items[1] as CopyDirectoryStrategy);
      this.IsCopyFromListStrategy(mainStrategy.Items[2]);
      this.AreSame(new CopyFromListStrategy() { Source = "tl1" }, mainStrategy.Items[2] as CopyFromListStrategy);
      var copylist = mainStrategy.Items[2] as CopyFromListStrategy;
      Assert.AreEqual(3, copylist.Items.Count);
      this.IsCopyFileStrategy(copylist.Items[0]);
      this.AreSame(new CopyFileStrategy() { Source = "tsf2", Destination = "tdf2", Replace = true, Rename = "tmpf" }, copylist.Items[0] as CopyFileStrategy);
      this.IsCopyDirectoryStrategy(copylist.Items[1]);
      this.AreSame(new CopyDirectoryStrategy() { Source = "tsd2", Destination = "tdd2", Recursive = true, Rename = "tmpd" }, copylist.Items[1] as CopyDirectoryStrategy);
      this.IsCopyFromListStrategy(copylist.Items[2]);
      var copylist2 = copylist.Items[2] as CopyFromListStrategy;
      this.AreSame(new CopyFromListStrategy() { Source = "tl2" }, copylist.Items[2] as CopyFromListStrategy);
      Assert.AreEqual(0, copylist2.Items.Count);
    }

    [Test]
    public void CopyFromList()
    {
      var directories = new TestDirectories();
      var basedir = directories.Create("cfl_dir");
      var innerdir0 = directories.Create("cfl_dir\\inner0");
      var innerdir1 = directories.Create("cfl_dir\\inner1");
      var innerdir11 = directories.Create("cfl_dir\\inner1\\inner11");
      File.WriteAllText("cfl_dir\\file", "test content");

      var controlStrategy = new CopyFromListStrategy();
      controlStrategy.Items = new List<ICopyStrategy>() {
        new CopyDirectoryStrategy() { Source = "cfl_dir\\inner0", Destination = "cfl_dir\\dest" },
        new CopyDirectoryStrategy() { Source = "cfl_dir\\inner1", Destination = "cfl_dir\\dest", Recursive = true },
        new CopyFileStrategy() { Source = "cfl_dir\\file", Destination = "cfl_dir\\dest\\", Rename = "filern" }
      };

      this.MakeCopyListFile("cfl_dir\\testlist", controlStrategy);

      var cfl_strategy = new CopyFromListStrategy() {
        Source = "cfl_dir\\testlist"
      };
      cfl_strategy.Copy();

      this.ExistWithContent("cfl_dir\\dest", 1, 2);
      this.ExistWithContent("cfl_dir\\dest\\inner0", 0, 0);
      this.ExistWithContent("cfl_dir\\dest\\inner1", 0, 1);
      this.ExistWithContent("cfl_dir\\dest\\inner1\\inner11", 0, 0);
      Assert.IsTrue(new FileInfo("cfl_dir\\dest\\filern").Exists);

      directories.TryClear();
    }

    private void MakeCopyListFile(string name, ICopyStrategy strategy)
    {
      var settings = CopyStrategy.Json.GetDefaultSerializerSettings();
      var content = JsonConvert.SerializeObject(strategy, Formatting.Indented, settings);
      File.WriteAllText(name, content, Encoding.UTF8);
    }

    private void IsCopyFileStrategy(ICopyStrategy strategy)
    {
      Assert.AreEqual(typeof(CopyFileStrategy), strategy.GetType());
    }

    private void IsCopyDirectoryStrategy(ICopyStrategy strategy)
    {
      Assert.AreEqual(typeof(CopyDirectoryStrategy), strategy.GetType());
    }

    private void IsCopyFromListStrategy(ICopyStrategy strategy)
    {
      Assert.AreEqual(typeof(CopyFromListStrategy), strategy.GetType());
    }

    private void AreSame(CopyFileStrategy expected, CopyFileStrategy actual)
    {
      Assert.AreEqual(expected.Source, actual.Source);
      Assert.AreEqual(expected.Destination, actual.Destination);
      Assert.AreEqual(expected.Replace, actual.Replace);
      Assert.AreEqual(expected.Rename, actual.Rename);
    }

    private void AreSame(CopyDirectoryStrategy expected, CopyDirectoryStrategy actual)
    {
      Assert.AreEqual(expected.Source, actual.Source);
      Assert.AreEqual(expected.Destination, actual.Destination);
      Assert.AreEqual(expected.Replace, actual.Replace);
      Assert.AreEqual(expected.Rename, actual.Rename);
      Assert.AreEqual(expected.Recursive, actual.Recursive);
    }

    private void AreSame(CopyFromListStrategy expected, CopyFromListStrategy actual)
    {
      Assert.AreEqual(expected.Source, actual.Source);
    }

    private void ExistWithContent(string path, int files, int directories)
    {
      var di = new DirectoryInfo(path);
      Assert.IsTrue(di.Exists);
      this.CheckContentCount(di, files, directories);
    }

    private void CheckContentCount(DirectoryInfo di, int files, int directories)
    {
      if (files == 0)
        Assert.IsFalse(di.GetFiles().Any());
      else
        Assert.AreEqual(files, di.GetFiles().Count());

      if (directories == 0)
        Assert.IsFalse(di.GetDirectories().Any());
      else
        Assert.AreEqual(directories, di.GetDirectories().Count());
    }
  }
}
