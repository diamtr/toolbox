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

    private string openedFilePath;
    private Stack<ViewModelBase> contentAreaViewModelStack;

    #endregion

    #region Commands

    public Command ControlLoadedCommand { get; private set; }
    public Command ControlUnloadedCommand { get; private set; }
    public Command CopyScriptsExecutionLogSelectedItemsCommand { get; private set; }

    private void InitCommands()
    {
      this.ControlLoadedCommand = new Command(
        x => { this.ControlLoaded(); },
        x => true
        );

      this.ControlUnloadedCommand = new Command(
        x => { this.ControlUnloaded(); },
        x => true
        );

      this.CopyScriptsExecutionLogSelectedItemsCommand = new Command(
        x => { this.CopyScriptsExecutionLogSelectedItems(x); },
        x => true
        );
    }

    #endregion

    #region Command Functions

    private void CopyScriptsExecutionLogSelectedItems(object parameter)
    {
      var items = ((IList)parameter).Cast<string>();
      if (items == null)
        return;

      var text = new StringBuilder();

      foreach (var item in items)
        text.AppendLine(item);

      Clipboard.SetData(DataFormats.Text, text.ToString());
    }

    #endregion

    private void ShowContentAreaViewModel(ViewModelBase viewModel)
    {
      if (this.contentAreaViewModelStack.Count > 0 &&
          this.contentAreaViewModelStack.Peek().GetType() == viewModel.GetType())
        this.contentAreaViewModelStack.Pop();
      this.contentAreaViewModelStack.Push(viewModel);
      this.OnPropertyChanged(nameof(this.AdditionalContentAreaViewModel));
    }

    private void HideContentAreaViewModel(ViewModelBase viewModel)
    {
      if (this.contentAreaViewModelStack.Peek() != viewModel)
        return;
       
      this.contentAreaViewModelStack.Pop();
      this.OnPropertyChanged(nameof(this.AdditionalContentAreaViewModel));
    }

    private void ControlLoaded()
    {
      if (!this.Variables.Items.Any())
        this.Variables.LoadFromSettings();
    }

    private void ControlUnloaded()
    {
    }

    private void OnShowScriptDetailsRequested(ScriptDetailsViewModel scriptDetailsViewModel)
    {
      scriptDetailsViewModel.ClosingRequested += this.OnScriptDetailsViewModelClosingRequested;
      this.ShowContentAreaViewModel(scriptDetailsViewModel);
    }

    private void OnScriptDetailsViewModelClosingRequested(ScriptDetailsViewModel scriptDetailsViewModel)
    {
      scriptDetailsViewModel.ClosingRequested -= this.OnScriptDetailsViewModelClosingRequested;
      this.HideContentAreaViewModel(scriptDetailsViewModel);
    }

    private void OnMainMenuViewModelOpenRequested(string filePath)
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
      this.ShowContentAreaViewModel(pinnedItemViewModel);
    }

    private void OnPinnedItemViewModelClosingRequested(PinnedItemViewModel pinnedItemViewModel)
    {
      pinnedItemViewModel.ClosingRequested -= this.OnPinnedItemViewModelClosingRequested;
      this.HideContentAreaViewModel(pinnedItemViewModel);
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

    #region ctors

    public MainViewModel()
    {
      this.contentAreaViewModelStack = new Stack<ViewModelBase>();
      this.MainMenuViewModel = new MainMenuViewModel();
      this.MainMenuViewModel.OpenRequested += this.OnMainMenuViewModelOpenRequested;
      this.MainMenuViewModel.SaveRequested += this.OnMainMenuViewModelSaveRequested;
      this.MainMenuViewModel.PinRequested += this.OnMainMenuViewModelPinRequested;
      this.Variables = new Variables();
      this.Outputs = Outputs.Instance;
      this.ScriptsListViewModel = new ScriptsListViewModel();
      this.ScriptsListViewModel.ShowScriptDetailsRequested += this.OnShowScriptDetailsRequested;
      this.ShowContentAreaViewModel(this.Outputs);
      this.InitCommands();
    }

    #endregion
  }
}
