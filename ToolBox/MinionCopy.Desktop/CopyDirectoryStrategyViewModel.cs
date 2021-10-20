using System;
using ToolBox.Desktop.Base;

namespace MinionCopy.Desktop
{
  public class CopyDirectoryStrategyViewModel : ViewModelBase, ICopyStrategyViewModel
  {
    public CopyDirectoryStrategy Strategy
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

    private CopyDirectoryStrategy strategy;
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

    public CopyDirectoryStrategyViewModel(CopyDirectoryStrategy strategy) : this()
    {
      this.Strategy = strategy;
    }

    public CopyDirectoryStrategyViewModel()
    {
      this.Strategy = new CopyDirectoryStrategy();
      this.CopyResult = CopyResult.None;
      this.RequestRemoveCommand = new Command(
        x => { this.InvokeRequestRemoveFromParent(); },
        x => true
        );
    }
  }
}
