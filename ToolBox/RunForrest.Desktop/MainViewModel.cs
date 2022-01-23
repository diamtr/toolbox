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

      instance.Navigator.ViewModelChanged += () => { instance.OnPropertyChanged(nameof(instance.ContentAreaViewModel)); };
      instance.MainMenuViewModel.SelectedPinnedItemChanged += instance.SetCurrentContent;
      instance.ScriptsListViewModel.ShowScriptDetailsRequested += instance.SetCurrentContent;

      instance.Outputs = Outputs.Instance;
      instance.SetCurrentContent(instance.Outputs);

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

    private void MainMenuViewModel_SaveRequested(string filePath)
    {
      this.ScriptsListViewModel.SaveToFile(filePath);
    }

    private void MainMenuViewModel_PinRequested()
    {
      var pinnedItemViewModel = this.CreateNewPinnedItemViewModel();
      pinnedItemViewModel.PinAccepted += this.PinnedItemViewModel_PinAccepted;
      this.SetCurrentContent(pinnedItemViewModel);
    }

    private void MainMenuVievModel_SelectedPinnedItemChanged(PinnedItemViewModel selectedPinnedItemModel)
    {
      if (selectedPinnedItemModel == null)
        return;

      selectedPinnedItemModel.PinAccepted += this.PinnedItemViewModel_PinAccepted;
      selectedPinnedItemModel.ApplyPinRequested += this.PinnedItemViewModel_ApplyPinRequested;
      this.SetCurrentContent(selectedPinnedItemModel);
    }

    private void PinnedItemViewModel_PinAccepted(PinnedItemViewModel pinnedItemViewModel)
    {
      this.MainMenuViewModel.PinnedItemsAppend(pinnedItemViewModel.PinnedItem);
      this.MainMenuViewModel.SelectedPinnedItemName = pinnedItemViewModel.PinnedItem.Name;
      pinnedItemViewModel.PinAccepted -= this.PinnedItemViewModel_PinAccepted;
      this.ForgetContent(pinnedItemViewModel);
    }

    private void PinnedItemViewModel_ApplyPinRequested(PinnedItemViewModel pinnedItemViewModel)
    {
      pinnedItemViewModel.PinAccepted -= this.PinnedItemViewModel_PinAccepted;
      pinnedItemViewModel.ApplyPinRequested -= this.PinnedItemViewModel_ApplyPinRequested;
      this.ForgetContent(pinnedItemViewModel);
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

    private void SetCurrentContent(ViewModelBase viewModel)
    {
      if (viewModel == null)
        return;

      if (viewModel is IClosableViewModel)
        ((IClosableViewModel)viewModel).CloseRequested += this.ForgetContent;

      this.Navigator.SetCurrent(viewModel);
    }

    private void ForgetContent(ViewModelBase viewModel)
    {
      if (viewModel == null)
        return;

      if (viewModel is IClosableViewModel)
        ((IClosableViewModel)viewModel).CloseRequested -= this.ForgetContent;

      this.Navigator.Forget(viewModel);
    }

    #region ctors

    public MainViewModel()
    {
      this.Navigator = new ViewModelNavigator();
      this.MainMenuViewModel = new MainMenuViewModel();
      this.MainMenuViewModel.OpenRequested += this.LoadScriptsFromFile;
      this.MainMenuViewModel.SaveRequested += this.MainMenuViewModel_SaveRequested;
      this.MainMenuViewModel.PinRequested += this.MainMenuViewModel_PinRequested;
      this.MainMenuViewModel.SelectedPinnedItemChanged += this.MainMenuVievModel_SelectedPinnedItemChanged;
      this.Variables = new Variables();
      this.ScriptsListViewModel = new ScriptsListViewModel();
    }

    #endregion
  }
}
