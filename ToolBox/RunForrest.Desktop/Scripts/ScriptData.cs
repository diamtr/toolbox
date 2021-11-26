using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace RunForrest.Desktop
{
  public class ScriptData
  {
    #region Fields & Properties

    public string Body { get; set; }
    public string WorkingDirectory { get; set; }
    public string Comment { get; set; }

    #endregion

    public static string Serialize(IEnumerable<ScriptData> scripts)
    {
      var result = new StringBuilder();

      foreach(var script in scripts)
      {
        // Add script comment
        if (!string.IsNullOrWhiteSpace(script.Comment))
        {
          foreach (var commentLine in script.Comment.Split(new string[] { Environment.NewLine }, StringSplitOptions.None))
          {
            result.AppendFormat("REM {0}", commentLine);
            result.AppendLine();
          }
        }

        // Add script working directory change
        if (!string.IsNullOrWhiteSpace(script.WorkingDirectory))
        {
          result.AppendFormat("cd {0}", script.WorkingDirectory);
          result.AppendLine();
        }

        // Add script body
        if (!string.IsNullOrWhiteSpace(script.Body))
          result.AppendLine(script.Body);
      }

      return result.ToString();
    }

    public static List<ScriptData> Deserialize(string[] content)
    {
      var scripts = new List<ScriptData>();
      var comment = new StringBuilder();
      var workingDirectory = string.Empty;

      foreach (var line in content)
      {
        // If the line is a comment
        if (line.StartsWith("REM", StringComparison.InvariantCultureIgnoreCase))
        {
          // Cut first 3 chars and go next
          comment.AppendLine();
          comment.Append(line.Substring(3).Trim());
          continue;
        }

        // If the line is a single change directory command
        if (line.StartsWith("cd", StringComparison.InvariantCultureIgnoreCase) &&
            !line.Contains("&&"))
        {
          // Cut first 2 chars and go next
          workingDirectory = line.Substring(2).Trim();
          continue;
        }

        scripts.Add(new ScriptData()
        {
          Body = line,
          WorkingDirectory = workingDirectory,
          Comment = comment.ToString().Trim()
        });
        comment = new StringBuilder();
        workingDirectory = string.Empty;
      }

      return scripts;
    }

    public ScriptData SubstituteVriables(IEnumerable<VariableData> variables)
    {
      var result = new ScriptData();
      result.Body = ReplaceVariablesIn(this.Body, variables);
      result.WorkingDirectory = ReplaceVariablesIn(this.WorkingDirectory, variables);
      result.Comment = this.Comment;
      return result;
    }

    private string ReplaceVariablesIn(string source, IEnumerable<VariableData> variables)
    {
      var result = source;
      if (variables == null || !variables.Any())
        return result;

      var matches = Regex.Matches(source, @"{\w+}");
      foreach (Match match in matches)
      {
        if (!match.Success)
          continue;

        var varName = match.Value.Trim(new char[] { '{', '}' });
        if (!variables.Any(x => x.Name == varName))
          continue;

        result = result.Replace(match.Value, variables.First(x => x.Name == varName).Value);
      }

      return result;
    }
  }
}
