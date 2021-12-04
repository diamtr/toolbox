using System;
using ToolBox.Desktop.Base;

namespace RunForrest.Desktop
{
  public class ScriptViewModel : ViewModelBase
  {
    public bool IsMuteChecked
    {
      get
      {
        return this.isMuteChecked;
      }
      set
      {
        this.isMuteChecked = value;
        this.OnPropertyChanged();
        if (this.isMuteChecked && this.IsSoloChecked)
          this.IsSoloChecked = false;
      }
    }
    public bool IsSoloChecked
    {
      get
      {
        return this.isSoloChecked;
      }
      set
      {
        this.isSoloChecked = value;
        this.OnPropertyChanged();
        if (this.isSoloChecked && this.IsMuteChecked)
          this.IsMuteChecked = false;
      }
    }

    public event Action<ScriptViewModel> ShowDetailsRequested;
    public event Action<ScriptViewModel> RemoveRequested;

    private bool isMuteChecked;
    private bool isSoloChecked;

    public Command ShowDetailsCommand { get; private set; }
    public Command RemoveCommand { get; private set; }

    private void InitCommands()
    {
      this.ShowDetailsCommand = new Command(
        x => { this.ShowDetailsRequested?.Invoke(this); },
        x => true
        );

      this.RemoveCommand = new Command(
        x => { this.RemoveRequested?.Invoke(this); },
        x => true
        );
    }

    public ScriptViewModel() : base()
    {
      this.InitCommands();
    }
  }
}
