using System.Collections.Generic;
using System.Linq;
using ToolBox.Desktop.Base;

namespace RunForrest.Desktop
{
  public class PinnedItemModel
  {
    private const string SettingsOwnerName = "RunForrestPinnedItem";

    public string Name { get; set; }
    public string Path { get; set; }

    public void SaveInSettings()
    {
      var settings = Settings.GetSettings(SettingsOwnerName);
      settings.Add(this.Name, this.Path);
    }

    public static List<PinnedItemModel> LoadFromSettings()
    {
      var settings = Settings.GetSettings(SettingsOwnerName).GetAll();
      return settings
        .Select(x => new PinnedItemModel() { Name = x.Name, Path = x.Value })
        .ToList();
    }
  }
}
