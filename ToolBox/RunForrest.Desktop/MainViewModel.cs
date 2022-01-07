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
    public ViewModelBase ContentAreaViewModel
    {
      get
      {
        return this.Navigator.GetCurrent();
      }
    }
    public Variables Variables { get; protected set; }
    public Outputs Outputs { get; protected set; }
    public ScriptsListViewModel ScriptsListViewModel { get; protected set; }
    public MainMenuViewModel MainMenuViewModel { get; private set; }
    public ViewModelNavigator Navigator { get; private set; }

    private string openedFilePath;

    #endregion

    public static MainViewModel GetInstance()
    {
      var instance = new MainViewModel();
      instance.Outputs = Outputs.Instance;
      instance.Navigator.SetCurrent(instance.Outputs);

      if (!instance.Variables.Items.Any())
        instance.Variables.LoadFromSettings();

      var pinnedItems = PinnedItemModel.LoadFromSettings();
      foreach (var item in pinnedItems)
        instance.MainMenuViewModel.PinnedItemsAppend(item);

      return instance;
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
      this.Navigator.SetCurrent(pinnedItemViewModel);
    }

    private void OnPinnedItemViewModelClosingRequested(PinnedItemViewModel pinnedItemViewModel)
    {
      pinnedItemViewModel.ClosingRequested -= this.OnPinnedItemViewModelClosingRequested;
      pinnedItemViewModel.PinAccepted -= this.OnPinnedItemViewModelPinAccepted;
      this.Navigator.Forget(pinnedItemViewModel);
    }

    private void OnPinnedItemViewModelPinAccepted(PinnedItemViewModel pinnedItemViewModel)
    {
      this.MainMenuViewModel.PinnedItemsAppend(pinnedItemViewModel.PinnedItem);
      this.MainMenuViewModel.SelectedPinnedItemName = pinnedItemViewModel.PinnedItem.Name;
      pinnedItemViewModel.ClosingRequested -= this.OnPinnedItemViewModelClosingRequested;
      pinnedItemViewModel.PinAccepted -= this.OnPinnedItemViewModelPinAccepted;
      this.Navigator.Forget(pinnedItemViewModel);
    }

    private void OnPinnedItemViewModelApplyPinRequested(PinnedItemViewModel pinnedItemViewModel)
    {
      pinnedItemViewModel.ClosingRequested -= this.OnPinnedItemViewModelClosingRequested;
      pinnedItemViewModel.PinAccepted -= this.OnPinnedItemViewModelPinAccepted;
      pinnedItemViewModel.ApplyPinRequested -= this.OnPinnedItemViewModelApplyPinRequested;
      this.Navigator.Forget(pinnedItemViewModel);
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
      this.Navigator.SetCurrent(pinnedItemViewModel);
    }

    private void SetCurrentContent(ViewModelBase viewModel)
    {
      if (viewModel is IClosableViewModel)
        ((IClosableViewModel)viewModel).CloseRequested += this.Navigator.Forget;

      this.Navigator.SetCurrent(viewModel);
    }

    private void ViewModelNavigator_ViewModelChanged()
    {
      this.OnPropertyChanged(nameof(this.ContentAreaViewModel));
    }

    #region ctors

    public MainViewModel()
    {
      this.Navigator = new ViewModelNavigator();
      this.Navigator.ViewModelChanged += this.ViewModelNavigator_ViewModelChanged;
      this.MainMenuViewModel = new MainMenuViewModel();
      this.MainMenuViewModel.OpenRequested += this.LoadScriptsFromFile;
      this.MainMenuViewModel.SaveRequested += this.OnMainMenuViewModelSaveRequested;
      this.MainMenuViewModel.PinRequested += this.OnMainMenuViewModelPinRequested;
      this.MainMenuViewModel.SelectedPinnedItemNameChanged += this.OnMainMenuVievModelSelectedPinnedItemNameChanged;
      this.Variables = new Variables();
      this.ScriptsListViewModel = new ScriptsListViewModel();
      this.ScriptsListViewModel.ShowScriptDetailsRequested += this.SetCurrentContent;
    }

    #endregion
  }
}
