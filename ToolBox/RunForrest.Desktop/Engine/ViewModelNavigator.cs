using System;
using System.Collections.Generic;
using ToolBox.Desktop.Base;

namespace RunForrest.Desktop
{
  public class ViewModelNavigator
  {
    public event Action ViewModelChanged;

    private Stack<ViewModelBase> stack;

    public void SetCurrent(ViewModelBase viewModel)
    {
      if (this.stack.Count > 0 &&
          this.stack.Peek().GetType() == viewModel.GetType())
        this.stack.Pop();

      this.stack.Push(viewModel);
      this.ViewModelChanged?.Invoke();
    }

    public ViewModelBase GetCurrent()
    {
      if (this.stack.Count <= 0)
        return null;

      return this.stack.Peek();
    }

    public void Forget(ViewModelBase viewModel)
    {
      if (this.stack.Count > 0 &&
          this.stack.Peek() != viewModel)
        return;

      this.stack.Pop();
      this.ViewModelChanged?.Invoke();
    }

    public ViewModelNavigator()
    {
      this.stack = new Stack<ViewModelBase>();
    }
  }
}
