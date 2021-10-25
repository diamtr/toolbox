using System.Collections.ObjectModel;
using System.Linq;
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
    public ObservableCollection<ICopyDetailedResult> CopyExceptions { get; set; }
    public ObservableCollection<ICopyDetailedResult> CopyDetailedResults { get; set; }
    public ICopyDetailedResult SelectedCopyException
    {
      get
      {
        return this.selectedCopyException;
      }
      set
      {
        this.selectedCopyException = value;
        this.OnPropertyChanged();
      }
    }
    public ICopyDetailedResult SelectedCopyDetailedResult
    {
      get
      {
        return this.selectedCopyDetailedResult;
      }
      set
      {
        this.selectedCopyDetailedResult = value;
        this.OnPropertyChanged();
      }
    }
    public string TotalResults
    {
      get
      {
        return this.totalResults;
      }
      set
      {
        this.totalResults = value;
        this.OnPropertyChanged();
      }
    }

    private CopyFromListStrategyViewModel copyFromListStrategyViewModel;
    private Settings settings;
    private ICopyDetailedResult selectedCopyException;
    private ICopyDetailedResult selectedCopyDetailedResult;
    private string totalResults;

    public Command CopyCommand { get; private set; }
    public Command UserControlLoadedCommand { get; private set; }

    private void Copy()
    {
      if (this.CopyFromListStrategyViewModel == null)
        return;
      this.CopyFromListStrategyViewModel.Copy();
      this.RefreshCopyResults();
    }

    private void RefreshCopyResults()
    {
      var detailedResults = this.CopyFromListStrategyViewModel.GetCopyDetailedResults();
      this.CopyDetailedResults.Clear();
      foreach (var detailedResult in detailedResults)
        this.CopyDetailedResults.Add(detailedResult);

      this.CopyExceptions.Clear();
      var exceptions = detailedResults.Where(x => x.CopyResult == CopyResult.Failed);
      foreach (var exception in exceptions)
        this.CopyExceptions.Add(exception);

      this.TotalResults = $"Total: {this.CopyDetailedResults.Count}. Failed: {this.CopyExceptions.Count}.";
    }

    private void UserControlLoaded()
    {
      var lastStrategyPath = this.settings.Get<string>(nameof(this.CopyFromListStrategyViewModel.LastStrategyPath));
      if (string.IsNullOrWhiteSpace(lastStrategyPath))
        return;
      this.CopyFromListStrategyViewModel.OpenCopyFromListStrategy(lastStrategyPath);
    }

    private void MainViewModel_SelectedCopyExceptionChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
      if (e.PropertyName != nameof(this.SelectedCopyException))
        return;
      if (this.SelectedCopyException == null)
        return;
      this.CopyFromListStrategyViewModel.SetSelectedItem(this.SelectedCopyException.Owner);
    }

    private void MainViewModel_SelectedCopyDetailedResultChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
      if (e.PropertyName != nameof(this.SelectedCopyDetailedResult))
        return;
      if (this.SelectedCopyDetailedResult == null)
        return;
      this.CopyFromListStrategyViewModel.SetSelectedItem(this.SelectedCopyDetailedResult.Owner);
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
      this.CopyExceptions = new ObservableCollection<ICopyDetailedResult>();
      this.CopyDetailedResults = new ObservableCollection<ICopyDetailedResult>();
      this.PropertyChanged += MainViewModel_SelectedCopyExceptionChanged;
      this.PropertyChanged += MainViewModel_SelectedCopyDetailedResultChanged;
      this.InitCommands();
    }
  }
}
