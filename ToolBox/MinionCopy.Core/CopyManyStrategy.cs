using System;
using System.Collections.Generic;
using System.Linq;

namespace MinionCopy
{
  public abstract class CopyManyStrategy : CopyStrategy
  {
    public List<ICopyStrategy> Items { get; set; }

    public CopyManyStrategy()
    {
      this.Items = new List<ICopyStrategy>();
    }

    public override ICopyStrategy PrepareDestination()
    {
      throw new NotImplementedException();
    }
    public override ICopyStrategy WithRename()
    {
      throw new NotImplementedException();
    }
    public override ICopyStrategy WithReplace()
    {
      throw new NotImplementedException();
    }
    public override IEnumerable<ICopyStrategy> GetChildren()
    {
      return this.Items.SelectMany(x => x.GetChildren());
    }

  }
}
