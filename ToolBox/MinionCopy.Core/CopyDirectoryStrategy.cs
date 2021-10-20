using System;
using System.IO;
using System.Linq;

namespace MinionCopy
{
  public class CopyDirectoryStrategy : CopyUnitStrategy
  {
    public bool Recursive { get; set; }

    public override ICopyStrategy WithSourceExistsValidation()
    {
      if (!(new DirectoryInfo(this.Source).Exists))
        throw new ArgumentException($"{nameof(CopyDirectoryStrategy)}. Directory '{nameof(this.Source)}' does not exist.");

      return this;
    }

    public override ICopyStrategy PrepareDestination()
    {
      if (!new DirectoryInfo(this.Destination).Exists)
      {
        var withReplace = this.Replace ? " with replace" : string.Empty;
        var recursive = this.Recursive ? " recursive" : string.Empty;
        Console.WriteLine($"Copy{withReplace}{recursive}: {this.Source} >> Create {this.Destination}");
        Directory.CreateDirectory(this.Destination);
      }

      return this;
    }

    public override ICopyStrategy WithRename()
    {
      var targetDirName = new DirectoryInfo(this.Source).Name;
      if (!string.IsNullOrWhiteSpace(this.Rename))
        targetDirName = this.Rename;
      this.Destination = Path.Combine(this.Destination, targetDirName);

      return this;
    }

    public override ICopyStrategy WithReplace()
    {
      if (this.Replace && new DirectoryInfo(this.Destination).Exists)
      {
        Directory.Delete(this.Destination, true);
        Console.WriteLine($"Delete {this.Destination}");
      }

      return this;
    }

    public override void Copy()
    {
      var withReplace = this.Replace ? " with replace" : string.Empty;
      var recursive = this.Recursive ? " recursive" : string.Empty;
      Console.WriteLine($"Copy{withReplace}{recursive}: {this.Source} >> {this.Destination}");

      this
        .ValidateReqiredProperties()
        .MakeSourcePathRooted()
        .MakeDestinationPathRooted()
        .WithSourceExistsValidation()
        .WithRename()
        .WithReplace()
        .PrepareDestination();

      foreach (var fileInfo in new DirectoryInfo(this.Source).GetFiles())
        new CopyFileStrategy()
        {
          Source = fileInfo.FullName,
          Destination = this.Destination,
          Replace = this.Replace
        }.Copy();

      if (!this.Recursive)
        return;

      foreach (var dirInfo in new DirectoryInfo(this.Source).GetDirectories())
      {
        var newDestination = Path.Combine(this.Destination, dirInfo.Name);
        if (!dirInfo.GetFiles().Any() && !dirInfo.GetDirectories().Any())
          Directory.CreateDirectory(newDestination);
        else
          new CopyDirectoryStrategy()
          {
            Source = dirInfo.FullName,
            Destination = newDestination,
            Replace = this.Replace,
            Recursive = this.Recursive
          }.Copy();
      }
    }
  }
}
