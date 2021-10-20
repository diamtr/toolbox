using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace MinionCopy
{
  public class CopyFromListStrategy : CopyManyStrategy
  {
    public string Name { get; set; }

    public override void Copy()
    {
      this
        .ValidateReqiredProperties()
        .MakeSourcePathRooted()
        .WithSourceExistsValidation();
      this.ReadSource();

      foreach (var item in this.GetChildren())
        item.Copy();
    }

    public override ICopyStrategy ValidateReqiredProperties()
    {
      if (!string.IsNullOrWhiteSpace(this.Name))
        return this;

      if (string.IsNullOrWhiteSpace(this.Source))
        throw new ArgumentException($"{nameof(CopyFromListStrategy)}. '{nameof(this.Source)}' is null or empty.");

      return this;
    }
    public override ICopyStrategy WithSourceExistsValidation()
    {
      if (!string.IsNullOrWhiteSpace(this.Name))
        return this;

      if (!(new FileInfo(this.Source).Exists))
        throw new ArgumentException($"{nameof(CopyFromListStrategy)}. File in '{nameof(this.Source)}' does not exist.");

      return this;
    }

    public void ReadSource()
    {
      if (!string.IsNullOrWhiteSpace(this.Name))
        return;

      this.Items = CopyFromListStrategy.Parse(this.Source).Items;
    }

    public void Save(string path)
    {
      this.Source = Path.GetFileNameWithoutExtension(path);
      var settings = CopyStrategy.Json.GetDefaultSerializerSettings();
      var content = JsonConvert.SerializeObject(this, Formatting.Indented, settings);
      File.WriteAllText(path, content, Encoding.UTF8);
    }

    public static CopyFromListStrategy Parse(string path)
    {
      var content = File.ReadAllText(path);
      var settings = CopyStrategy.Json.GetDefaultSerializerSettings();
      return JsonConvert.DeserializeObject<CopyFromListStrategy>(content, settings);
    }
  }
}
