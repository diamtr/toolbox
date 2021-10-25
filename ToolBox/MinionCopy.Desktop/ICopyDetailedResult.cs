using System;

namespace MinionCopy.Desktop
{
  public interface ICopyDetailedResult
  {
    public CopyResult CopyResult { get; }
    public string Message { get; }
    public ICopyStrategyViewModel Owner { get; }
    public Exception Exception { get; }
  }
}
