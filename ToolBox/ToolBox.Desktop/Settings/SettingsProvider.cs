using Newtonsoft.Json;
using System.IO;

namespace ToolBox.Desktop
{
  internal sealed class SettingsProvider
  {
    private const string SettingsFileName = "settings.json";

    internal static SettingsProvider Instance
    {
      get
      {
        if (instance == null)
          instance = new SettingsProvider();
        return instance;
      }
    }

    private Settings settings;
    private static SettingsProvider instance;

    public static Settings GetSettings()
    {
      if (Instance.settings == null)
        Instance.ReadSettings();

      return Instance.settings;
    }

    public static void SaveSettings()
    {
      var content = JsonConvert.SerializeObject(Instance.settings, Formatting.Indented);
      File.WriteAllText(SettingsFileName, content);
    }

    private void ReadSettings()
    {
      var content = File.ReadAllText(SettingsFileName);
      this.settings = JsonConvert.DeserializeObject<Settings>(content);
    }
  }
}
