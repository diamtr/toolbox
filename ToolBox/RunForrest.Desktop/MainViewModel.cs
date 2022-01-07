using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using ToolBox.Desktop.Base;

namespace RunForrest.Desktop
{
  public class MainViewModel : ViewModelBase
  {

    #region Fields & Properties
    
    public string OpenedFilePath
    {
      get
      {
        return this.openedFilePath;
      }
      set
      {
        this.openedFilePath = value;
        this.OnPropertyChanged();
      }
    }
    public ViewModelBase AdditionalContentAreaViewModel
    {
      get
      {
        return this.contentAreaViewModelStack.Peek();
      }
    }
    public Variables Variables { get; protected set; }
    public Outputs Outputs { get; protected set; }
    public ScriptsListViewModel ScriptsListViewModel { get; protected set; }
    public MainMenuViewModel MainMenuViewModel { get; private set; }

    private bool onceLoaded;
    private string openedFilePath;
    private Stack<ViewModelBase> contentAreaViewModelStack;

    #endregion

    #region Commands

    public Command ControlLoadedCommand { get; private set; }

    private void InitCommands()
    {
      this.ControlLoadedCommand = new Command(
        x => { this.ControlLoaded(); },
        x => true
        );
    }

    #endregion

    private void ShowInContentArea(ViewModelBase viewModel)
    {
      if (this.contentAreaViewModelStack.Count > 0 &&
          this.contentAreaViewModelStack.Peek().GetType() == viewModel.GetType())
        this.contentAreaViewModelStack.Pop();
      this.contentAreaViewModelStack.Push(viewModel);
      this.OnPropertyChanged(nameof(this.AdditionalContentAreaViewModel));
    }

    private void HideFromContentArea(ViewModelBase viewModel)
    {
      if (this.contentAreaViewModelStack.Peek() != viewModel)
        return;
       
      this.contentAreaViewModelStack.Pop();
      this.OnPropertyChanged(nameof(this.AdditionalContentAreaViewModel));
    }

    private void ControlLoaded()
    {
      if (this.onceLoaded)
        return;

      if (!this.Variables.Items.Any())
        this.Variables.LoadFromSettings();

      var pinnedItems = PinnedItemModel.LoadFromSettings();
      foreach (var item in pinnedItems)
        this.MainMenuViewModel.PinnedItemsAppend(item);

      this.onceLoaded = true;
    }

    private void OnShowScriptDetailsRequested(ScriptDetailsViewModel scriptDetailsViewModel)
    {
      scriptDetailsViewModel.ClosingRequested += this.OnScriptDetailsViewModelClosingRequested;
      this.ShowInContentArea(scriptDetailsViewModel);
    }

    private void OnScriptDetailsViewModelClosingRequested(ScriptDetailsViewModel scriptDetailsViewModel)
    {
      scriptDetailsViewModel.ClosingRequested -= this.OnScriptDetailsViewModelClosingRequested;
      this.HideFromContentArea(scriptDetailsViewModel);
    }

    private void LoadScriptsFromFile(string filePath)
    {
      this.ScriptsListViewModel.OpenFromFile(filePath);
      this.OpenedFilePath = filePath;
    }

    private void OnMainMenuViewModelSaveRequested(string filePath)
    {
      this.ScriptsListViewModel.SaveToFile(filePath);
    }

    private void OnMainMenuViewModelPinRequested()
    {
      var pinnedItemViewModel = this.CreateNewPinnedItemViewModel();
      pinnedItemViewModel.ClosingRequested += this.OnPinnedItemViewModelClosingRequested;
      pinnedItemViewModel.PinAccepted += this.OnPinnedItemViewModelPinAccepted;
      this.ShowInContentArea(pinnedItemViewModel);
    }

    private void OnPinnedItemViewModelClosingRequested(PinnedItemViewModel pinnedItemViewModel)
    {
      pinnedItemViewModel.ClosingRequested -= this.OnPinnedItemViewModelClosingRequested;
      pinnedItemViewModel.PinAccepted -= this.OnPinnedItemViewModelPinAccepted;
      this.HideFromContentArea(pinnedItemViewModel);
    }

    private void OnPinnedItemViewModelPinAccepted(PinnedItemViewModel pinnedItemViewModel)
    {
      this.MainMenuViewModel.PinnedItemsAppend(pinnedItemViewModel.PinnedItem);
      this.MainMenuViewModel.SelectedPinnedItemName = pinnedItemViewModel.PinnedItem.Name;
      pinnedItemViewModel.ClosingRequested -= this.OnPinnedItemViewModelClosingRequested;
      pinnedItemViewModel.PinAccepted -= this.OnPinnedItemViewModelPinAccepted;
      this.HideFromContentArea(pinnedItemViewModel);
    }

    private void OnPinnedItemViewModelApplyPinRequested(PinnedItemViewModel pinnedItemViewModel)
    {
      pinnedItemViewModel.ClosingRequested -= this.OnPinnedItemViewModelClosingRequested;
      pinnedItemViewModel.PinAccepted -= this.OnPinnedItemViewModelPinAccepted;
      pinnedItemViewModel.ApplyPinRequested -= this.OnPinnedItemViewModelApplyPinRequested;
      this.HideFromContentArea(pinnedItemViewModel);
      this.LoadScriptsFromFile(pinnedItemViewModel.PinnedItem.Path);
    }

    private PinnedItemViewModel CreateNewPinnedItemViewModel()
    {
      var pinnedItem = new PinnedItemModel();
      if (!string.IsNullOrWhiteSpace(this.OpenedFilePath))
      {
        pinnedItem.Path = this.OpenedFilePath;
        pinnedItem.Name = Path.GetFileNameWithoutExtension(this.OpenedFilePath);
      }
      return new PinnedItemViewModel(pinnedItem);
    }

    private void OnMainMenuVievModelSelectedPinnedItemNameChanged(PinnedItemModel newPinnedItemModel)
    {
      if (newPinnedItemModel == null)
        return;

      var pinnedItemViewModel = new PinnedItemViewModel(newPinnedItemModel);
      pinnedItemViewModel.ClosingRequested += this.OnPinnedItemViewModelClosingRequested;
      pinnedItemViewModel.PinAccepted += this.OnPinnedItemViewModelPinAccepted;
      pinnedItemViewModel.ApplyPinRequested += this.OnPinnedItemViewModelApplyPinRequested;
      this.ShowInContentArea(pinnedItemViewModel);
    }

    #region ctors

    public MainViewModel()
    {
      this.contentAreaViewModelStack = new Stack<ViewModelBase>();
      this.MainMenuViewModel = new MainMenuViewModel();
      this.MainMenuViewModel.OpenRequested += this.LoadScriptsFromFile;
      this.MainMenuViewModel.SaveRequested += this.OnMainMenuViewModelSaveRequested;
      this.MainMenuViewModel.PinRequested += this.OnMainMenuViewModelPinRequested;
      this.MainMenuViewModel.SelectedPinnedItemNameChanged += this.OnMainMenuVievModelSelectedPinnedItemNameChanged;
      this.Variables = new Variables();
      this.Outputs = Outputs.Instance;
      this.ScriptsListViewModel = new ScriptsListViewModel();
      this.ScriptsListViewModel.ShowScriptDetailsRequested += this.OnShowScriptDetailsRequested;
      this.ShowInContentArea(this.Outputs);
      this.InitCommands();
    }

    #endregion
  }
}
