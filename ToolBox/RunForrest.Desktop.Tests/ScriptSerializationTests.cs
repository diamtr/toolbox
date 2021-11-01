using NUnit.Framework;
using System.Collections.Generic;
using TestStack.BDDfy;

namespace RunForrestPlugin.Tests
{
  [TestFixture]
  public class ScriptDataSerializationTests
  {
    ScriptData scriptData;
    string serializationResult;

    [TearDown]
    public void TearDown()
    {
      this.scriptData = null;
    }

    [Test]
    [TestCaseSource(typeof(BatchSerializationCases), "Cases")]
    public void SingleScriptBatchSerialization(string body, string workingDirectory, string comment, string result)
    {
      this.Given(_ => this.GivenNewlyScriptData(body, workingDirectory, comment))
        .When(_ => this.WhenScriptSerializedToBatch())
        .Then(_ => this.ThenSerializationResultIsCorrect(result))
        .BDDfy();
    }

    public void GivenNewlyScriptData(string body, string workingDirectory, string comment)
    {
      this.scriptData = new ScriptData();
      this.scriptData.Body = body;
      this.scriptData.WorkingDirectory = workingDirectory;
      this.scriptData.Comment = comment;
    }

    private void WhenScriptSerializedToBatch()
    {
      this.serializationResult = ScriptData.Serialize(new List<ScriptData>() { this.scriptData });
    }

    private void ThenSerializationResultIsCorrect(string etalon)
    {
      Assert.AreEqual(etalon, this.serializationResult);
    }

  }
}
