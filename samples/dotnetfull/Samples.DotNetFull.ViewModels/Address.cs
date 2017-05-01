using System.ComponentModel.DataAnnotations;

namespace Samples.DotNetFull.ViewModels
{
    public class Address
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Country { get; set; }

        [Required]
        [StringLength(100)]
        public string City { get; set; }

        [Required]
        [RegularExpression(@"(?<!\d)(?!0000)\d{4}(?!\d)", ErrorMessage = "Invalid Postal Code")]       
        public string PostalCode { get; set; }

        public string Street { get; set; }

        public string HouseNumber { get; set; }
    }
}
