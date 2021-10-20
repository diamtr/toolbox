using MinionCopy;
using NUnit.Framework;
using System;
using System.IO;
using System.Linq;

namespace MinionCopyTests
{
  public class CopyFileTests
  {
    private const string SourceFileName = "testFile.txt";
    private const string SourceFileContent = "test text";
    private const string DestFileName = "testFile2.txt";

    [SetUp]
    public void SetUp()
    {
      File.Delete(SourceFileName);
      File.Delete(DestFileName);
      File.WriteAllText(SourceFileName, SourceFileContent);
    }

    [Test]
    public void NewCopyOptionsIsEmpty()
    {
      var cfs = new CopyFileStrategy();
      Assert.IsNull(cfs.Source);
      Assert.IsNull(cfs.Destination);
      Assert.IsFalse(cfs.Replace);
      Assert.IsNull(cfs.Rename);
    }

    [Test]
    public void GetChildren()
    {
      var cfs = new CopyFileStrategy();
      var children = cfs.GetChildren();
      Assert.AreEqual(1, children.Count());
      Assert.AreEqual(cfs, children.First());
    }

    [Test]
    public void CopyFileValidation()
    {
      var options = new CopyFileStrategy();
      Assert.Throws<ArgumentException>(() => { options.Copy(); });
      options.Source = "C:\\Windows";
      Assert.Throws<ArgumentException>(() => { options.Copy(); });
      options.Source = "fileNotExists.txt";
      Assert.Throws<ArgumentException>(() => { options.Copy(); });
    }

    [Test]
    public void CopyFileSourceRelativeDestRelativeNamesOnly()
    {
      this.BuildCopyOptions(SourceFileName, DestFileName).Copy();
      FileAssert.Exists(SourceFileName, "Source file does not exist");
      FileAssert.Exists(DestFileName, "Destination file does not exist");
      File.Delete(DestFileName);
    }

    [Test]
    public void CopyFileSourceRelativeNameDestAbsoluteName()
    {
      var destFile = Path.GetFullPath(Path.Combine(TestContext.CurrentContext.TestDirectory, DestFileName));
      FileAssert.DoesNotExist(destFile, "Destination file already exists");
      this.BuildCopyOptions(SourceFileName, destFile).Copy();
      FileAssert.Exists(SourceFileName, "Source file does not exist");
      FileAssert.Exists(destFile, "Destination file does not exist");
      File.Delete(destFile);
    }

    [Test]
    public void CopyFileSourceAbsoluteNameDestAbsoluteName()
    {
      var sourceFile = Path.GetFullPath(Path.Combine(TestContext.CurrentContext.TestDirectory, SourceFileName));
      var destFile = Path.GetFullPath(Path.Combine(TestContext.CurrentContext.TestDirectory, DestFileName));
      File.WriteAllText(sourceFile, "test text");
      FileAssert.DoesNotExist(destFile, "Destination file already exists");
      this.BuildCopyOptions(sourceFile, destFile).Copy();
      FileAssert.Exists(sourceFile, "Source file does not exist");
      FileAssert.Exists(destFile, "Destination file does not exist");
      File.Delete(destFile);
    }

    [Test]
    public void CopyFileSourceRelativeNameDestExistingDirectoryName()
    {
      var destDirPath = Path.GetFullPath(Path.Combine(TestContext.CurrentContext.TestDirectory, "tmp"));
      Directory.CreateDirectory(destDirPath);
      this.BuildCopyOptions(SourceFileName, destDirPath).Copy();
      FileAssert.Exists(Path.Combine(destDirPath, SourceFileName));
      File.Delete(Path.Combine(destDirPath, SourceFileName));
      Directory.Delete(destDirPath, true);
    }

    [Test]
    public void CopyFileSourceRelativeNameDestNotExistingDirectoryName()
    {
      var destDirPath = Path.GetFullPath(Path.Combine(TestContext.CurrentContext.TestDirectory, "tmp\\ne"));
      this.BuildCopyOptions(SourceFileName, destDirPath).Copy();
      FileAssert.Exists(Path.Combine(destDirPath, SourceFileName));
      File.Delete(Path.Combine(destDirPath, SourceFileName));
      Directory.Delete(destDirPath, true);
    }

    [Test]
    public void CopyFileReplace()
    {
     this.BuildCopyOptions(SourceFileName, DestFileName).Copy();
      Assert.Throws<IOException>(() => { this.BuildCopyOptions(SourceFileName, DestFileName).Copy(); });
      var newContent = "new test text";
      File.WriteAllText(SourceFileName, newContent);
      this.BuildCopyOptions(SourceFileName, DestFileName, replace: true).Copy();
      Assert.AreEqual(newContent, File.ReadAllText(DestFileName));
    }

    [Test]
    public void CopyFileRename()
    {
      var newDestFileName = "testFile3.txt";
      FileAssert.Exists(SourceFileName, "Source file does not exist");
      this.BuildCopyOptions(SourceFileName, DestFileName, rename: newDestFileName).Copy();
      FileAssert.DoesNotExist(DestFileName, "Destination (original name) file exists");
      FileAssert.Exists(newDestFileName, "Destination file does not exist");
      File.Delete(newDestFileName);
    }

    private CopyFileStrategy BuildCopyOptions(string source, string dest,
      bool replace = false, string rename = null)
    {
      var options = new CopyFileStrategy();
      options.Source = source;
      options.Destination = dest;
      options.Replace = replace;
      options.Rename = rename;
      return options;
    }
  }
}