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

    public ScriptDetailsViewModel(ScriptModel script) : this()
    {
      this.script = script;
    }

    public ScriptDetailsViewModel() : base()
    {
      this.script = new ScriptModel();
    }
  }
}
