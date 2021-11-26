using System;

namespace RunForrest.Desktop.Tests
{
  class BatchSerializationCases
  {
    const string ScriptBody = @"ping google.com";
    const string ScriptWorkingDirectory = @"D:\tmp";
    const string ScriptOneLineComment = @"Script comment";

    static object[] Cases =
    {
      new object[]
      {
        string.Empty, string.Empty, string.Empty,
        string.Empty
      },
      new object[]
      {
        ScriptBody, string.Empty, string.Empty,
        string.Format("{0}{1}", ScriptBody, Environment.NewLine)
      },
      new object[]
      {
        string.Empty, ScriptWorkingDirectory, string.Empty,
        string.Format("cd {0}{1}", ScriptWorkingDirectory, Environment.NewLine)
      },
      new object[]
      {
        string.Empty, string.Empty, ScriptOneLineComment,
        string.Format("REM {0}{1}", ScriptOneLineComment, Environment.NewLine)
      },
      new object[]
      {
        string.Empty, string.Empty, string.Join(Environment.NewLine, new string[] { ScriptOneLineComment, ScriptOneLineComment }),
        string.Format("REM {0}{1}REM {0}{1}", ScriptOneLineComment, Environment.NewLine)
      },
      new object[]
      {
        ScriptBody, ScriptWorkingDirectory, string.Empty,
        string.Format("cd {0}{2}{1}{2}", ScriptWorkingDirectory, ScriptBody, Environment.NewLine)
      },
      new object[]
      {
        ScriptBody, string.Empty, ScriptOneLineComment,
        string.Format("REM {0}{2}{1}{2}", ScriptOneLineComment, ScriptBody, Environment.NewLine)
      },
      new object[]
      {
        string.Empty, ScriptWorkingDirectory, ScriptOneLineComment,
        string.Format("REM {0}{2}cd {1}{2}", ScriptOneLineComment, ScriptWorkingDirectory, Environment.NewLine)
      },
      new object[]
      {
        ScriptBody, ScriptWorkingDirectory, ScriptOneLineComment,
        string.Format("REM {0}{3}cd {1}{3}{2}{3}", ScriptOneLineComment, ScriptWorkingDirectory, ScriptBody, Environment.NewLine)
      },
      new object[]
      {
        ScriptBody, ScriptWorkingDirectory, string.Join(Environment.NewLine, new string[] { ScriptOneLineComment, ScriptOneLineComment }),
        string.Format("REM {0}{3}REM {0}{3}cd {1}{3}{2}{3}", ScriptOneLineComment, ScriptWorkingDirectory, ScriptBody, Environment.NewLine)
      }
    };
  }
}
