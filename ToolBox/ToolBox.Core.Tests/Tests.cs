using NUnit.Framework;

namespace ToolBox.Core.Tests
{
  [TestFixture]
  public class ToolsTests
  {
    [Test]
    public void CreateTools()
    {
      var tools = new Tools();
      Assert.IsEmpty(tools);
    }
  }
}