using System;

namespace TypeSync.Models.CSharp
{
    public class DependantType : IComparable<DependantType>
    {
        public string Name { get; set; }

        public string ContainingAssembly { get; set; }

        public override string ToString()
        {
            return Name + " [" + ContainingAssembly + "]";
        }


        public int CompareTo(DependantType other)
        {
            return this.ToString().CompareTo(other.ToString());
        }
    }
}
