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
    }

    private void OnMainMenuViewModelSaveRequested(string filePath)
    {
      this.ScriptsListViewModel.SaveToFile(filePath);
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
      this.ScriptsListViewModel.ShowScriptDetailsRequested += this.OnShowScriptDetailsRequested;
      this.AdditionalContentAreaViewModel = this.Outputs;
      this.InitCommands();
    }

    #endregion
  }
}
