using NUnit.Framework;
using System;
using System.IO;
using System.Linq;

namespace RunForrest.Desktop.Tests
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
