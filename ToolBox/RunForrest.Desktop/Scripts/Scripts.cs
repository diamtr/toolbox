using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using ToolBox.Desktop.Base;

namespace RunForrest.Desktop
{
  public class Scripts : ViewModelBase
  {
    private Script selectedItem;
    public Script SelectedItem
    {
      get { return this.selectedItem; }
      set
      {
        this.selectedItem = value;
        this.AttachToSelectedItemCloseEvent(value);
        this.OnPropertyChanged();
      }
    }

    public ObservableCollection<Script> Items { get; set; }

    public Command AddNewScriptCommand { get; private set; }
    public Command RemoveSelectedScriptsCommand { get; private set; }

    private void InitCommands()
    {
      this.AddNewScriptCommand = new Command(
        x => { this.CreateNewItem(); },
        x => true
        );

      this.RemoveSelectedScriptsCommand = new Command(
        x => { this.RemoveSelectedItems(); },
        x => true
        );
    }

    private void CreateNewItem()
    {
      var newItem = new Script();
      this.Items.Add(newItem);
      this.SelectedItem = newItem;
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

      this.SelectedItem.ScriptDataViewClosed -= this.OnSelectedScriptClose;
      this.SelectedItem = null;
    }

    private void RemoveSelectedItems()
    {
      var selectedItems = this.Items.Where(x => x.IsChecked).ToList();
      foreach (var item in selectedItems)
        this.Items.Remove(item);
    }

    public void AbortRunningScriptExecution()
    {
      var script = this.Items.SingleOrDefault(x => x.IsRunning);
      if (script == null)
        return;
      script.Abort();
    }

    public void LoadFromFile(object sender, string path)
    {
      var scripts = ScriptData.Deserialize(File.ReadAllLines(path))
        .Select(x => new Script() { ScriptData = x });
      this.Items.Clear();
      foreach (var script in scripts)
        this.Items.Add(script);
    }

    public void SaveToFile(object sender, string path)
    {
      File.WriteAllText(path, ScriptData.Serialize(this.Items.Select(x => x.ScriptData)));
    }

    public Scripts()
    {
      this.Items = new ObservableCollection<Script>();
      this.InitCommands();
    }
  }
}
