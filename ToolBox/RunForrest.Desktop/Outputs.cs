using System.Diagnostics;
using System.Windows;
using ToolBox.Desktop.Base;

namespace RunForrest.Desktop
{
  public class Outputs : ViewModelBase
  {
    #region ctors

    private Outputs()
    {
      this.Items = new ObservableQueue<string>();
      this.MaxCount = 500;
    }

    #endregion

    #region Singletone impl.

    private static Outputs instance;
    public static Outputs Instance
    {
      get
      {
        if (instance == null)
          instance = new Outputs();
        return instance;
      }
    }

    #endregion

    public ObservableQueue<string> Items { get; set; }
    public int MaxCount { get; set; }

    public void Append(object sender, DataReceivedEventArgs e)
    {
      if (e == null)
        return;
      this.Append(e.Data ?? string.Empty);
    }

    public void Append(string line)
    {
      // From UI thread if possible.
      if (Application.Current != null)
        Application.Current.Dispatcher.Invoke(() => { this.Items.Enqueue(line); });
      else
        this.Items.Enqueue(line);

      if (this.MaxCount + 1 > this.Items.Count)
        return;

      if (Application.Current != null)
        Application.Current.Dispatcher.Invoke(() => { this.Items.Dequeue(); });
      else
        this.Items.Dequeue();
    }

    public void Clear()
    {
      if (Application.Current != null)
        Application.Current.Dispatcher.Invoke(() => { this.Items.Clear(); });
      else
        this.Items.Clear();
    }
  }
}
