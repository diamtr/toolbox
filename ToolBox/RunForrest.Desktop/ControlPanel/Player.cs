using System;
using ToolBox.Desktop.Base;

namespace RunForrest.Desktop
{
  public class Player: ViewModelBase
  {
    private int total;
    public int Total
    {
      get { return this.total; }
      set { this.total = value; this.OnPropertyChanged(); }
    }

    private int current;
    public int Current
    {
      get { return this.current; }
      set { this.current = value; this.OnPropertyChanged(); }
    }

    public event Action Play;
    public event Action Stop;
    public event Action Forward;

    public Command PlayCommand { get; private set; }
    public Command StopCommand { get; private set; }
    public Command ForwardCommand { get; private set; }

    private void InitCommands()
    {
      this.PlayCommand = new Command(
        x => { this.Play?.Invoke(); },
        x => true
        );

      this.StopCommand = new Command(
        x => { this.Stop?.Invoke(); },
        x => true
        );

      this.ForwardCommand = new Command(
        x => { this.Forward?.Invoke(); },
        x => true
        );
    }

    public void Reset()
    {
      this.Current = 0;
      this.Total = 1;
    }

    public void Reset(int total)
    {
      this.Current = 0;
      this.Total = total;
    }

    public void IncCurrent()
    {
      this.Current++;
    }

    public Player()
    {
      this.Reset();
      this.Stop += this.Reset;
      this.InitCommands();
    }
  }
}
