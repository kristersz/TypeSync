using System;
using System.ComponentModel.DataAnnotations;
using Samples.DotNetFull.Common.Enums;

namespace Samples.DotNetFull.ViewModels
{
    public class Person
    {
        public long Id { get; set; }

        [Display(Name = "First name")]
        [Required(ErrorMessage= "{0} is required")]
        [StringLength(50, MinimumLength = 10, ErrorMessage = "{0} length should be between {2} and {1}.")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Last name is required")]
        public string LastName { get; set; }

        public DateTime DateOfBirth { get; set; }

        public Address Address { get; set; }

        public Gender Gender { get; set; }
    }
}
