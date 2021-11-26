using System;
using System.Collections.Generic;
using System.Linq;

namespace ToolBox.Desktop.Base
{
  public class Settings
  {
    public string Owner { get; private set; }

    public static Settings GetSettings(string owner)
    {
      return new Settings() { Owner = owner };
    }

    public void Add(string name, object value)
    {
      if (this.HasSetting(name))
      {
        this.Update(name, value);
        return;
      }

      var newSetting = new Setting();
      newSetting.Owner = this.Owner;
      newSetting.Name = name;
      newSetting.Value = value?.ToString() ?? string.Empty;

      using (var settingsContext = new SettingsContext())
      {
        settingsContext.Add(newSetting);
        settingsContext.SaveChanges();
      }
    }

    public T Get<T>(string name)
    {
      return this.Get<T>(name, default(T));
    }

    public T Get<T>(string name, T defaultValue)
    {
      var setting = this.GetAll().FirstOrDefault(x => x.Name == name);

      if (setting == null)
        return defaultValue;

      return (T)Convert.ChangeType(setting.Value, typeof(T));
    }

    public List<Setting> GetAll()
    {
      var ownerSettings = new List<Setting>();
      using (var settingsContext = new SettingsContext())
        ownerSettings.AddRange(settingsContext.Settings.Where(x => x.Owner == this.Owner));
      return ownerSettings;
    }

    public void Update(string name, object value)
    {
      if (!this.HasSetting(name))
        throw new Exception($"Setting {name} for {this.Owner} does not exist.");

      using (var settingsContext = new SettingsContext())
      {
        var setting = settingsContext.Settings.FirstOrDefault(x => x.Owner == this.Owner && x.Name == name);
        setting.Value = value?.ToString() ?? string.Empty;
        settingsContext.SaveChanges();
      }
    }

    public void Remove(string name)
    {
      using (var settingsContext = new SettingsContext())
      {
        var setting = settingsContext.Settings.FirstOrDefault(x => x.Owner == this.Owner && x.Name == name);
        settingsContext.Remove(setting);
        settingsContext.SaveChanges();
      }
    }

    public bool HasSetting(string name)
    {
      using (var settingsContext = new SettingsContext())
        return settingsContext.Settings.Any(x => x.Owner == this.Owner && x.Name == name);
    }

    private Settings()
    {
    }
  }
}
