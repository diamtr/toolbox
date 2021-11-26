using System.Collections.ObjectModel;
using ToolBox.Desktop.Base;

namespace RunForrest.Desktop
{
  public class MainMenuViewModel : ViewModelBase
  {
    public ObservableCollection<string> PinnedItems { get; set; }
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

    public Command OpenCommand { get; private set; }
    public Command SaveCommand { get; private set; }
    public Command CleanCommand { get; private set; }
    public Command PinCommand { get; private set; }

    private void InitCommands()
    {
      this.OpenCommand = new Command(
        x => { },
        x => true
        );

      this.SaveCommand = new Command(
        x => { },
        x => true
        );

      this.CleanCommand = new Command(
        x => { },
        x => true
        );

      this.PinCommand = new Command(
        x => { },
        x => true
        );
    }

    public MainMenuViewModel() : base()
    {
      this.PinnedItems = new ObservableCollection<string>();
      this.PinnedItems.Add(@"D:\repos\toolbox\ToolBox\RunForrest.Desktop\MainViewModel.cs");
      this.PinnedItems.Add(@"D:\repos\toolbox\ToolBox\RunForrest.Desktop\bin\Debug\netcoreapp3.1\ToolBox.Desktop.Base.dll");
      this.InitCommands();
    }
  }
}
