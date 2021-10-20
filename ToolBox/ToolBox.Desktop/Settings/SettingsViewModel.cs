using ToolBox.Desktop.Base;

namespace ToolBox.Desktop
{
  public class SettingsViewModel : ViewModelBase
  {
    private const string SettingsOwnerName = "ToolBox";
    private const string DefaultToolsPath = ".\\Tools";
    private const bool DefaultRememberLastTool = true;

    public string ToolsPath
    {
      get
      {
        return this.toolsPath;
      }
      set
      {
        this.toolsPath = value;
        this.OnPropertyChanged();
      }
    }
    public bool RememberLastTool
    {
      get
      {
        return this.rememberLastTool;
      }
      set
      {
        this.rememberLastTool = value;
        this.OnPropertyChanged();
      }
    }
    public string LastDisplayedTool;

    private string toolsPath;
    private bool rememberLastTool;
    private Settings settings;

    public Command SaveSettingsCommand { get; private set; }

    private void InitCommands()
    {
      this.SaveSettingsCommand = new Command(
        x => { this.SaveSettings(); },
        x => true
      );
    }

    public void LoadSettings()
    {
      this.ToolsPath = this.settings.Get<string>(nameof(this.ToolsPath), DefaultToolsPath);
      this.RememberLastTool = this.settings.Get<bool>(nameof(this.RememberLastTool), DefaultRememberLastTool);
      this.LastDisplayedTool = this.settings.Get<string>(nameof(this.LastDisplayedTool));
    }

    public void SaveSettings()
    {
      this.settings.Add(nameof(this.ToolsPath), this.ToolsPath);
      this.settings.Add(nameof(this.RememberLastTool), this.RememberLastTool);
      this.settings.Add(nameof(this.LastDisplayedTool), this.LastDisplayedTool);
    }

    public SettingsViewModel()
    {
      this.settings = Settings.GetSettings(SettingsOwnerName);
      this.InitCommands();
    }
  }
}
