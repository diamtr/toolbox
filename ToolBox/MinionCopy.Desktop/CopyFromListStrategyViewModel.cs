using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using ToolBox.Desktop.Base;

namespace MinionCopy.Desktop
{
  public class CopyFromListStrategyViewModel : ViewModelBase, ICopyStrategyViewModel
  {
    public string DisplayName
    {
      get
      {
        return this.displayName;
      }
      set
      {
        this.displayName = value;
        this.OnPropertyChanged();
      }
    }
    public CopyFromListStrategy Strategy
    {
      get
      {
        return this.strategy;
      }
      private set
      {
        this.strategy = value;
        this.OnPropertyChanged();
      }
    }
    public ObservableCollection<ICopyStrategyViewModel> DisplayItems { get; set; }
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
    public string LastStrategyPath
    { 
      get
      {
        return this.lastStrategyPath;
      }
      private set
      {
        this.lastStrategyPath = value;
        this.OnPropertyChanged();
      }
    }

    protected readonly string DefaultInitialDirectory = Path.Combine(Environment.CurrentDirectory, "Tools", "minioncopy");
    private string displayName;
    private CopyFromListStrategy strategy;
    private CopyResult copyResult;
    private string lastStrategyPath;

    public event Action<ICopyStrategyViewModel> RemoveRequested;

    public Command AddCopyFileStrategyCommand { get; private set; }
    public Command AddCopyDirectoryStrategyCommand { get; private set; }
    public Command AddCopyFromListStrategyCommand { get; private set; }
    public Command ClearCopyFromListStrategyCommand { get; private set; }
    public Command OpenCopyFromListStrategyCommand { get; private set; }
    public Command SaveCopyFromListStrategyCommand { get; private set; }
    public Command RequestRemoveCommand { get; private set; }

    public ICopyStrategy GetStrategy()
    {
      return this.Strategy;
    }

    public void SetStrategy(CopyFromListStrategy strategy)
    {
      this.ClearCopyFromListStrategy();
      this.CopyResult = CopyResult.None;

      if (!string.IsNullOrWhiteSpace(strategy.Name))
        this.DisplayName = strategy.Name;

      foreach (var item in strategy.Items)
      {
        ICopyStrategyViewModel newViewModel = null;
        if (item is CopyFileStrategy)
          newViewModel = new CopyFileStrategyViewModel((CopyFileStrategy)item);
        if (item is CopyDirectoryStrategy)
          newViewModel = new CopyDirectoryStrategyViewModel((CopyDirectoryStrategy)item);
        if (item is CopyFromListStrategy)
          newViewModel = new CopyFromListStrategyViewModel((CopyFromListStrategy)item);
        if (newViewModel != null)
        {
          this.DisplayItems.Add(newViewModel);
          newViewModel.RemoveRequested += this.DisplayItems_RemoveRequested;
        }
      }

      this.Strategy = strategy;
    }

    public void Copy()
    {
      this.CopyResult = CopyResult.None;

      foreach (var strategyViewModel in this.DisplayItems)
        strategyViewModel.Copy();

      if (this.DisplayItems.Any(x => x.CopyResult == CopyResult.Failed))
        this.CopyResult = CopyResult.Failed;
      else if (this.DisplayItems.Any(x => x.CopyResult == CopyResult.Success))
        this.CopyResult = CopyResult.Success;
    }

    public List<CopyException> GetCopyExceptions()
    {
      var exceptions = new List<CopyException>();
      foreach (var item in this.DisplayItems)
      {
        var itemExceptions = item.GetCopyExceptions();
        if (itemExceptions.Any())
          exceptions.AddRange(itemExceptions);
      }
      return exceptions;
    }

    public void AddCopyFileStrategy()
    {
      var newViewModel = new CopyFileStrategyViewModel();
      this.DisplayItems.Add(newViewModel);
      newViewModel.RemoveRequested += this.DisplayItems_RemoveRequested;
    }

    public void AddCopyDirectoryStrategy()
    {
      var newViewModel = new CopyDirectoryStrategyViewModel();
      this.DisplayItems.Add(newViewModel);
      newViewModel.RemoveRequested += this.DisplayItems_RemoveRequested;
    }

