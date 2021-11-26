using NUnit.Framework;
using System.Collections.Generic;

namespace RunForrest.Desktop.Tests
{
  [TestFixture]
  public class ScriptDataTests
  {
    private const string Empty = "";
    private const string DfltB = "ping yandex.ru";
    private const string DfltWD = "L:\\Aunch\\me\\here";
    private const string DfltC = "My tests is awersome";
    private const string VarB = "ping {site}";
    private const string VarWD = "L:\\Aunch\\{b_dir}\\here";
    private const string VarC = "My tests is {cmnt}";
    private const string RsltB = "ping google.com";
    private const string RsltWD = "L:\\Aunch\\dev\\here";
    private const string RsltC = "My tests is {cmnt}";

    private List<VariableData> variables = new List<VariableData>() {
      new VariableData() { Name = "site", Value = "google.com" },
      new VariableData() { Name = "b_dir", Value = "dev" },
      new VariableData() { Name = "cmmnt", Value = "fantastic" }
      };

    [Test]
    public void CreateNew()
    {
      var scriptData = new ScriptData();
      Assert.NotNull(scriptData);
      Assert.IsNull(scriptData.Body);
      Assert.IsNull(scriptData.WorkingDirectory);
      Assert.IsNull(scriptData.Comment);
    }

    [Test]
    [TestCase(Empty, Empty, Empty, Empty, Empty, Empty)]
    [TestCase(DfltB, Empty, Empty, DfltB, Empty, Empty)]
    [TestCase(Empty, DfltWD, Empty, Empty, DfltWD, Empty)]
    [TestCase(Empty, Empty, DfltC, Empty, Empty, DfltC)]
    [TestCase(DfltB, DfltWD, Empty, DfltB, DfltWD, Empty)]
    [TestCase(DfltB, Empty, DfltC, DfltB, Empty, DfltC)]
    [TestCase(Empty, DfltWD, DfltC, Empty, DfltWD, DfltC)]
    [TestCase(DfltB, DfltWD, DfltC, DfltB, DfltWD, DfltC)]
    [TestCase(VarB, Empty, Empty, RsltB, Empty, Empty)]
    [TestCase(Empty, VarWD, Empty, Empty, RsltWD, Empty)]
    [TestCase(Empty, Empty, VarC, Empty, Empty, RsltC)]
    [TestCase(VarB, VarWD, Empty, RsltB, RsltWD, Empty)]
    [TestCase(VarB, Empty, VarC, RsltB, Empty, RsltC)]
    [TestCase(Empty, VarWD, VarC, Empty, RsltWD, RsltC)]
    [TestCase(VarB, VarWD, VarC, RsltB, RsltWD, RsltC)]
    public void SubstituteVriables(string b, string wd, string c,
                                   string rb, string rwd, string rc)
    {
      var source = new ScriptData() { Body = b, WorkingDirectory = wd, Comment = c };
      var final = source.SubstituteVriables(this.variables);
      Assert.AreEqual(rb, final.Body);
      Assert.AreEqual(rwd, final.WorkingDirectory);
      Assert.AreEqual(rc, final.Comment);
    }
  }
}
