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

    public event Action<ScriptViewModel> DeleteRequested;

    private bool isMuteChecked;
    private bool isSoloChecked;

    public Command DeleteCommand { get; private set; }

    private void InitCommands()
    {
      this.DeleteCommand = new Command(
        x => { this.RaiseDeleteRequested(); },
        x => true
        );
    }

    private void RaiseDeleteRequested()
    {
      this.DeleteRequested?.Invoke(this);
    }

    public ScriptViewModel() : base()
    {
      this.InitCommands();
    }
  }
}
