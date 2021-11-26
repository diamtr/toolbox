using ToolBox.Desktop.Base;

namespace RunForrest.Desktop
{
  public class ScriptViewModel : ViewModelBase
  {

    public Command DeleteCommand { get; private set; }
    public Command MuteCommand { get; private set; }
    public Command SoloCommand { get; private set; }
    public Command RunCommand { get; private set; }

    private void InitCommands()
    {
      this.DeleteCommand = new Command(
        x => { },
        x => true
        );

      this.MuteCommand = new Command(
        x => { },
        x => true
        );

      this.SoloCommand = new Command(
        x => { },
        x => true
        );

      this.RunCommand = new Command(
        x => { },
        x => true
        );
    }

    public ScriptViewModel() : base()
    {
      this.InitCommands();
    }
  }
}
