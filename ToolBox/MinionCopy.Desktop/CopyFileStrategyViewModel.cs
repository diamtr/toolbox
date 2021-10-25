using System;
using System.Collections.Generic;
using ToolBox.Desktop.Base;

namespace MinionCopy.Desktop
{
  public class CopyFileStrategyViewModel : ViewModelBase, ICopyStrategyViewModel
  {
    public CopyFileStrategy Strategy
    {
      get
      {
        return this.strategy;
      }
      set
      {
        this.strategy = value;
        this.OnPropertyChanged();
      }
    }
    public CopyResult CopyResult
    {
      get
      {
        return this.copyResult;
      }
      set
      {
        this.copyResult = value;
        this.OnPropertyChanged();
      }
    }

    private CopyFileStrategy strategy;
    private CopyResult copyResult;
    private ICopyDetailedResult copyDetailedResult;

    public event Action<ICopyStrategyViewModel> RemoveRequested;

    public Command RequestRemoveCommand { get; private set; }

    public ICopyStrategy GetStrategy()
    {
      return this.Strategy;
    }
    
    public void Copy()
    {
      this.CopyResult = CopyResult.None;
      this.copyDetailedResult = null;

      try
      {
        this.Strategy.Copy();
        this.copyDetailedResult = new CopySuccess(this);
        this.CopyResult = CopyResult.Success;
      }
      catch (Exception ex)
      {
        this.copyDetailedResult = new CopyException(this, ex);
        this.CopyResult = CopyResult.Failed;
      }
    }

    public List<ICopyDetailedResult> GetCopyDetailedResults()
    {
      var detailedResults = new List<ICopyDetailedResult>();
      if (this.copyDetailedResult != null)
        detailedResults.Add(this.copyDetailedResult);
      return detailedResults;
    }

    public bool HasItem(ICopyStrategyViewModel item)
    {
      return Equals(this, item);
    }

    public void SetSelectedItem(ICopyStrategyViewModel item)
    {
      return;
    }

    public void InvokeRequestRemoveFromParent()
    {
      this.RemoveRequested?.Invoke(this);
    }

    public CopyFileStrategyViewModel(CopyFileStrategy strategy) : this()
    {
      this.Strategy = strategy;
    }

    public CopyFileStrategyViewModel()
    {
      this.Strategy = new CopyFileStrategy();
      this.CopyResult = CopyResult.None;
      this.RequestRemoveCommand = new Command(
        x => { this.InvokeRequestRemoveFromParent(); },
        x => true
        );
    }
  }
}
