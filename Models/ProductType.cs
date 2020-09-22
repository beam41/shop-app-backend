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

    public class ProductTypeDisplayDTO
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public ICollection<ProductDisplayDTO> ProductList { get; set; }
    }

    public class ProductTypeInputDTO
    {
        [Required]
        public string Name { get; set; }
    }

    public class ProductTypeDetailDTO
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public ICollection<ProductListInTypeDTO> ProductList { get; set; }
    }

    public class ProductTypeListDTO
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public int ProductCount { get; set; }
    }
}
