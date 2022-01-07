using System;
using ToolBox.Desktop.Base;

namespace RunForrest.Desktop
{
  public interface IClosableViewModel
  {
    public event Action<ViewModelBase> CloseRequested;
  }
}
