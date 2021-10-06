namespace ToolBox.Desktop
{
  public class SettingsViewModel : ViewModelBase
  {
    public Settings Settings
    {
      get
      {
        return this.settings;
      }
      set
      {
        this.settings = value;
        this.OnPropertyChanged();
      }
    }

    private Settings settings;

    public Command SaveSettingsCommand { get; private set; }

    private void InitCommands()
    {
      this.SaveSettingsCommand = new Command(
        x => { SettingsProvider.SaveSettings(); },
        x => true
      );
    }

    public void LoadSettings()
    {
      this.Settings = SettingsProvider.GetSettings();
    }

    public void SaveSettings()
    {
      SettingsProvider.SaveSettings();
    }

    public SettingsViewModel()
    {
      this.InitCommands();
    }
  }
}
