using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace ShopAppBackend.Models.DTOs
{
    public class ProductEditFormDto
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public double Price { get; set; }

        [Required]
        public string Description { get; set; }

        [Required]
        public bool IsVisible { get; set; }

        [Required]
        public int TypeId { get; set; }

        public ICollection<IFormFile> Images { get; set; }

        public ICollection<int> MarkForDeleteId { get; set; }
    }
}
