using System;
using ToolBox.Desktop.Base;

namespace RunForrest.Desktop
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

    #endregion

    public event EventHandler<AdditionalContentAreaType> AdditionalContentAreaTypeChanged;

    private void OnAdditionalContentAreaTypeChanged()
    {
      this.AdditionalContentAreaTypeChanged?.Invoke(this, this.additionalContentAreaType);
    }

    public ControlPanel()
    {
      this.AdditionalContentAreaType = AdditionalContentAreaType.Empty;
    }
  }
}
