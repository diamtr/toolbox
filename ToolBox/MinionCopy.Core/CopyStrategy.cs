using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace MinionCopy
{
  public abstract class CopyStrategy : ICopyStrategy
  {
    public string Source { get; set; }
    public string Destination { get; set; }
    public bool Replace { get; set; }
    public string Rename { get; set; }
    
    public abstract ICopyStrategy ValidateReqiredProperties();
    public abstract ICopyStrategy WithSourceExistsValidation();
    public abstract ICopyStrategy PrepareDestination();
    public abstract ICopyStrategy WithRename();
    public abstract ICopyStrategy WithReplace();
    public abstract void Copy();
    public abstract IEnumerable<ICopyStrategy> GetChildren();

    public virtual ICopyStrategy MakeSourcePathRooted()
    {
      if (!string.IsNullOrWhiteSpace(this.Source))
        this.Source = Path.GetFullPath(this.Source);
      return this;
    }
    public virtual ICopyStrategy MakeDestinationPathRooted()
    {
      if (!string.IsNullOrWhiteSpace(this.Destination))
        this.Destination = Path.GetFullPath(this.Destination);
      return this;
    }

    public class Json
    {
      public static JsonSerializerSettings GetDefaultSerializerSettings()
      {
        return new JsonSerializerSettings()
        {
          TypeNameHandling = TypeNameHandling.Objects
        };
      }
    }
  }
}
