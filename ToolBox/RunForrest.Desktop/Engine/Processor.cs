using System.Diagnostics;

namespace RunForrest.Desktop.Engine
{
  public sealed class Processor
  {
    #region ctors

    private Processor() { }

    #endregion

    #region Singletone impl.

    private static Processor instance;
    public static Processor Instance
    {
      get
      {
        if (instance == null)
          instance = new Processor();
        return instance;
      }
    }

    #endregion

    public Process GetNewProcess(ProcessOptions options)
    {
      var process = new Process();

      if (options == null)
        return process;

      process.EnableRaisingEvents = options.EnableRaisingEvents;
      process.StartInfo = options.StartInfo;
      
      return process;
    }

  }
}
