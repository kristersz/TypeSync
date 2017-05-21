using System.Collections.Generic;

namespace TypeSync.Models.Angular
{
    public class AngularFormFieldModel
    {
        public AngularFormFieldModel()
        {
            Validators = new List<AngularFormValidator>();
        }

        public string Name { get; set; }

        public List<AngularFormValidator> Validators { get; set; }
    }
}
