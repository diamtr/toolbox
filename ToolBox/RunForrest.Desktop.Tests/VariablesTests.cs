using NUnit.Framework;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Web.Script.Serialization;

namespace RunForrestPlugin.Tests
{
  [TestFixture]
  public class VariablesTests
  {
    [Test]
    public void CreateVariablesClass()
    {
      var variables = new Variables();

      Assert.IsEmpty(variables.Items);
    }

    [Test]
    public void AddOneItemToVariables()
    {
      var variables = new Variables();
      var variable = new VariableData() { Name = "Name1", Value = "Value1" };
      variables.AddNewVariable(variable);

      Assert.IsNotEmpty(variables.Items);
      Assert.AreEqual(1, variables.Items.Count);
      Assert.AreEqual("Name1", variables.Items.First().Name);
      Assert.AreEqual("Value1", variables.Items.First().Value);
    }

    [Test]
    public void AddTwoDifferentNameItemsToVariables()
    {
      var variables = new Variables();
      var variable1 = new VariableData() { Name = "Name1", Value = "Value1" };
      var variable2 = new VariableData() { Name = "Name2", Value = "Value2" };
      variables.AddNewVariable(variable1);
      variables.AddNewVariable(variable2);

      Assert.IsNotEmpty(variables.Items);
      Assert.AreEqual(2, variables.Items.Count);
    }

    [Test]
    public void AddTwoSameNameItemsToVariables()
    {
      var variables = new Variables();
      var variable1 = new VariableData() { Name = "Name1", Value = "Value1" };
      var variable2 = new VariableData() { Name = "Name1", Value = "Value2" };
      variables.AddNewVariable(variable1);
      variables.AddNewVariable(variable2);

      Assert.IsNotEmpty(variables.Items);
      Assert.AreEqual(1, variables.Items.Count);
      Assert.AreEqual("Name1", variables.Items.First().Name);
      Assert.AreEqual("Value1", variables.Items.First().Value);
    }

    [Test]
    public void DeleteItemFromEmptyVariables()
    {
      var variables = new Variables();
      var variable = new VariableData() { Name = "Name1", Value = "Value1" };
      variables.Delete(variable);

      Assert.IsEmpty(variables.Items);
    }

    [Test]
    public void DeleteNotContainedItemFromVariables()
    {
      var variables = new Variables();
      var variable1 = new VariableData() { Name = "Name1", Value = "Value1" };
      var variable2 = new VariableData() { Name = "Name2", Value = "Value2" };
      variables.AddNewVariable(variable1);
      variables.Delete(variable2);

      Assert.IsNotEmpty(variables.Items);
      Assert.AreEqual(1, variables.Items.Count);
      Assert.AreEqual("Name1", variables.Items.First().Name);
      Assert.AreEqual("Value1", variables.Items.First().Value);
    }

    [Test]
    public void DeleteContainedItemFromVariables()
    {
      var variables = new Variables();
      var variable1 = new VariableData() { Name = "Name1", Value = "Value1" };
      var variable2 = new VariableData() { Name = "Name2", Value = "Value2" };
      variables.AddNewVariable(variable1);
      variables.AddNewVariable(variable2);
      variables.Delete(variable1);

      Assert.IsNotEmpty(variables.Items);
      Assert.AreEqual(1, variables.Items.Count);
      Assert.AreEqual("Name2", variables.Items.First().Name);
      Assert.AreEqual("Value2", variables.Items.First().Value);
    }

    [Test]
    public void GetPathToSaveVariablesIfNotPassed()
    {
      var baseDir = System.AppDomain.CurrentDomain.BaseDirectory;
      var defaultPath = Path.Combine(baseDir, "rfp_variables.json");
      var variables = new Variables();
      var getVariablesFilePathMethod =
        variables.GetType()
        .GetMethods(System.Reflection.BindingFlags.Instance |
                    System.Reflection.BindingFlags.NonPublic)
        .FirstOrDefault(x => x.Name == "GetVariablesFilePath");
      Assert.IsNotNull(getVariablesFilePathMethod);
      var path = getVariablesFilePathMethod.Invoke(variables, new object[] { null });
      Assert.AreEqual(defaultPath, path);
    }

    [Test]
    public void GetPathToSaveVariablesIfPassedRelative()
    {
      var baseDir = System.AppDomain.CurrentDomain.BaseDirectory;
      var expectedPath = Path.Combine(baseDir, "Scripts\\rfp_variables.json");
      var passedPath = @"Scripts\rfp_variables.json";
      var variables = new Variables();
      var getVariablesFilePathMethod =
        variables.GetType()
        .GetMethods(System.Reflection.BindingFlags.Instance |
                    System.Reflection.BindingFlags.NonPublic)
        .FirstOrDefault(x => x.Name == "GetVariablesFilePath");
      Assert.IsNotNull(getVariablesFilePathMethod);
      var path = getVariablesFilePathMethod.Invoke(variables, new object[] { passedPath });
      Assert.AreEqual(expectedPath, path);
    }

