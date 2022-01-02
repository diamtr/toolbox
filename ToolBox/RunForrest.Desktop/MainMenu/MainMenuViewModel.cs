using Microsoft.Win32;
using System;
using System.Collections.ObjectModel;
using ToolBox.Desktop.Base;

namespace RunForrest.Desktop
{
  public class MainMenuViewModel : ViewModelBase
  {
    public ObservableCollection<PinnedItemModel> PinnedItems { get; set; }
    public string SelectedPinnedItem
    { 
      get
      {
        return this.selectedPinnedItem;
      }
      set
      {
        this.selectedPinnedItem = value;
        this.OnPropertyChanged();
      }
    }

    private string selectedPinnedItem;

    public event Action<string> OpenRequested;
    public event Action<string> SaveRequested;
    public event Action PinRequested;

    public Command OpenCommand { get; private set; }
    public Command SaveCommand { get; private set; }
    public Command PinCommand { get; private set; }

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
      this.PinnedItems = new ObservableCollection<PinnedItemModel>();
      this.InitCommands();
    }
  }
}
