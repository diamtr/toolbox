using System;

namespace MinionCopy.Desktop
{
  public class CopyException
  {
    public ICopyStrategyViewModel Owner { get; private set; }
    public Exception Exception { get; private set; }

    public CopyException(ICopyStrategyViewModel owner, Exception exception)
    {
      this.Owner = owner;
      this.Exception = exception;
    }
  }
}
