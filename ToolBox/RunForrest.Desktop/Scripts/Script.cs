using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using ToolBox.Desktop.Base;

namespace RunForrest.Desktop
{
  public class Script : ViewModelBase
  {
    #region Fields & Properties

    private ScriptData scriptData;
    public ScriptData ScriptData
    {
      get { return this.scriptData; }
      set { this.scriptData = value; this.OnPropertyChanged(); }
    }

    private bool isChecked;
    public bool IsChecked
    {
      get { return this.isChecked; }
      set { this.isChecked = value; this.OnPropertyChanged(); }
    }

    private bool isRunning;
    public bool IsRunning
    {
      get { return this.isRunning; }
      set { this.isRunning = value; this.OnPropertyChanged(); }
    }

    #endregion

    #region Events & Handlers

    public event Action<string> OutputCatched;
    public event Action<int> ScriptExited;
    public event Action ScriptDataViewClosed;

    #endregion

    #region Commands

    public Command AbortScriptCommand { get; private set; }
    public Command CloseScriptDataViewCommand { get; private set; }

    private void InitCommands()
    {
      this.AbortScriptCommand = new Command(
        x => { this.Abort(); },
        x => true
        );

      this.CloseScriptDataViewCommand = new Command(
        x => { this.ScriptDataViewClosed?.Invoke(); },
        x => true
        );
    }

    #endregion

    #region Command Methods

    public void Abort()
    {
      ProcessEnvironment.StopCurrentProcessExecution();
      this.IsRunning = false;
    }

    #endregion

    public bool HasBlankScriptData()
    {
      return string.IsNullOrWhiteSpace(this.ScriptData.Body) &&
        string.IsNullOrWhiteSpace(this.ScriptData.Comment) &&
        string.IsNullOrWhiteSpace(this.ScriptData.WorkingDirectory);
    }

    public void RunScript(IEnumerable<VariableData> variables)
    {
      if (string.IsNullOrWhiteSpace(this.ScriptData.Body))
        return;
      this.IsRunning = true;
      var data = this.ScriptData.SubstituteVriables(variables);
      ProcessEnvironment.InitNewCmdProcess();
      ProcessEnvironment.AppendCurrentProcessArguments(data.Body);
      if (!string.IsNullOrWhiteSpace(data.WorkingDirectory))
        ProcessEnvironment.Instance.CurrentProcess.StartInfo.WorkingDirectory = data.WorkingDirectory;
      ProcessEnvironment.Instance.CurrentProcess.OutputDataReceived += this.CatchOutputData;
      ProcessEnvironment.Instance.CurrentProcess.ErrorDataReceived += this.CatchOutputData;
      ProcessEnvironment.Instance.CurrentProcess.Exited += this.CatchScriptExited;
      ProcessEnvironment.RunCurrentProcess();
      this.IsRunning = false;
    }

    private void CatchOutputData(object sender, DataReceivedEventArgs outLine)
    {
      if (string.IsNullOrWhiteSpace(outLine.Data))
        return;

      this.OutputCatched?.Invoke(outLine.Data);
    }

    private void CatchScriptExited(object sender, EventArgs e)
    {
      var process = sender as Process;
      if (process == null)
        return;
      this.ScriptExited?.Invoke(process.ExitCode);
    }

    #region ctors

    public Script()
    {
      this.ScriptData = new ScriptData();
      this.InitCommands();
    }

    #endregion
  }
}
