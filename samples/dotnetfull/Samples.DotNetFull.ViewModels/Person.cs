using System;
using Samples.DotNetFull.Common.Enums;

namespace Samples.DotNetFull.ViewModels
{
    public class Person
    {
        public long Id { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public DateTime DateOfBirth { get; set; }

        public Address Address { get; set; }

        public Gender Gender { get; set; }
    }
}
