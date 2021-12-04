﻿using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using ToolBox.Desktop.Base;

namespace RunForrest.Desktop
{
  public class MainViewModel : ViewModelBase
  {

    #region Fields & Properties

    private ViewModelBase additionalContentAreaViewModel;
    public ViewModelBase AdditionalContentAreaViewModel
    {
      get { return this.additionalContentAreaViewModel; }
      protected set { this.additionalContentAreaViewModel = value; this.OnPropertyChanged(); }
    }

    private TaskCompletionSource<bool> scriptsExecutionStopTcs;
    private CancellationTokenSource scriptsExecutionCts;

    public ControlPanel ControlPanel { get; protected set; }
    public Variables Variables { get; protected set; }
    public Outputs Outputs { get; protected set; }
    public ScriptsListViewModel ScriptsListViewModel { get; protected set; }
    public MainMenuViewModel MainMenuViewModel { get; private set; }

    #endregion

    #region Commands

    public Command ControlLoadedCommand { get; private set; }
    public Command ControlUnloadedCommand { get; private set; }
    public Command CopyScriptsExecutionLogSelectedItemsCommand { get; private set; }

    private void InitCommands()
    {
      this.ControlLoadedCommand = new Command(
        x => { this.ControlLoaded(); },
        x => true
        );

      this.ControlUnloadedCommand = new Command(
        x => { this.ControlUnloaded(); },
        x => true
        );

      this.CopyScriptsExecutionLogSelectedItemsCommand = new Command(
        x => { this.CopyScriptsExecutionLogSelectedItems(x); },
        x => true
        );
    }

    #endregion

    #region Command Functions

    private async void RunScriptsExecution()
    {
      //if (this.ScriptsListViewModel.Items.Any(x => x.IsRunning))
      //  return;

      this.Outputs.Clear();
      this.ControlPanel.AdditionalContentAreaType = AdditionalContentAreaType.Log;

      var scriptVmsToExecute = this.GetScriptsToExecute();

      this.ControlPanel.Player.Reset(scriptVmsToExecute.Count);

      await this.ExecuteScripts(scriptVmsToExecute);
    }

    private async Task ExecuteScripts(List<Script> scripts)
    {
      this.scriptsExecutionCts = new CancellationTokenSource();
      var cancellationToken = this.scriptsExecutionCts.Token;
      this.scriptsExecutionStopTcs = new TaskCompletionSource<bool>();

      var scriptsExecutionTask = this.GetScriptsExecutionTask(scripts, cancellationToken);
      scriptsExecutionTask.Start();

      await Task.Run(() =>
      {
        var completedTaskIndex = Task.WaitAny(scriptsExecutionTask, this.scriptsExecutionStopTcs.Task);
        if (completedTaskIndex == 1)
        {
          this.scriptsExecutionCts.Cancel();
          ProcessEnvironment.StopCurrentProcessExecution();
        }
      });

      this.scriptsExecutionCts = null;
      this.scriptsExecutionStopTcs = null;
    }

    private List<Script> GetScriptsToExecute()
    {
      //var hasCheckedScripts = this.ScriptsListViewModel.Items.Any(x => x.IsChecked);
      //var scriptVmsToExecute = this.ScriptsListViewModel.Items.ToList();

      //if (hasCheckedScripts)
      //  scriptVmsToExecute = scriptVmsToExecute.Where(x => x.IsChecked).ToList();

      return new List<Script>();
    }

    private Task GetScriptsExecutionTask(IEnumerable<Script> scripts,
                                         CancellationToken cancellationToken)
    {
      return new Task(() =>
      {
        #warning TODO what about exceptions
        try
        {
          foreach (var script in scripts)
          {
            if (cancellationToken.IsCancellationRequested)
              break;
            var toExecute = script.ScriptData.SubstituteVriables(this.Variables.Items);
            this.Outputs.Append(toExecute.Body);
            this.ControlPanel.NowExecuting = toExecute;
            script.OutputCatched += this.Outputs.Append;
            script.RunScript(this.Variables.Items);
            script.OutputCatched -= this.Outputs.Append;
            this.ControlPanel.Player.IncCurrent();
            this.Outputs.Append(string.Empty);
          }
        }
        finally
        {
          this.ControlPanel.NowExecuting = null;
        }
      },
      cancellationToken);
    }

