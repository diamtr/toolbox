using System;
using System.Collections.Generic;

namespace MinionCopy.Desktop
{
  public interface ICopyStrategyViewModel
  {
    public CopyResult CopyResult { get; set; }
    public ICopyStrategy GetStrategy();
    public void Copy();
    public List<ICopyDetailedResult> GetCopyDetailedResults();
    public bool HasItem(ICopyStrategyViewModel item);
    public void SetSelectedItem(ICopyStrategyViewModel item);

    public event Action<ICopyStrategyViewModel> RemoveRequested;
  }
}
