using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Web.Script.Serialization;
using ToolBox.Shared;

namespace RunForrestPlugin
{
  public class Variables : ViewModelBase
  {
    #region Constants

    private const string DefaultVariablesFileName = @"rfp_variables.json";

    #endregion

    #region Fields & Props

    private DateTime? lastSaveDateTime;
    public DateTime? LastSaveDateTime
    {
      get
      {
        return this.lastSaveDateTime;
      }
      set
      {
        this.lastSaveDateTime = value;
        this.OnPropertyChanged();
      }
    }
    private VariableData newVariable;
    public VariableData NewVariable
    {
      get { return this.newVariable; }
      set { this.newVariable = value; this.OnPropertyChanged(); }
    }
    public ObservableCollection<VariableData> Items { get; private set; }

    #endregion

    #region Commands

    public Command AddVariableCommand { get; private set; }
    public Command DeleteVariableCommand { get; private set; }
    public Command SaveVariablesCommand { get; private set; }

    protected void InitCommands()
    {
      this.AddVariableCommand = new Command(
        x => { this.AddNewVariable(); },
        x => true
        );

      this.DeleteVariableCommand = new Command(
        x => { this.Delete(x); },
        x => true
        );

      this.SaveVariablesCommand = new Command(
        x => { this.SaveToFile(); },
        x => true
        );
    }

    #endregion

    #region Methods

    private void AddNewVariable()
    {
      if (this.NewVariable != null &&
          !string.IsNullOrWhiteSpace(this.NewVariable.Name) &&
          !this.Items.Any(x => x.Name == this.NewVariable.Name))
        this.Items.Add(this.NewVariable);
      this.NewVariable = new VariableData();
    }

    public void AddNewVariable(VariableData newVariable)
    {
      if (newVariable == null ||
          string.IsNullOrWhiteSpace(newVariable.Name) ||
          this.Items.Any(x => x.Name == newVariable.Name))
        return;
      this.Items.Add(newVariable);
    }

    protected void Delete(object parameter)
    {
      var variable = parameter as VariableData;
      if (variable == null)
        return;
      this.Delete(variable);
    }

    public void Delete(VariableData variable)
    {
      this.Items.Remove(variable);
    }

    public void SaveToFile(string path = DefaultVariablesFileName)
    {
      path = this.GetVariablesFilePath(path);
      var content = new StringBuilder();
      var jss = new JavaScriptSerializer();
      jss.Serialize(this.Items, content);
      File.WriteAllText(path, content.ToString());
      this.LastSaveDateTime = DateTime.Now;
    }

    public void LoadFromFile(string path = DefaultVariablesFileName)
    {
      path = this.GetVariablesFilePath(path);
      if (!File.Exists(path))
        return;
      var text = File.ReadAllText(path);
      var jss = new JavaScriptSerializer();
      var variablesList = jss.Deserialize<List<VariableData>>(text);
      this.Items.ClearAddRange(variablesList);
    }

    private string GetVariablesFilePath(string path)
    {
      var dir = System.AppDomain.CurrentDomain.BaseDirectory;
      if (string.IsNullOrWhiteSpace(path))
        return Path.Combine(dir, DefaultVariablesFileName);

      if (!Path.IsPathRooted(path))
        return Path.Combine(dir, path);

      return path;
    }

    #endregion

    #region ctors

    public Variables()
    {
      this.newVariable = new VariableData();
      this.Items = new ObservableCollection<VariableData>();
      this.InitCommands();
    }

    #endregion
  }
}
