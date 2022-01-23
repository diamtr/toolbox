using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;

namespace RunForrest.Desktop.Engine
{
  public class ProcessExecutionInfo
  {
    public ProcessExecutionInfo(Process process, Task task)
    {
      this.Process = process;
      this.ExecutionTask = task;
      this.ExecutionTaskCompletionSource = new TaskCompletionSource<bool>();
    }

    public Process Process { get; private set; }
    public Task ExecutionTask { get; private set; }
    public TaskCompletionSource<bool> ExecutionTaskCompletionSource { get; private set; }
  }
}
