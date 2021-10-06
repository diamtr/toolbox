using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Controls;
using ToolBox.Desktop.Base;

namespace ToolBox.Desktop
{
  public class MainViewModel : ViewModelBase
  {
    public const string MainWindowDefaultTitle = "Tool Box";

    public string MainWindowTitle
    {
      get
      { 
        return this.mainWindowTitle;
      }
      private set
      {
        this.mainWindowTitle = value;
        this.OnPropertyChanged();
      }
    }
    public SettingsViewModel SettingsViewModel
    {
      get
      {
        return this.settingsViewModel;
      }
      set
      {
        this.settingsViewModel = value;
        this.OnPropertyChanged();
      }
    }
    public UserControl SelectedTool
    {
      get
      {
        return this.selectedTool;
      }
      set
      {
        this.selectedTool = value;
        this.OnPropertyChanged();
      }
    }
    public string SelectedToolName
    {
      get
      {
        return this.selectedToolName;
      }
      set
      {
        this.selectedToolName = value;
        this.OnPropertyChanged();
        if (this.SelectedToolNameChanged != null)
          this.SelectedToolNameChanged.Invoke();
      }
    }
    public ObservableCollection<string> ToolsDisplayNames { get; private set; }
    public List<IDesktopTool> Tools { get; private set; }

    public event Action SelectedToolNameChanged;

    private string mainWindowTitle;
    private SettingsViewModel settingsViewModel;
    private UserControl selectedTool;
    private string selectedToolName;

    public Command WindowLoadedCommand { get; private set; }
    public Command WindowClosingCommand { get; private set; }

    private void InitCommands()
    {
      this.WindowLoadedCommand = new Command(
        x => { this.OnWindowLoaded(); },
        x => true
        );

      this.WindowClosingCommand = new Command(
        x => { this.OnWindowClosing(); },
        x => true
        );
    }

    private void OnWindowLoaded()
    {
      this.SettingsViewModel.LoadSettings();
      this.LoadTools();
      var lastDisplayedTool = this.SettingsViewModel.Settings.LastDisplayedTool;
      if (this.ToolsDisplayNames.Contains(lastDisplayedTool))
        this.SelectedToolName = lastDisplayedTool;
    }

    private void OnWindowClosing()
    {
      this.SettingsViewModel.Settings.LastDisplayedTool = this.SelectedToolName;
      this.SettingsViewModel.SaveSettings();
    }

    private void OnSelectedToolNameChanged()
    {
      var tool = this.Tools.FirstOrDefault(x => x.DisplayName == this.SelectedToolName);
      if (tool == null)
        return;
      this.SelectedTool = tool.GetUserControl();
      this.MainWindowTitle = $"{MainWindowDefaultTitle} - {this.SelectedToolName}";
    }

    private void LoadTools()
    {
      var tools = ToolsProvider.LoadTools(SettingsProvider.GetSettings());
      if (ToolsProvider.HasLoadException)
        return;
      this.Tools.Clear();
      this.Tools.AddRange(tools);
      this.ToolsDisplayNames.Clear();
      foreach (var tool in this.Tools)
        this.ToolsDisplayNames.Add(tool.DisplayName);
    }

    public MainViewModel()
    {
      this.MainWindowTitle = MainWindowDefaultTitle;
      this.ToolsDisplayNames = new ObservableCollection<string>();
      this.Tools = new List<IDesktopTool>();
      this.SettingsViewModel = new SettingsViewModel();
      this.InitCommands();
      this.SelectedToolNameChanged += this.OnSelectedToolNameChanged;
    }
  }
}
