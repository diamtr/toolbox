using System.Collections.ObjectModel;
using ToolBox.Desktop.Base;

namespace MinionCopy.Desktop
{
  public class MainViewModel : ViewModelBase
  {
    private const string SettingsOwnerName = "MinionCopy";

    public CopyFromListStrategyViewModel CopyFromListStrategyViewModel
    {
      get
      {
        return this.copyFromListStrategyViewModel;
      }
      private set
      {
        this.copyFromListStrategyViewModel = value;
        this.OnPropertyChanged();
      }
    }
    public ObservableCollection<CopyException> CopyExceptions { get; set; }

    private CopyFromListStrategyViewModel copyFromListStrategyViewModel;
    private Settings settings;

    public Command CopyCommand { get; private set; }
    public Command UserControlLoadedCommand { get; private set; }

    private void Copy()
    {
      if (this.CopyFromListStrategyViewModel == null)
        return;

      this.CopyFromListStrategyViewModel.Copy();
      this.RefreshCopyExceptions();
    }

    private void RefreshCopyExceptions()
    {
      this.CopyExceptions.Clear();
      var exceptions = this.CopyFromListStrategyViewModel.GetCopyExceptions();
      foreach (var exception in exceptions)
        this.CopyExceptions.Add(exception);
    }

    private void UserControlLoaded()
    {
      var lastStrategyPath = this.settings.Get<string>(nameof(this.CopyFromListStrategyViewModel.LastStrategyPath));
      if (string.IsNullOrWhiteSpace(lastStrategyPath))
        return;
      this.CopyFromListStrategyViewModel.OpenCopyFromListStrategy(lastStrategyPath);
    }

    private void CopyFromListStrategyViewModel_LastStrategyPathChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
      if (e.PropertyName != nameof(this.CopyFromListStrategyViewModel.LastStrategyPath))
        return;
      this.settings.Add(nameof(this.CopyFromListStrategyViewModel.LastStrategyPath), this.CopyFromListStrategyViewModel.LastStrategyPath);
    }

    private void InitCommands()
    {
      this.CopyCommand = new Command(
        x => { this.Copy(); },
        x => true
        );

      this.UserControlLoadedCommand = new Command(
        x => { this.UserControlLoaded(); },
        x => true
        );
    }

    public MainViewModel()
    {
      this.settings = Settings.GetSettings(SettingsOwnerName);
      this.CopyFromListStrategyViewModel = new CopyFromListStrategyViewModel();
      this.CopyFromListStrategyViewModel.PropertyChanged += CopyFromListStrategyViewModel_LastStrategyPathChanged;
      this.CopyExceptions = new ObservableCollection<CopyException>();
      this.InitCommands();
    }
  }
}
