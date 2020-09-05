using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ShopAppBackend.Models
{
    public class Product
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public int Price { get; set; }

        public string Description { get; set; }

        public bool IsVisible { get; set; }

        public ICollection<ProductImage> ProductImages { get; set; }

        public ProductType Type { get; set; }
    }

    public class ProductFormDTO
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public int Price { get; set; }

        public string Description { get; set; }

        public bool IsVisible { get; set; }

        public int TypeId { get; set; }

        public static implicit operator Product(ProductFormDTO p)
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
