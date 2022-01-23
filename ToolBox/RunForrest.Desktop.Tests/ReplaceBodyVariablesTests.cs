using NUnit.Framework;
using System.Collections.Generic;

namespace RunForrest.Desktop.Tests
{
  [TestFixture]
  public class ReplaceScriptBodyVariablesTests
  {
    private List<VariableData> variables;
    private ScriptData script;

    [SetUp]
    public void SetUp()
    {
      this.script = new ScriptData();
      this.variables = new List<VariableData>()
      {
        new VariableData() { Name = "branch", Value = "origin/master" },
        new VariableData() { Name = "mode", Value = "--hard" }
      };
    }

    [TearDown]
    public void TearDown()
    {
      this.variables = null;
      this.script = null;
    }

    [Test]
    public void WithoutVariables()
    {
      var body = @"git reset {mode} {no_variable} {branch}";
      this.variables = null;
      this.script.Body = body;
      Assert.AreEqual(body, this.script.SubstituteVriables(this.variables).Body);
    }
  }

  class ReplaceScriptBodyVariablesCases
  {
    static object[] Cases =
    {
      new object[] { "", "" },
      new object[] { "git fetch", "git fetch" },
      new object[] { "git fetch {branch}", "git fetch origin/master" },
      new object[] { "git fetch {branch} {branch}", "git fetch origin/master origin/master" },
      new object[] { "git fetch {branch {branch}", "git fetch {branch origin/master" },
      new object[] { "git fetch {branch} branch}", "git fetch origin/master branch}" },
      new object[] { "git fetch {bra {branch} nch}", "git fetch {bra origin/master nch}" },
      new object[] { "git reset {mode} {branch}", "git reset --hard origin/master" },
      new object[] { "git reset {mode} {no_variable} {branch}", "git reset --hard {no_variable} origin/master" }
    };
  }
}