    public void AddCopyFromListStrategy()
    {
      var newViewModel = new CopyFromListStrategyViewModel();
      this.DisplayItems.Add(newViewModel);
      newViewModel.RemoveRequested += this.DisplayItems_RemoveRequested;
    }

    public void ClearCopyFromListStrategy()
    {
      this.DisplayItems.Clear();
    }

    public void OpenCopyFromListStrategy()
    {
      var dialog = new OpenFileDialog();
      dialog.InitialDirectory = this.DefaultInitialDirectory;
      dialog.DefaultExt = ".json";
      dialog.Filter = "json files (*.json)|*.json";
      dialog.Multiselect = false;
      dialog.CheckFileExists = true;
      dialog.CheckPathExists = true;

      var result = dialog.ShowDialog();
      if (result != true)
        return;

      this.OpenCopyFromListStrategy(dialog.FileName);
    }

    public void OpenCopyFromListStrategy(string path)
    {
      var fi = new FileInfo(path);
      if (!fi.Exists)
        return;
      this.LastStrategyPath = path;
      var strategy = CopyFromListStrategy.Parse(path);
      this.SetStrategy(strategy);
    }

    public void SaveCopyFromListStrategy()
    {
      var dialog = new SaveFileDialog();
      dialog.InitialDirectory = this.DefaultInitialDirectory;
      dialog.FileName = this.Strategy.Name;
      dialog.DefaultExt = ".json";
      dialog.Filter = "json files (*.json)|*.json";
      dialog.CheckPathExists = true;

      var result = dialog.ShowDialog();
      if (result != true)
        return;

      this.LastStrategyPath = dialog.FileName;
      this.Strategy.Save(dialog.FileName);
    }

    public void InvokeRequestRemoveFromParent()
    {
      this.RemoveRequested?.Invoke(this);
    }

    private void DisplayItems_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
    {
      if (e.Action == NotifyCollectionChangedAction.Add)
      {
        var itemsToAdd = e.NewItems.Cast<ICopyStrategyViewModel>()
          .Select(x => x.GetStrategy())
          .Except(this.Strategy.Items);
        if (itemsToAdd.Any())
          this.Strategy.Items.AddRange(itemsToAdd);
      }

      if (e.Action == NotifyCollectionChangedAction.Remove)
      {
        var itemsToRemove = e.OldItems.Cast<ICopyStrategyViewModel>()
          .Select(x => x.GetStrategy())
          .Intersect(this.Strategy.Items);
        foreach (var item in itemsToRemove)
          this.Strategy.Items.Remove(item);
      }

      if (e.Action == NotifyCollectionChangedAction.Reset)
        this.Strategy.Items.Clear();
    }

    private void DisplayItems_RemoveRequested(ICopyStrategyViewModel sender)
    {
      this.DisplayItems.Remove(sender);
    }

    private void InitCommands()
    {
      this.AddCopyFileStrategyCommand = new Command(
        x => { this.AddCopyFileStrategy(); },
        x => true
        );

      this.AddCopyDirectoryStrategyCommand = new Command(
        x => { this.AddCopyDirectoryStrategy(); },
        x => true
        );

      this.AddCopyFromListStrategyCommand = new Command(
        x => { this.AddCopyFromListStrategy(); },
        x => true
        );

      this.ClearCopyFromListStrategyCommand = new Command(
        x => { this.ClearCopyFromListStrategy(); },
        x => true
        );

      this.OpenCopyFromListStrategyCommand = new Command(
        x => { this.OpenCopyFromListStrategy(); },
        x => true
        );

      this.SaveCopyFromListStrategyCommand = new Command(
        x => { this.SaveCopyFromListStrategy(); },
        x => true
        );

      this.RequestRemoveCommand = new Command(
        x => { this.InvokeRequestRemoveFromParent(); },
        x => true
        );
    }

    public CopyFromListStrategyViewModel(CopyFromListStrategy strategy) : this()
    {
      this.SetStrategy(strategy);
    }

    public CopyFromListStrategyViewModel()
    {
      this.Strategy = new CopyFromListStrategy();
      this.CopyResult = CopyResult.None;
      this.DisplayName = "<New list>";
      this.DisplayItems = new ObservableCollection<ICopyStrategyViewModel>();
      this.DisplayItems.CollectionChanged += this.DisplayItems_CollectionChanged;
      this.InitCommands();
    }
  }
}
