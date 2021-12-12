using System;
using System.Diagnostics;
using System.Threading.Tasks;

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

    public ProcessExecutionInfo GetProcessExecutionInfo(ProcessOptions options)
    {
      if (options == null)
        throw new ArgumentNullException(nameof(options));

      var process = new Process();
      process.EnableRaisingEvents = options.EnableRaisingEvents;
      process.StartInfo = options.StartInfo;

      return new ProcessExecutionInfo(process, this.GetConsoleExecutionTask(process));
    }

    public async Task Execute(ProcessExecutionInfo processExecutionInfo)
    {
      processExecutionInfo.ExecutionTask.Start();
      await Task.Run(() =>
      {
        var completedTaskIndex = Task.WaitAny(processExecutionInfo.ExecutionTask,
                                              processExecutionInfo.ExecutionTaskCompletionSource.Task);
        // Close ExecutionTaskCompletionSource if execution ended.
        if (completedTaskIndex == 0)
          processExecutionInfo.ExecutionTaskCompletionSource.TrySetCanceled();
        // Break ExecutionTask if execution breaked.
        if (completedTaskIndex == 1)
        { /* put code here */ }
      });
    }

    private Task GetConsoleExecutionTask(Process process)
    {
      return new Task(() =>
      {
        process.Start();
        process.BeginErrorReadLine();
        process.BeginOutputReadLine();
        process.WaitForExit();
      });
    }
  }
}
