using System.Collections.Generic;

namespace ShopAppBackend.Models.DTOs
{
    public class ProductTypeDetailDto
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public ICollection<ProductListInTypeDto> ProductList { get; set; }
    }
}
