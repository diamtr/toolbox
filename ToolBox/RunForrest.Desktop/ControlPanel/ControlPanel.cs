using Microsoft.Win32;
using System;
using ToolBox.Shared;

namespace RunForrestPlugin
{
  public class ControlPanel : ViewModelBase
  {
    #region Fields & Props

    private string openedFile;
    public string OpenedFile
    {
      get { return this.openedFile; }
      set { this.openedFile = value; this.OnPropertyChanged(); }
    }

    private AdditionalContentAreaType additionalContentAreaType;
    public AdditionalContentAreaType AdditionalContentAreaType
    {
      get { return this.additionalContentAreaType; }
      set
      {
        this.additionalContentAreaType = value;
        this.OnPropertyChanged();
        this.OnAdditionalContentAreaTypeChanged();
      }
    }

    private ScriptData nowExecuting;
    public ScriptData NowExecuting
    {
      get { return this.nowExecuting; }
      set { this.nowExecuting = value; this.OnPropertyChanged(); }
    }

    public Player Player { get; protected set; }

    public System.Collections.Generic.List<string> RiecentScripts { get; set; }

    #endregion

    #region Events

    public event Action ClearScripts;
    public event EventHandler<string> LoadScriptsFromFile;
    public event EventHandler<string> SaveScriptsToFile;
    public event EventHandler<AdditionalContentAreaType> AdditionalContentAreaTypeChanged;

    #endregion

    #region Commands

    public Command SaveScriptsFileCommand { get; private set; }
    public Command OpenScriptsFileCommand { get; private set; }
    public Command ClearScriptsCommand { get; private set; }

    private void InitCommands()
    {
      this.SaveScriptsFileCommand = new Command(
        x => { this.OnSaveScriptsFile(); },
        x => true
        );

      this.OpenScriptsFileCommand = new Command(
        x => { this.OnOpenScriptsFile(); },
        x => true
        );

      this.ClearScriptsCommand = new Command(
        x => { this.OnClearScripts(); },
        x => true
        );
    }

    #endregion

    private void OnSaveScriptsFile()
    {
      var saveDialog = new SaveFileDialog();
      saveDialog.Filter = "Batch file (*.bat)|*.bat";
      saveDialog.InitialDirectory = Environment.CurrentDirectory;

      if (saveDialog.ShowDialog() == true)
        this.SaveScriptsToFile?.Invoke(this, saveDialog.FileName);
    }

    private void OnOpenScriptsFile()
    {
      var openDialog = new OpenFileDialog();
      openDialog.Filter = "Batch file (*.bat)|*.bat";
      openDialog.InitialDirectory = Environment.CurrentDirectory;
      openDialog.Multiselect = false;

      if (openDialog.ShowDialog() != true)
        return;

      this.OpenedFile = openDialog.FileName;
      this.LoadScriptsFromFile?.Invoke(this, openDialog.FileName);
    }

    private void OnClearScripts()
    {
      this.OpenedFile = string.Empty;
      this.ClearScripts?.Invoke();
    }

    private void OnAdditionalContentAreaTypeChanged()
    {
      this.AdditionalContentAreaTypeChanged?.Invoke(this, this.additionalContentAreaType);
    }

    public ControlPanel()
    {
      this.RiecentScripts = new System.Collections.Generic.List<string>() { "Test 1", "Test 2", "Test 3" };
      this.Player = new Player();
      this.InitCommands();
      this.AdditionalContentAreaType = AdditionalContentAreaType.Empty;
    }
  }
}
