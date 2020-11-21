using System.Collections.Generic;

namespace ShopAppBackend.Models.DTOs
{
    public class PromotionDisplayDto
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public ICollection<ProductDisplayDto> ProductList { get; set; }
    }
}