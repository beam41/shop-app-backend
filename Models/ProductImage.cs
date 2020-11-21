using System.ComponentModel.DataAnnotations;

namespace ShopAppBackend.Models
{
    public class ProductImage
    {
        public int Id { get; set; }

        [Required]
        public Product Product { get; set; }

        [Required]
        public string ImageFileName { get; set; }
    }
}
