using System;
using System.Collections.Generic;
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
    private CopyException copyException;

    public event Action<ICopyStrategyViewModel> RemoveRequested;

    public Command RequestRemoveCommand { get; private set; }

    public ICopyStrategy GetStrategy()
    {
      return this.Strategy;
    }

    public void Copy()
    {
      this.CopyResult = CopyResult.None;
      this.copyException = null;

      try
      {
        this.Strategy.Copy();
        this.CopyResult = CopyResult.Success;
      }
      catch (Exception ex)
      {
        this.copyException = new CopyException(this, ex);
        this.CopyResult = CopyResult.Failed;
      }
    }
    
    public List<CopyException> GetCopyExceptions()
    {
      var exceptions = new List<CopyException>();
      if (this.copyException != null)
        exceptions.Add(this.copyException);
      return exceptions;
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
