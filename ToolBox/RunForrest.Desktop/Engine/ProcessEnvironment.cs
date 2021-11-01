using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Management;
using System.Text;
using System.Threading.Tasks;

namespace RunForrestPlugin
{
  public class ProcessEnvironment
  {
    #region ctors

    private ProcessEnvironment() { }

    #endregion

    #region Singletone impl.
    
    private static ProcessEnvironment instance;
    public static ProcessEnvironment Instance
    {
      get
      {
        if (instance == null)
          instance = new ProcessEnvironment();
        return instance;
      }
    }

    #endregion

    public Process CurrentProcess { get; private set; }
    public Task CurrentProcessExecutionTask { get; private set; }
    public TaskCompletionSource<bool> CurrentProcessStopExecutionTaskCompletionSource { get; private set; }

    public static void InitNewCmdProcess()
    {
      Instance.CurrentProcess = new Process();
      Instance.CurrentProcess.EnableRaisingEvents = true;
      Instance.CurrentProcess.StartInfo.FileName = "cmd.exe";
      Instance.CurrentProcess.StartInfo.Arguments = "/c";
      Instance.CurrentProcess.StartInfo.CreateNoWindow = true;
      Instance.CurrentProcess.StartInfo.UseShellExecute = false;
      Instance.CurrentProcess.StartInfo.StandardErrorEncoding = Encoding.GetEncoding(CultureInfo.CurrentCulture.TextInfo.OEMCodePage);
      Instance.CurrentProcess.StartInfo.StandardOutputEncoding = Encoding.GetEncoding(CultureInfo.CurrentCulture.TextInfo.OEMCodePage);
      Instance.CurrentProcess.StartInfo.RedirectStandardError = true;
      Instance.CurrentProcess.StartInfo.RedirectStandardOutput = true;
      Instance.CurrentProcess.StartInfo.RedirectStandardInput = false;
    }

    public static void AppendCurrentProcessArguments(params object[] args)
    {
      Instance.CurrentProcess.StartInfo.Arguments =
        string.Format("{0} {1}", Instance.CurrentProcess.StartInfo.Arguments, string.Join(" ", args));
    }

    public static void RunCurrentProcess()
    {
      Instance.InitNewStopCurrentProcessExecutionCompletionSource();
      Instance.InitCurrentProcessExecutionTask();
      Instance.CurrentProcessExecutionTask.Start();

      var completedTaskIndex = Task.WaitAny(Instance.CurrentProcessExecutionTask,
                                            Instance.CurrentProcessStopExecutionTaskCompletionSource.Task);

      if (completedTaskIndex == 1)
        FinalizeCurrentProcess();
      if (completedTaskIndex == 0)
        Instance.CurrentProcessStopExecutionTaskCompletionSource.TrySetCanceled();

      Instance.CurrentProcess = null;
      Instance.CurrentProcessStopExecutionTaskCompletionSource = null;
      Instance.CurrentProcessExecutionTask = null;
    }

    public static void StopCurrentProcessExecution()
    {
      if (Instance.CurrentProcess == null)
        return;
      if (Instance.CurrentProcessStopExecutionTaskCompletionSource == null)
        Instance.CurrentProcessStopExecutionTaskCompletionSource = new TaskCompletionSource<bool>();
      Instance.CurrentProcessStopExecutionTaskCompletionSource.TrySetResult(true);
    }

    public static void FinalizeCurrentProcess()
    {
      Instance.FinalizeProcess(Instance.CurrentProcess);
    }

    private void InitNewStopCurrentProcessExecutionCompletionSource()
    {
      this.CurrentProcessStopExecutionTaskCompletionSource = new TaskCompletionSource<bool>();
    }

    private void InitCurrentProcessExecutionTask()
    {
      this.CurrentProcessExecutionTask = new Task(() => { this.ExecuteProcess(this.CurrentProcess); });
    }

    private void ExecuteProcess(Process process)
    {
      process.Start();
      process.BeginErrorReadLine();
      process.BeginOutputReadLine();
      process.WaitForExit();
    }

    private void FinalizeProcess(Process process)
    {
      // Children die first
      this.FinalizeChildProcesses(process);
      if (process != null && !process.HasExited)
        // Method Kill do it WITHOUT children processes
        process.Kill();
    }

    private void FinalizeChildProcesses(Process process)
    {
      var childProcesses = this.GetChildProcesses32(process);
      foreach (var cp in childProcesses)
        FinalizeProcess(cp);
    }

    private List<Process> GetChildProcesses32(Process process)
    {
      var children = new List<Process>();
      var query = $"Select * From Win32_Process Where ParentProcessID={process.Id}";
      var childProcesses32 = new ManagementObjectSearcher(query).Get();
      foreach (var co in childProcesses32)
        children.Add(Process.GetProcessById(Convert.ToInt32(co["ProcessId"])));

      return children;
    }
  }
}
