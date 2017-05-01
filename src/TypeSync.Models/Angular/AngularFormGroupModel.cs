using System.Collections.Generic;

namespace TypeSync.Models.Angular
{
    public class AngularFormGroupModel
    {
        public AngularFormGroupModel()
        {
            Fields = new List<AngularFormFieldModel>();
        }

        public string Name { get; set; }

        public List<AngularFormFieldModel> Fields { get; set; }
    }
}
