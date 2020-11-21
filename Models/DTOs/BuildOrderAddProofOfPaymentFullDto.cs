using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace ShopAppBackend.Models.DTOs
{
    public class BuildOrderAddProofOfPaymentFullDto
    {
        public IFormFile Image { get; set; }

        [Required]
        public string FullName { get; set; }

        [Required]
        public string Address { get; set; }

        [Required]
        public string Province { get; set; }

        [Required]
        public string District { get; set; }

        [Required]
        public string SubDistrict { get; set; }

        [Required]
        [StringLength(5)]
        public string PostalCode { get; set; }

        [Required]
        [StringLength(10)]
        public string PhoneNumber { get; set; }

        [Required]
        public int DistributionMethodId { get; set; }
    }
}