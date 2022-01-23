using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using ToolBox.Desktop.Base;

namespace RunForrest.Desktop
{
  public class MainMenuViewModel : ViewModelBase
  {
    public ObservableCollection<string> PinnedItemNames { get; private set; }
    public string SelectedPinnedItemName
    { 
      get
      {
        return this.selectedPinnedItemName;
      }
      set
      {
        this.selectedPinnedItemName = value;
        this.OnPropertyChanged();
        var item = this.pinnedItems.FirstOrDefault(x => x.Name == this.selectedPinnedItemName);
        this.SelectedPinnedItemChanged?.Invoke(new PinnedItemViewModel(item));
      }
    }

    private string selectedPinnedItemName;
    private List<PinnedItemModel> pinnedItems;

    public event Action<string> OpenRequested;
    public event Action<string> SaveRequested;
    public event Action PinRequested;
    public event Action<PinnedItemViewModel> SelectedPinnedItemChanged;

    public Command OpenCommand { get; private set; }
    public Command SaveCommand { get; private set; }
    public Command PinCommand { get; private set; }

    public void PinnedItemsAppend(PinnedItemModel pinnedItem)
    {
      if (pinnedItems.Any(x => x.Name == pinnedItem.Name &&
                               x.Path == pinnedItem.Path))
        return;

      this.pinnedItems.Add(pinnedItem);
      this.PinnedItemNames.Add(pinnedItem.Name);
    }

    private void InitCommands()
    {
      this.OpenCommand = new Command(
        x => { this.OnOpen(); },
        x => true
        );

      this.SaveCommand = new Command(
        x => { this.OnSave(); },
        x => true
        );

      this.PinCommand = new Command(
        x => { this.PinRequested?.Invoke(); },
        x => true
        );
    }

    private void OnOpen()
    {
      var openDialog = new OpenFileDialog();
      openDialog.Filter = "Batch file (*.bat)|*.bat";
      openDialog.InitialDirectory = Environment.CurrentDirectory;
      openDialog.Multiselect = false;

      if (openDialog.ShowDialog() != true)
        return;

      this.OpenRequested?.Invoke(openDialog.FileName);
    }

    private void OnSave()
    {
      var saveDialog = new SaveFileDialog();
      saveDialog.Filter = "Batch file (*.bat)|*.bat";
      saveDialog.InitialDirectory = Environment.CurrentDirectory;

      if (saveDialog.ShowDialog() == true)
        this.SaveRequested?.Invoke(saveDialog.FileName);
    }

    public MainMenuViewModel() : base()
    {
      this.pinnedItems = new List<PinnedItemModel>();
      this.PinnedItemNames = new ObservableCollection<string>();
      this.InitCommands();
    }
  }
}
