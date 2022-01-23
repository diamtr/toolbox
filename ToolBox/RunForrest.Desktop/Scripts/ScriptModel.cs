using RunForrest.Desktop.Engine;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RunForrest.Desktop
{
  public class ScriptModel
  {
    public const string ExecutableFileName = @"cmd.exe";

    public string Text { get; set; }
    public string WorkingDirectory { get; set; }
    public string Comment { get; set; }

    public override string ToString()
    {
      var sb = new StringBuilder();

      if (!string.IsNullOrWhiteSpace(this.Comment))
        sb.AppendLine($"rem {this.Comment}");

      if (!string.IsNullOrWhiteSpace(this.WorkingDirectory))
        sb.AppendLine($"cd {this.WorkingDirectory}");

      if (!string.IsNullOrWhiteSpace(this.Text))
        sb.AppendLine(this.Text);

      return sb.ToString();
    }

    public static List<ScriptModel> Deserialize(IEnumerable<string> rawText)
    {
      var scripts = new List<ScriptModel>();

      if (rawText == null || !rawText.Any())
        return scripts;

      var newScript = new ScriptModel();
      foreach (var textLine in rawText)
      {
        if (string.IsNullOrWhiteSpace(textLine))
          continue;
        if (textLine.StartsWith("rem", System.StringComparison.InvariantCultureIgnoreCase))
        {
          newScript.Comment = textLine.Substring(3).Trim();
          continue;
        }
        if (textLine.StartsWith("cd", System.StringComparison.InvariantCultureIgnoreCase))
        {
          newScript.WorkingDirectory = textLine.Substring(2).Trim();
          continue;
        }
        newScript.Text = textLine;
        scripts.Add(newScript);
        newScript = new ScriptModel();
      }

      return scripts;
    }

    public async Task Run()
    {
      var options = this.GetProcessOptions();
      var processExecutionInfo = Processor.Instance.GetProcessExecutionInfo(options);
      processExecutionInfo.Process.OutputDataReceived += Outputs.Instance.Append;
      processExecutionInfo.Process.ErrorDataReceived += Outputs.Instance.Append;
      await Processor.Instance.Execute(processExecutionInfo);
    }

    private ProcessOptions GetProcessOptions()
    {
      var startInfo = new ProcessStartInfo();
      startInfo.FileName = ExecutableFileName;
      startInfo.Arguments = string.Format("/c {0}", this.Text);
      startInfo.WorkingDirectory = this.WorkingDirectory;
      startInfo.CreateNoWindow = true;
      startInfo.UseShellExecute = false;
      startInfo.RedirectStandardError = true;
      startInfo.RedirectStandardOutput = true;
      startInfo.RedirectStandardInput = true;
      Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
      startInfo.StandardErrorEncoding = Encoding.GetEncoding(CultureInfo.CurrentCulture.TextInfo.OEMCodePage);
      startInfo.StandardOutputEncoding = Encoding.GetEncoding(CultureInfo.CurrentCulture.TextInfo.OEMCodePage);

      var options = new ProcessOptions();
      options.EnableRaisingEvents = true;
      options.StartInfo = startInfo;

      return options;
    }
  }
}