    private void StopAllScriptExecution()
    {
      this.scriptsExecutionStopTcs?.TrySetResult(true);
    }

    private void RemoveScripts()
    {
      //var scripts = this.ScriptsListViewModel.Items.Where(x => !x.IsChecked);
      //this.ScriptsListViewModel.Items.Clear();
      //foreach (var script in scripts)
      //  this.ScriptsListViewModel.Items.Add(script);
    }

    private void CopyScriptsExecutionLogSelectedItems(object parameter)
    {
      var items = ((IList)parameter).Cast<string>();
      if (items == null)
        return;

      var text = new StringBuilder();

      foreach (var item in items)
        text.AppendLine(item);

      Clipboard.SetData(DataFormats.Text, text.ToString());
    }

    #endregion

    private void AdditionalContentAreaTypeChanged(object sender, AdditionalContentAreaType acat)
    {
      switch (acat)
      {
        case AdditionalContentAreaType.Variables:
          this.AdditionalContentAreaViewModel = this.Variables;
          break;
        case AdditionalContentAreaType.Log:
          this.AdditionalContentAreaViewModel = this.Outputs;
          break;
        case AdditionalContentAreaType.Script:
          this.AdditionalContentAreaViewModel = this.ScriptsListViewModel.SelectedItem;
          break;
        default:
          this.AdditionalContentAreaViewModel = null;
          break;
      }
    }

    private void OnScriptsChanged(object sender, PropertyChangedEventArgs e)
    {
      if (e.PropertyName == "Items")
      {
        this.ControlPanel.Player.Reset();
        this.Outputs.Clear();
      }

      if (e.PropertyName == "SelectedItem")
        if (this.ScriptsListViewModel.SelectedItem != null)
          this.ControlPanel.AdditionalContentAreaType = AdditionalContentAreaType.Script;
        else
          this.ControlPanel.AdditionalContentAreaType = AdditionalContentAreaType.Empty;
    }

    private void ControlLoaded()
    {
      if (!this.Variables.Items.Any())
        this.Variables.LoadFromSettings();
    }

    private void ControlUnloaded()
    {
    }

    private void ClearScripts()
    {
      this.ScriptsListViewModel.Items.Clear();
      this.Outputs.Clear();
    }

    private void OnShowScriptDetailsRequested(ScriptDetailsViewModel sender)
    {
      this.AdditionalContentAreaViewModel = sender;
    }

    #region ctors
    
    public MainViewModel()
    {
      this.MainMenuViewModel = new MainMenuViewModel();
      this.Variables = new Variables();
      this.Outputs = new Outputs();
      this.ScriptsListViewModel = new ScriptsListViewModel();
      this.ScriptsListViewModel.PropertyChanged += this.OnScriptsChanged;
      this.ScriptsListViewModel.ShowScriptDetailsRequested += this.OnShowScriptDetailsRequested;
      this.ControlPanel = new ControlPanel();
      this.ControlPanel.ClearScripts += this.ClearScripts;
      this.ControlPanel.LoadScriptsFromFile += this.ScriptsListViewModel.LoadFromFile;
      this.ControlPanel.SaveScriptsToFile += this.ScriptsListViewModel.SaveToFile;
      this.ControlPanel.AdditionalContentAreaTypeChanged += this.AdditionalContentAreaTypeChanged;
      this.ControlPanel.Player.Play += this.RunScriptsExecution;
      this.ControlPanel.Player.Stop += this.StopAllScriptExecution;
      this.ControlPanel.Player.Forward += this.ScriptsListViewModel.AbortRunningScriptExecution;

      this.InitCommands();
    }

    #endregion
  }

  public enum AdditionalContentAreaType
  {
    Log,
    Variables,
    Script,
    Empty
  }
}
