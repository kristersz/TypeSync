using System;

namespace Samples.DotNetFull.ViewModels
{
    public class Student : Person
    {
        public DateTime? YearOfGraduation { get; set; }

        public StudentType Type { get; set; }
    }
}
