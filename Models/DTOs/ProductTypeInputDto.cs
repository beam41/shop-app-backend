using System.ComponentModel.DataAnnotations;

namespace ShopAppBackend.Models.DTOs
{
    public class ProductTypeInputDto
    {
        [Required]
        public string Name { get; set; }
    }
}
