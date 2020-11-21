using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ShopAppBackend.Models
{
    public class ProductType
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        public bool Archived { get; set; }

        public ICollection<Product> Products { get; set; }
    }
}
