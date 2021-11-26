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
#warning AttachToSelectedItemCloseEvent()
        //this.AttachToSelectedItemCloseEvent(value);
        this.OnPropertyChanged();
      }
    }

    public ObservableCollection<ScriptViewModel> Items { get; set; }

    public Command AddNewScriptCommand { get; private set; }
    public Command RemoveSelectedScriptsCommand { get; private set; }

    private void InitCommands()
    {
      this.AddNewScriptCommand = new Command(
        x => { this.CreateNewScript(); },
        x => true
        );

      this.RemoveSelectedScriptsCommand = new Command(
        x => { this.RemoveSelectedScript(); },
        x => true
        );
    }

    private void CreateNewScript()
    {
      var newScript = new ScriptViewModel();
      this.Items.Add(newScript);
      this.SelectedItem = newScript;
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
