using System;

namespace Samples.DotNetFull.ViewModels
{
    public class Student
    {
        public long Id { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public DateTime DateOfBirth { get; set; }

        public DateTime? YearOfGraduation { get; set; }

        public Address Address { get; set; }

        public StudentType Type { get; set; }
    }
}
