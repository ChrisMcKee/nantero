using System;
using System.ComponentModel.DataAnnotations;

namespace Pactas.SDK.DTO
{
    public class CustomerDTO
    {
        public string ExternalCustomerId { get; set; }

        public string CompanyName { get; set; }
        [Required]
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }

        public string Language { get; set; }
        public string VatId { get; set; }
        public string EmailAddress { get; set; }
        public BearerMedium DefaultBearerMedium { get; set; }
        public string Notes { get; set; }
        public string Tag { get; set; }
        public AddressDTO Address { get; set; }
        public string TimeZoneKey { get; set; } //TODO: Set this information or get it somewhere else when creating the contract object
        public string Locale { get; set; }
        /// <summary>
        /// The 2-letter ISO country code in capital letters, e.g. DE
        /// </summary>
        [StringLength(2, MinimumLength = 2)]
        [CustomValidation(typeof(AddressValidation), "ValidateCountry")]
        public string ServiceCountry { get; set; }
        public CustomerType CustomerType { get; set; }

        public string CustomerName
        {
            get
            {
                return string.IsNullOrWhiteSpace(CompanyName) ? FirstName + " " + LastName : CompanyName;
            }
        }
    }

    public class CustomerReadDTO : CustomerDTO
    {
        public string Id { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
