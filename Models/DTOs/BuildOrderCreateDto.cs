using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace ShopAppBackend.Models.DTOs
{
    public class BuildOrderCreateDto
    {
        [Required]
        public string FullName { get; set; }

        [Required]
        public string OrderDescription { get; set; }

        public ICollection<IFormFile> DescriptionImages { get; set; }

        [Required]
        [StringLength(10)]
        public string PhoneNumber { get; set; }
    }
}
