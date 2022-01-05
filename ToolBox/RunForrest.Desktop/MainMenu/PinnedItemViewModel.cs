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
        return this.PinnedItem.Name;
      }
      set
      {
        this.PinnedItem.Name = value;
        this.OnPropertyChanged();
      }
    }
    public string Path
    {
      get
      {
        return this.PinnedItem.Path;
      }
      set
      {
        this.PinnedItem.Path = value;
        this.OnPropertyChanged();
      }
    }
    public PinnedItemModel PinnedItem { get; private set; }

    public event Action<PinnedItemViewModel> ClosingRequested;
    public event Action<PinnedItemViewModel> PinAccepted;
    public event Action<PinnedItemViewModel> ApplyPinRequested;

    public Command RequestClosingCommand { get; private set; }
    public Command AcceptPinCommand { get; private set; }
    public Command ApplyPinCommand { get; private set; }

    private void InitCommands()
    {
      this.RequestClosingCommand = new Command(
        x => { this.ClosingRequested?.Invoke(this); },
        x => true
        );

      this.AcceptPinCommand = new Command(
        x => { this.AcceptPin(); },
        x => true
        );

      this.ApplyPinCommand = new Command(
        x => { this.ApplyPinRequested?.Invoke(this); },
        x => true
        );
    }

    private void AcceptPin()
    {
      this.PinnedItem.SaveInSettings();
      this.PinAccepted?.Invoke(this);
    }

    public PinnedItemViewModel() : this(new PinnedItemModel())
    {
    }

    public PinnedItemViewModel(PinnedItemModel pinnedItem) : base()
    {
      this.PinnedItem = pinnedItem;
      this.InitCommands();
    }
  }
}
