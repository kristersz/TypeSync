using System;

namespace Samples.DotNetFull.ViewModels
{
    public class Student : Person
    {
        public string StudentLicenceNumber { get; set; }

        public DateTime? YearOfGraduation { get; set; }

        public StudentType? Type { get; set; }
    }
}
