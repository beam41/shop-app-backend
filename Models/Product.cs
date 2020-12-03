using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ShopAppBackend.Models
{
    public class Product
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

        public bool Archived { get; set; }

        [Required]
        public ProductType Type { get; set; }

        public ICollection<ProductImage> ProductImages { get; set; }

        public ICollection<PromotionItem> PromotionItems { get; set; }

        public ICollection<OrderProduct> OrderProducts { get; set; }
    }
}
