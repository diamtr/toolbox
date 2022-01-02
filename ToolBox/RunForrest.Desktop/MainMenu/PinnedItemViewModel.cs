using System;
using ToolBox.Desktop.Base;

namespace RunForrest.Desktop
{
  public class PinnedItemViewModel : ViewModelBase
  {
    public string Name
    {
      get
      {
        return this.pinnedItem.Name;
      }
      set
      {
        this.pinnedItem.Name = value;
        this.OnPropertyChanged();
      }
    }
    public string Path
    {
      get
      {
        return this.pinnedItem.Path;
      }
      set
      {
        this.pinnedItem.Path = value;
        this.OnPropertyChanged();
      }
    }

    public event Action<PinnedItemViewModel> ClosingRequested;

    public Command RequestClosingCommand { get; private set; }

    private PinnedItemModel pinnedItem;

    private void InitCommands()
    {
      this.RequestClosingCommand = new Command(
        x => { this.ClosingRequested?.Invoke(this); },
        x => true
        );
    }

    public PinnedItemViewModel() : this(new PinnedItemModel())
    {
    }

    public PinnedItemViewModel(PinnedItemModel pinnedItem) : base()
    {
      this.pinnedItem = pinnedItem;
      this.InitCommands();
    }
  }
}
