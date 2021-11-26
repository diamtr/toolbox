using System;
using System.Collections.ObjectModel;
using System.Linq;
using ToolBox.Desktop.Base;

namespace RunForrest.Desktop
{
  public class Variables : ViewModelBase
  {
    #region Constants

    private const string SettingsOwnerName = "RunForrest";

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
        x => { this.Save(); },
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

    public void Save()
    {
      var settings = Settings.GetSettings(SettingsOwnerName);
      foreach (var variable in this.Items)
      {
        if (settings.HasSetting(variable.Name))
          settings.Update(variable.Name, variable.Value);
        else
          settings.Add(variable.Name, variable.Value);
      }
      this.LastSaveDateTime = DateTime.Now;
    }

    public void LoadFromSettings()
    {
      var settings = Settings.GetSettings(SettingsOwnerName);
      var variablesList = settings.GetAll();
      this.Items.Clear();
      foreach (var variable in variablesList)
        this.Items.Add(new VariableData() { Name = variable.Name, Value = variable.Value });
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