    [Test]
    public void GetPathToSaveVariablesIfPassedRooted()
    {
      var expectedPath = @"D:\\rfp_variables.json";
      var passedPath = @"D:\\rfp_variables.json";
      var variables = new Variables();
      var getVariablesFilePathMethod =
        variables.GetType()
        .GetMethods(System.Reflection.BindingFlags.Instance |
                    System.Reflection.BindingFlags.NonPublic)
        .FirstOrDefault(x => x.Name == "GetVariablesFilePath");
      Assert.IsNotNull(getVariablesFilePathMethod);
      var path = getVariablesFilePathMethod.Invoke(variables, new object[] { passedPath });
      Assert.AreEqual(expectedPath, path);
    }

    [Test]
    public void VariablesSavedToDefaultFile()
    {
      var variables = new Variables();
      var variable1 = new VariableData() { Name = "Name1", Value = "Value1" };
      var variable2 = new VariableData() { Name = "Name2", Value = "Value2" };
      variables.AddNewVariable(variable1);
      variables.AddNewVariable(variable2);
      variables.SaveToFile();
      var defaultPath = Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "rfp_variables.json");
      Assert.IsTrue(File.Exists(defaultPath));
      var text = File.ReadAllText(defaultPath);
      Assert.IsNotEmpty(text);
      var expectedText = @"[{""Name"":""Name1"",""Value"":""Value1""},{""Name"":""Name2"",""Value"":""Value2""}]";
      Assert.AreEqual(expectedText, text);
    }

    [Test]
    public void CheckVariablesLastSaveDateTime()
    {
      var variables = new Variables();
      var variable1 = new VariableData() { Name = "Name1", Value = "Value1" };
      var variable2 = new VariableData() { Name = "Name2", Value = "Value2" };
      variables.AddNewVariable(variable1);
      variables.AddNewVariable(variable2);
      var dateTimeNow = DateTime.Now;
      variables.SaveToFile();
      Assert.Less(variables.LastSaveDateTime - dateTimeNow, TimeSpan.FromSeconds(1));
    }

    [Test]
    public void VariablesLoadedFromDefaultFile()
    {
      var defaultPath = Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "rfp_variables.json");
      var givenText = @"[{""Name"":""Name1"",""Value"":""Value1""},{""Name"":""Name2"",""Value"":""Value2""}]";
      File.WriteAllText(defaultPath, givenText);
      var variables = new Variables();
      Assert.IsEmpty(variables.Items);
      variables.LoadFromFile();
      Assert.AreEqual(2, variables.Items.Count);
      Assert.AreEqual(1, variables.Items.Where(x => x.Name == "Name1" && x.Value == "Value1").Count());
      Assert.AreEqual(1, variables.Items.Where(x => x.Name == "Name2" && x.Value == "Value2").Count());
    }

    [Test]
    public void VariablesLoadedFromNotExistsFile()
    {
      var notExistingPath = Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "rfp_variables_not_exists.json");
      var variables = new Variables();
      Assert.IsEmpty(variables.Items);
      variables.LoadFromFile(notExistingPath);
      Assert.AreEqual(0, variables.Items.Count);
    }

    [Test]
    public void AddNewVariableCommand()
    {
      var variables = new Variables();
      var command = variables.AddVariableCommand;
      Assert.IsEmpty(variables.Items);
      Assert.IsNotNull(variables.NewVariable);
      Assert.IsTrue(command.CanExecute(null));

      Assert.IsTrue(command.CanExecute(null));
      command.Execute(null);
      Assert.IsEmpty(variables.Items);
      Assert.IsNull(variables.NewVariable.Name);
      Assert.IsNull(variables.NewVariable.Value);

      variables.NewVariable.Name = "Name1";
      variables.NewVariable.Value = "Value1";
      Assert.IsTrue(command.CanExecute(null));
      command.Execute(null);
      Assert.IsNotNull(variables.NewVariable);
      Assert.IsNull(variables.NewVariable.Name);
      Assert.IsNull(variables.NewVariable.Value);
      Assert.IsNotEmpty(variables.Items);
      Assert.AreEqual(1, variables.Items.Count);
      Assert.AreEqual("Name1", variables.Items[0].Name);
      Assert.AreEqual("Value1", variables.Items[0].Value);

      variables.NewVariable.Name = "Name1";
      variables.NewVariable.Value = "Value2";
      Assert.IsTrue(command.CanExecute(null));
      command.Execute(null);
      Assert.IsNotNull(variables.NewVariable);
      Assert.IsNull(variables.NewVariable.Name);
      Assert.IsNull(variables.NewVariable.Value);
      Assert.IsNotEmpty(variables.Items);
      Assert.AreEqual(1, variables.Items.Count);
      Assert.AreEqual("Name1", variables.Items[0].Name);
      Assert.AreEqual("Value1", variables.Items[0].Value);
    }
  }
}
