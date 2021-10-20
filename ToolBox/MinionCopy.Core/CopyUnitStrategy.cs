using System;
using System.Collections.Generic;

namespace MinionCopy
{
  public abstract class CopyUnitStrategy : CopyStrategy
  {
    public override ICopyStrategy ValidateReqiredProperties()
    {
      if (string.IsNullOrWhiteSpace(this.Source))
        throw new ArgumentException($"Options. '{nameof(this.Source)}' is null or empty.");

      if (string.IsNullOrWhiteSpace(this.Destination))
        throw new ArgumentException($"Options. '{nameof(this.Destination)}' is null or empty.");

      return this;
    }
    public override IEnumerable<ICopyStrategy> GetChildren()
    {
      return new List<ICopyStrategy>() { this };
    }
  }
}
