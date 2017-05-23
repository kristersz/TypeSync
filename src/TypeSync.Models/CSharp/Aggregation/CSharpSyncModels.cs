using System.Collections.Generic;

namespace TypeSync.Models.CSharp
{
    public class CSharpSyncModels
    {
        public CSharpSyncModels()
        {
            Controllers = new List<CSharpControllerModel>();
        }

        public CSharpDataModels DataModels { get; set; }

        public List<CSharpControllerModel> Controllers { get; set; }
    }
}
