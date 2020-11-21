using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace ShopAppBackend.Models.DTOs
{
    public class ProductAddFormDto
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

        [Required]
        public ICollection<IFormFile> Images { get; set; }

        public static implicit operator Product(ProductAddFormDto p)
        {
            return new Product
            {
                Id = p.Id,
                Name = p.Name,
                Price = p.Price,
                Description = p.Description,
                IsVisible = p.IsVisible
            };
        }
    }
}
