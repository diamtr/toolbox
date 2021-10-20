using System.Collections.Generic;

namespace MinionCopy
{
  public interface ICopyStrategy
  {
    string Source { get; set; }
    string Destination { get; set; }
    bool Replace { get; set; }
    string Rename { get; set; }
    
    ICopyStrategy ValidateReqiredProperties();
    ICopyStrategy MakeSourcePathRooted();
    ICopyStrategy MakeDestinationPathRooted();
    ICopyStrategy WithSourceExistsValidation();
    ICopyStrategy PrepareDestination();
    ICopyStrategy WithRename();
    ICopyStrategy WithReplace();
    IEnumerable<ICopyStrategy> GetChildren();
    void Copy();
  }
}
