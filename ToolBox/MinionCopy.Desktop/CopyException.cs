﻿using System;

namespace MinionCopy.Desktop
{
  public class CopyException : ICopyDetailedResult
  {
    private const string DefaultMessage = @"<Empty>";

    public CopyResult CopyResult { get; private set; }
    public string Message
    {
      get
      {
        return $"Error: {this.Exception?.Message ?? DefaultMessage}";
      }
    }
    public ICopyStrategyViewModel Owner { get; private set; }
    public Exception Exception { get; private set; }

    public CopyException(ICopyStrategyViewModel owner, Exception exception)
    {
      this.CopyResult = CopyResult.Failed;
      this.Owner = owner;
      this.Exception = exception;
    }
  }
}
