using NUnit.Framework;

namespace RunForrestPlugin.Tests
{
  [TestFixture]
  public class OutputsTests
  {
    [Test]
    public void CreateOutputsClass()
    {
      var outputs = new Outputs();
      Assert.AreEqual(0, outputs.Items.Count);
    }

    [Test]
    public void OverMaxCount()
    {
      var outputs = new Outputs();
      Assert.AreEqual(0, outputs.Items.Count);
      outputs.MaxCount = 3;
      outputs.Append("line1");
      Assert.AreEqual(1, outputs.Items.Count);
      outputs.Append("line2");
      Assert.AreEqual(2, outputs.Items.Count);
      outputs.Append("line3");
      Assert.AreEqual(3, outputs.Items.Count);
      outputs.Append("line4");
      Assert.AreEqual(3, outputs.Items.Count);
      outputs.Append("line5");
      Assert.AreEqual(3, outputs.Items.Count);
    }

    [Test]
    public void AppendLines()
    {
      var outputs = new Outputs();
      var line1 = @"First line";
      outputs.Append(line1);
      Assert.AreEqual(1, outputs.Items.Count);
      var line2 = @"Second line";
      outputs.Append(line2);
      Assert.AreEqual(2, outputs.Items.Count);
      Assert.AreEqual(@"First line", outputs.Items.Dequeue());
      Assert.AreEqual(@"Second line", outputs.Items.Dequeue());
    }

    [Test]
    public void Clean()
    {
      var outputs = new Outputs();
      outputs.Append("line");
      outputs.Append("line");
      outputs.Append("line");
      outputs.Append("line");
      Assert.Greater(outputs.Items.Count, 0);
      outputs.Clear();
      Assert.Zero(outputs.Items.Count);
    }

  }
}
