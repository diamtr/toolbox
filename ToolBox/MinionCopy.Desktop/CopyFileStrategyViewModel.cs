using System;
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

    public event Action<ICopyStrategyViewModel> RemoveRequested;

    public Command RequestRemoveCommand { get; private set; }

    public ICopyStrategy GetStrategy()
    {
      return this.Strategy;
    }
    
    public void Copy()
    {
      this.CopyResult = CopyResult.None;

      try
      {
        this.Strategy.Copy();
        this.CopyResult = CopyResult.Success;
      }
      catch
      {
        this.CopyResult = CopyResult.Failed;
      }
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
