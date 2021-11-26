using NUnit.Framework;
using System.Linq;

namespace RunForrest.Desktop.Tests
{
  [TestFixture]
  public class ScriptsTests
  {
    [Test]
    public void CreateScriptsClass()
    {
      var scripts = new Scripts();
      Assert.IsEmpty(scripts.Items);
    }

    [Test]
    public void AddNewCommand()
    {
      var scripts = new Scripts();
      Assert.IsEmpty(scripts.Items);

      var command = scripts.AddNewScriptCommand;
      Assert.IsTrue(command.CanExecute(null));
      command.Execute(null);

      var selectedScriptViewModel = scripts.SelectedItem;
      Assert.IsNotNull(selectedScriptViewModel);
      selectedScriptViewModel.HasBlankScriptData();
    }

    [Test]
    public void RemoveSelectedScriptsCommand()
    {
      var scripts = new Scripts();
      Assert.IsEmpty(scripts.Items);

      var command = scripts.RemoveSelectedScriptsCommand;
      Assert.IsTrue(command.CanExecute(null));
      command.Execute(null);

      Assert.IsEmpty(scripts.Items);

      var newScript = new Script();
      scripts.Items.Add(newScript);
      Assert.AreEqual(1, scripts.Items.Count);
      Assert.IsFalse(scripts.Items.First().IsChecked);
      command.Execute(null);
      Assert.AreEqual(1, scripts.Items.Count);
      Assert.IsFalse(scripts.Items[0].IsChecked);

      var newCheckedScript = new Script();
      scripts.Items.Add(newCheckedScript);
      newCheckedScript.IsChecked = true;
      Assert.AreEqual(2, scripts.Items.Count);
      Assert.IsFalse(scripts.Items[0].IsChecked);
      Assert.IsTrue(scripts.Items[1].IsChecked);
      command.Execute(null);
      Assert.AreEqual(1, scripts.Items.Count);
      Assert.IsFalse(scripts.Items[0].IsChecked);
    }
  }
}
