using System;
using ToolBox.Desktop.Base;

namespace RunForrest.Desktop
{
  public class ScriptDetailsViewModel : ViewModelBase
  {
    public string ScriptText
    {
      get
      {
        return this.script.Text;
      }
      set
      {
        this.script.Text = value;
        this.OnPropertyChanged();
      }
    }

    private ScriptModel script;

    public event Action<ScriptDetailsViewModel> ClosingRequested;

    public Command RequestClosingCommand { get; private set; }

    public void Refresh()
    {
      this.OnPropertyChanged(nameof(this.ScriptText));
    }

    private void InitCommands()
    {
      this.RequestClosingCommand = new Command(
        x => { this.ClosingRequested?.Invoke(this); },
        x => true
        );
    }

    public ScriptDetailsViewModel(ScriptModel script) : this()
    {
      this.script = script;
    }

    public ScriptDetailsViewModel() : base()
    {
      this.script = new ScriptModel();
      this.InitCommands();
    }
  }
}
