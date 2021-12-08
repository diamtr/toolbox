using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using ToolBox.Desktop.Base;

namespace RunForrest.Desktop
{
  public class ScriptsListViewModel : ViewModelBase
  {
    private ScriptViewModel selectedItem;
    public ScriptViewModel SelectedItem
    {
      get { return this.selectedItem; }
      set
      {
        this.selectedItem = value;
        this.OnPropertyChanged();
      }
    }

    public ObservableCollection<ScriptViewModel> Items { get; set; }

    public event Action<ScriptDetailsViewModel> ShowScriptDetailsRequested;

    public Command AddNewScriptCommand { get; private set; }
    public Command RemoveSelectedScriptsCommand { get; private set; }
    public Command RunScriptsCommand { get; private set; }

    private void InitCommands()
    {
      this.AddNewScriptCommand = new Command(
        x => { this.AddNewScript(); },
        x => true
        );

      this.RemoveSelectedScriptsCommand = new Command(
        x => { this.RemoveSelectedScript(); },
        x => true
        );

      this.RunScriptsCommand = new Command(
        x => { this.RunScripts(); },
        x => true
        );
    }

    private void AddNewScript()
    {
      var newScript = new ScriptViewModel();
      newScript.RemoveRequested += this.OnScriptViewModelRemoveRequested;
      newScript.ShowDetailsRequested += this.OnScriptViewModelShowDetailsRequested;
      this.Items.Add(newScript);
    }

    private void OnScriptViewModelRemoveRequested(ScriptViewModel sender)
    {
      if (sender == null)
        return;

      sender.RemoveRequested -= this.OnScriptViewModelRemoveRequested;
      if (this.Items.Any(x => Equals(x, sender)))
        this.Items.Remove(sender);
    }

    private void OnScriptViewModelShowDetailsRequested(ScriptDetailsViewModel sender)
    {
      if (sender == null)
        return;

      this.ShowScriptDetailsRequested?.Invoke(sender);
    }

    private void RunScripts()
    {
      var scripts = this.GetScriptsToRun();
      foreach (var script in scripts)
        script.Run();
    }

    private List<ScriptViewModel> GetScriptsToRun()
    {
      var scripts = this.Items.Where(x => x.IsSoloChecked);
      if (scripts.Any())
        return scripts.ToList();

      return this.Items.Where(x => !x.IsMuteChecked).ToList();
    }

    private void AttachToSelectedItemCloseEvent(Script newValue)
    {
      if (newValue == null)
        return;
      newValue.ScriptDataViewClosed += this.OnSelectedScriptClose;
    }

    private void OnSelectedScriptClose()
    {
      if (this.SelectedItem == null)
        return;

      //this.SelectedItem.ScriptDataViewClosed -= this.OnSelectedScriptClose;
      this.SelectedItem = null;
    }

    private void RemoveSelectedScript()
    {
      if (this.SelectedItem == null)
        return;
      this.Items.Remove(this.SelectedItem);
    }

    public void AbortRunningScriptExecution()
    {
      //var script = this.Items.SingleOrDefault(x => x.IsRunning);
      //if (script == null)
      //  return;
      //script.Abort();
    }

    public void LoadFromFile(object sender, string path)
    {
      //var scripts = ScriptData.Deserialize(File.ReadAllLines(path))
      //  .Select(x => new Script() { ScriptData = x });
      //this.Items.Clear();
      //foreach (var script in scripts)
      //  this.Items.Add(script);
    }

    public void SaveToFile(object sender, string path)
    {
      //File.WriteAllText(path, ScriptData.Serialize(this.Items.Select(x => x.ScriptData)));
    }

    public ScriptsListViewModel()
    {
      this.Items = new ObservableCollection<ScriptViewModel>();
      this.InitCommands();
    }
  }
}
