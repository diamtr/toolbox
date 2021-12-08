using RunForrest.Desktop.Engine;
using System.Diagnostics;
using System.Globalization;
using System.Text;

namespace RunForrest.Desktop
{
  public class ScriptModel
  {
    public const string ExecutableFileName = @"cmd.exe";

    public string Text { get; set; }
    public string WorkingDirectory { get; set; }
    public string Comment { get; set; }

    public ProcessOptions GetProcessOptions()
    {
      var startInfo = new ProcessStartInfo();
      startInfo.FileName = ExecutableFileName;
      startInfo.Arguments = string.Format("/c ", this.Text);
      startInfo.WorkingDirectory = this.WorkingDirectory;
      startInfo.CreateNoWindow = true;
      startInfo.UseShellExecute = false;
      startInfo.StandardErrorEncoding = Encoding.GetEncoding(CultureInfo.CurrentCulture.TextInfo.OEMCodePage);
      startInfo.StandardOutputEncoding = Encoding.GetEncoding(CultureInfo.CurrentCulture.TextInfo.OEMCodePage);
      startInfo.RedirectStandardError = true;
      startInfo.RedirectStandardOutput = true;
      startInfo.RedirectStandardInput = true;

      var options = new ProcessOptions();
      options.EnableRaisingEvents = true;
      options.StartInfo = startInfo;

      return options;
    }
  }
}
