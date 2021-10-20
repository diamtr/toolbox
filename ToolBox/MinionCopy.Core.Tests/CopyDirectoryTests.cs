using MinionCopy;
using NUnit.Framework;
using System.IO;
using System.Linq;

namespace MinionCopyTests
{
  public class CopyDirectoryTests
  {
    TestDirectories directories;

    [SetUp]
    public void SetUp()
    {
      this.directories = new TestDirectories();
    }

    [TearDown]
    public void TearDown()
    {
      this.directories.TryClear();
      this.directories = null;
    }

    [Test]
    public void GetChildren()
    {
      var cds = new CopyDirectoryStrategy();
      var children = cds.GetChildren();
      Assert.AreEqual(1, children.Count());
      Assert.AreEqual(cds, children.First());
    }

    [Test]
    public void CopyDirectory()
    {
      this.directories.Create("test_source_dir");
      var cds = new CopyDirectoryStrategy() { Source = "test_source_dir", Destination = "test_dest_dir" };
      cds.Copy();
      this.ExistWithContent("test_dest_dir", 0, 1);
      this.ExistWithContent("test_dest_dir\\test_source_dir", 0, 0);
    }

    [Test]
    public void CopyDirectoryWithSubDirectoryNoRecursive()
    {
      this.directories.Create("test_source_dir");
      this.directories.Create("test_source_dir\\empty");
      var cds = new CopyDirectoryStrategy() { Source = "test_source_dir", Destination = "test_dest_dir" };
      cds.Copy();
      this.ExistWithContent("test_dest_dir", 0, 1);
      this.ExistWithContent("test_dest_dir\\test_source_dir", 0, 0);
      this.DirectoryNotExists("test_dest_dir\\test_source_dir\\empty");
    }

    [Test]
    public void CopyDirectoryWithSubDirectoryRecursive()
    {
      this.directories.Create("test_source_dir");
      this.directories.Create("test_source_dir\\empty");
      var cds = new CopyDirectoryStrategy() { Source = "test_source_dir", Destination = "test_dest_dir", Recursive = true };
      cds.Copy();
      this.ExistWithContent("test_dest_dir", 0, 1);
      this.ExistWithContent("test_dest_dir\\test_source_dir", 0, 1);
      this.ExistWithContent("test_dest_dir\\test_source_dir\\empty", 0, 0);
    }

    [Test]
    public void CopyDirectoryRename()
    {
      this.directories.Create("test_source_dir");
      var cds = new CopyDirectoryStrategy() { Source = "test_source_dir", Destination = "test_dest_dir", Rename = "test_renamed_dir" };
      cds.Copy();
      this.ExistWithContent("test_dest_dir", 0, 1);
      this.ExistWithContent("test_dest_dir\\test_renamed_dir", 0, 0);
      this.DirectoryNotExists("test_dest_dir\\test_source_dir");
    }

    [Test]
    public void CopyDirectoryWithSubDirectoryRenameNoRecursive()
    {
      this.directories.Create("test_source_dir");
      this.directories.Create("test_source_dir\\subdir");
      var cds = new CopyDirectoryStrategy() { Source = "test_source_dir", Destination = "test_dest_dir", Rename = "test_renamed_dir" };
      cds.Copy();
      this.ExistWithContent("test_dest_dir", 0, 1);
      this.ExistWithContent("test_dest_dir\\test_renamed_dir", 0, 0);
      this.DirectoryNotExists("test_dest_dir\\test_renamed_dir\\subdir");
      this.DirectoryNotExists("test_dest_dir\\test_source_dir");
      this.DirectoryNotExists("test_dest_dir\\test_source_dir\\subdir");
    }

    [Test]
    public void CopyDirectoryWithSubDirectoryRenameRecursive()
    {
      this.directories.Create("test_source_dir");
      this.directories.Create("test_source_dir\\subdir");
      var cds = new CopyDirectoryStrategy() { Source = "test_source_dir", Destination = "test_dest_dir", Rename = "test_renamed_dir", Recursive = true };
      cds.Copy();
      this.ExistWithContent("test_dest_dir", 0, 1);
      this.ExistWithContent("test_dest_dir\\test_renamed_dir", 0, 1);
      this.ExistWithContent("test_dest_dir\\test_renamed_dir\\subdir", 0, 0);
      this.DirectoryNotExists("test_dest_dir\\test_source_dir");
      this.DirectoryNotExists("test_dest_dir\\test_source_dir\\subdir");
    }

    [Test]
    public void CopyDirectoryReplace()
    {
      this.directories.Create("test_source_dir");
      File.WriteAllText("test_source_dir\\controlfile.txt", "");
      this.directories.Create("test_dest_dir\\test_source_dir");
      File.WriteAllText("test_dest_dir\\test_source_dir\\toreplace.txt", "");
      var cds = new CopyDirectoryStrategy() { Source = "test_source_dir", Destination = "test_dest_dir", Replace = true };
      cds.Copy();
      this.ExistWithContent("test_dest_dir", 0, 1);
      this.ExistWithContent("test_dest_dir\\test_source_dir", 1, 0);
      this.FileExists("test_dest_dir\\test_source_dir\\controlfile.txt");
      this.FileNotExists("test_dest_dir\\test_source_dir\\toreplace.txt");
    }

    [Test]
    public void CopyDirectoryWithSubDirectoryReplaceNoRecursive()
    {
      this.directories.Create("test_source_dir");
      File.WriteAllText("test_source_dir\\controlfile.txt", "");
      this.directories.Create("test_dest_dir\\test_source_dir");
      File.WriteAllText("test_dest_dir\\test_source_dir\\toreplace.txt", "");
      this.directories.Create("test_dest_dir\\test_source_dir\\subdir");
      File.WriteAllText("test_dest_dir\\test_source_dir\\subdir\\toreplacesub.txt", "");
      var cds = new CopyDirectoryStrategy() { Source = "test_source_dir", Destination = "test_dest_dir", Replace = true };
      cds.Copy();
      this.ExistWithContent("test_dest_dir", 0, 1);
      this.ExistWithContent("test_dest_dir\\test_source_dir", 1, 0);
      this.FileExists("test_dest_dir\\test_source_dir\\controlfile.txt");
      this.FileNotExists("test_dest_dir\\test_source_dir\\toreplace.txt");
      this.DirectoryNotExists("test_dest_dir\\test_source_dir\\subdir");
      this.FileNotExists("test_dest_dir\\test_source_dir\\subdir\\toreplacesub.txt");
    }

    private void FileNotExists(string path)
    {
      Assert.IsFalse(new FileInfo(path).Exists);
    }

    private void FileExists(string path)
    {
      Assert.IsTrue(new FileInfo(path).Exists);
    }

    private void DirectoryNotExists(string path)
    {
      Assert.IsFalse(new DirectoryInfo(path).Exists);
    }

    private void ExistWithContent(string path, int files, int directories)
    {
      var di = this.directories.Track(path);
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
