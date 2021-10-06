using System;

namespace ToolBox.Desktop
{
  public class ToolsLoadException : Exception
  {
    public ToolsLoadException() : base()
    {
    }

    public ToolsLoadException(string message) : base(message)
    {
    }

    public ToolsLoadException(string message, Exception inner) : base(message, inner)
    {
    }
  }
}
