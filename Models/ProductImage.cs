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

    public class ImageUrlDTO
    {
        public int Id { get; set; }

        public string ImageUrl { get; set; }
    }
}
