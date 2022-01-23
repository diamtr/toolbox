using System;
using ToolBox.Desktop.Base;

namespace RunForrest.Desktop
{
  public class ScriptDetailsViewModel : ViewModelBase, IClosableViewModel
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

    public string WorkingDirectory
    {
      get
      {
        return this.script.WorkingDirectory;
      }
      set
      {
        this.script.WorkingDirectory = value;
        this.OnPropertyChanged();
      }
    }

    public string Comment
    {
      get
      {
        return this.script.Comment;
      }
      set
      {
        this.script.Comment = value;
        this.OnPropertyChanged();
      }
    }

    private ScriptModel script;

    public event Action<ViewModelBase> CloseRequested;

    public Command RequestClosingCommand { get; private set; }

    public void Refresh()
    {
      this.OnPropertyChanged(nameof(this.ScriptText));
    }

    private void InitCommands()
    {
      this.RequestClosingCommand = new Command(
        x => { this.CloseRequested?.Invoke(this); },
        x => true
        );
    }

    public ScriptDetailsViewModel() : this(new ScriptModel())
    {
    }

    public ScriptDetailsViewModel(ScriptModel script) : base()
    {
      this.script = script;
      this.InitCommands();
    }
  }
}
