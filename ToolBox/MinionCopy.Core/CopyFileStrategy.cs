using System;
using System.IO;

namespace MinionCopy
{
  public class CopyFileStrategy : CopyUnitStrategy
  {
    public override ICopyStrategy WithSourceExistsValidation()
    {
      if (!(new FileInfo(this.Source).Exists))
        throw new ArgumentException($"{nameof(CopyFileStrategy)}. File in '{nameof(this.Source)}' does not exist.");

      return this;
    }

    public override ICopyStrategy PrepareDestination()
    {
      if (!this.IsFilePath(this.Destination))
        this.Destination = Path.Combine(this.Destination, Path.GetFileName(this.Source));
      var dirPath = Path.GetDirectoryName(this.Destination);
      var di = new DirectoryInfo(dirPath);
      if (!di.Exists)
      {
        Console.WriteLine($"Copy: {this.Source} >> Create {dirPath}");
        di.Create();
      }

      return this;
    }

    public override ICopyStrategy WithRename()
    {
      if (string.IsNullOrWhiteSpace(this.Rename))
        return this;

      this.Destination = Path.Combine(Path.GetDirectoryName(this.Destination), this.Rename);

      return this;
    }

    public override ICopyStrategy WithReplace()
    {
      // Property this.Replace used "as is" in Copy() method.
      return this;
    }

    public override void Copy()
    {
      this
        .ValidateReqiredProperties()
        .MakeSourcePathRooted()
        .MakeDestinationPathRooted()
        .WithSourceExistsValidation()
        .PrepareDestination()
        .WithRename()
        .WithReplace();

      File.Copy(this.Source, this.Destination, this.Replace);

      var withReplace = this.Replace ? " with replace" : string.Empty;
      Console.WriteLine($"Copy{withReplace}: {this.Source} >> {this.Destination}");
    }

    protected bool IsFilePath(string path)
    {
      // Some like this root\subdir\subdir\
      if (path.EndsWith("\\"))
        return false;

      if (new FileInfo(path).Exists)
        return true;

      if (new DirectoryInfo(path).Exists)
        return false;

      return !string.IsNullOrWhiteSpace(Path.GetExtension(path));
    }
  }
}
