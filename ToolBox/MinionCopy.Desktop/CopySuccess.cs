using System;

namespace MinionCopy.Desktop
{
  public class CopySuccess : ICopyDetailedResult
  {
    public CopyResult CopyResult { get; private set; }
    public string Message
    {
      get
      {
        return $"Ok. {this.Owner.GetStrategy().Source} >> {this.Owner.GetStrategy().Destination}";
      }
    }
    public ICopyStrategyViewModel Owner { get; private set; }
    public Exception Exception { get; private set; }

    public CopySuccess(ICopyStrategyViewModel owner)
    {
      this.CopyResult = CopyResult.Success;
      this.Owner = owner;
    }
  }
}
