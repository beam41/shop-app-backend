using System.Collections.Generic;

namespace ShopAppBackend.Models.DTOs
{
    public class ProductTypeDisplayDto
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public ICollection<ProductDisplayDto> ProductList { get; set; }
    }
}
