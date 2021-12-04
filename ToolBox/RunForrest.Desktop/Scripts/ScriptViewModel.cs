using System;
using System.ComponentModel;
using ToolBox.Desktop.Base;

namespace RunForrest.Desktop
{
  public class ScriptViewModel : ViewModelBase
  {
    public string ScriptText
    {
      get
      {
        return this.script.Text;
      }
      set
      {
        this.script.Text = value;
        this.OnPropertyChanged();
      }
    }
    public bool IsMuteChecked
    {
      get
      {
        return this.isMuteChecked;
      }
      set
      {
        this.isMuteChecked = value;
        this.OnPropertyChanged();
        if (this.isMuteChecked && this.IsSoloChecked)
          this.IsSoloChecked = false;
      }
    }
    public bool IsSoloChecked
    {
      get
      {
        return this.isSoloChecked;
      }
      set
      {
        this.isSoloChecked = value;
        this.OnPropertyChanged();
        if (this.isSoloChecked && this.IsMuteChecked)
          this.IsMuteChecked = false;
      }
    }

    public event Action<ScriptDetailsViewModel> ShowDetailsRequested;
    public event Action<ScriptViewModel> RemoveRequested;

    private ScriptModel script;
    private bool isMuteChecked;
    private bool isSoloChecked;

    public Command ShowDetailsCommand { get; private set; }
    public Command RemoveCommand { get; private set; }

    private void InitCommands()
    {
      this.ShowDetailsCommand = new Command(
        x => {
          var detailsViewModel = new ScriptDetailsViewModel(this.script);
          detailsViewModel.PropertyChanged += this.OnDetailsViewModelPropertyChanged;
          this.ShowDetailsRequested?.Invoke(detailsViewModel);
        },
        x => true
        );

      this.RemoveCommand = new Command(
        x => { this.RemoveRequested?.Invoke(this); },
        x => true
        );
    }

    private void OnDetailsViewModelPropertyChanged(object sender, PropertyChangedEventArgs e)
    {
      if (e.PropertyName == nameof(this.ScriptText))
        this.OnPropertyChanged(e.PropertyName);
    }

    public ScriptViewModel() : base()
    {
      this.script = new ScriptModel();
      this.InitCommands();
    }
  }
}
