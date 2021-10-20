using System;

namespace MinionCopy.Desktop
{
  public interface ICopyStrategyViewModel
  {
    public CopyResult CopyResult { get; set; }
    public ICopyStrategy GetStrategy();
    public void Copy();

    public event Action<ICopyStrategyViewModel> RemoveRequested;
  }
}
