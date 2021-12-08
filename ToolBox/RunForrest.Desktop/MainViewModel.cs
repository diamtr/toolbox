using System.Collections;
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
    
    public string OpenedFilePath
    {
      get
      {
        return this.openedFilePath;
      }
      set
      {
        this.openedFilePath = value;
        this.OnPropertyChanged();
      }
    }
    public ViewModelBase AdditionalContentAreaViewModel
    {
      get { return this.additionalContentAreaViewModel; }
      protected set { this.additionalContentAreaViewModel = value; this.OnPropertyChanged(); }
    }
    public Variables Variables { get; protected set; }
    public Outputs Outputs { get; protected set; }
    public ScriptsListViewModel ScriptsListViewModel { get; protected set; }
    public MainMenuViewModel MainMenuViewModel { get; private set; }

    private string openedFilePath;
    private ViewModelBase additionalContentAreaViewModel;
    private TaskCompletionSource<bool> scriptsExecutionStopTcs;
    private CancellationTokenSource scriptsExecutionCts;

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
      this.Outputs.Clear();

      var scriptVmsToExecute = this.GetScriptsToExecute();
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
            script.OutputCatched += this.Outputs.Append;
            script.RunScript(this.Variables.Items);
            script.OutputCatched -= this.Outputs.Append;
            this.Outputs.Append(string.Empty);
          }
        }
        finally
        {
        }
      },
      cancellationToken);
    }

    private void StopAllScriptExecution()
    {
      this.scriptsExecutionStopTcs?.TrySetResult(true);
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
        this.Outputs.Clear();
      }
    }

    private void ControlLoaded()
    {
      if (!this.Variables.Items.Any())
        this.Variables.LoadFromSettings();
    }

    private void ControlUnloaded()
    {
    }

    private void OnShowScriptDetailsRequested(ScriptDetailsViewModel sender)
    {
      sender.ClosingRequested += this.OnScriptDetailsViewModelClosingRequested;
      this.AdditionalContentAreaViewModel = sender;
    }

    private void OnScriptDetailsViewModelClosingRequested(ScriptDetailsViewModel sender)
    {
      sender.ClosingRequested -= this.OnScriptDetailsViewModelClosingRequested;
      this.AdditionalContentAreaViewModel = this.Outputs;
    }

    private void OnMainMenuViewModelOpenRequested(string filePath)
    {
      this.OpenedFilePath = filePath;
    }

    private void OnMainMenuViewModelSaveRequested(string filePath)
    {

    }

    private void OnMainMenuViewModelPinRequested()
    {

    }

    #region ctors

    public MainViewModel()
    {
      this.MainMenuViewModel = new MainMenuViewModel();
      this.MainMenuViewModel.OpenRequested += this.OnMainMenuViewModelOpenRequested;
      this.MainMenuViewModel.SaveRequested += this.OnMainMenuViewModelSaveRequested;
      this.MainMenuViewModel.PinRequested += this.OnMainMenuViewModelPinRequested;
      this.Variables = new Variables();
      this.Outputs = Outputs.Instance;
      this.ScriptsListViewModel = new ScriptsListViewModel();
      this.ScriptsListViewModel.PropertyChanged += this.OnScriptsChanged;
      this.ScriptsListViewModel.ShowScriptDetailsRequested += this.OnShowScriptDetailsRequested;

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
