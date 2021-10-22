using System;
using System.Collections.Generic;

namespace MinionCopy.Desktop
{
  public interface ICopyStrategyViewModel
  {
    public CopyResult CopyResult { get; set; }
    public ICopyStrategy GetStrategy();
    public void Copy();
    public List<CopyException> GetCopyExceptions();

    public event Action<ICopyStrategyViewModel> RemoveRequested;
  }
}
