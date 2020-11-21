using System.Collections.Generic;

namespace ShopAppBackend.Models.DTOs
{
    public class PromotionDetailDto
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public bool IsBroadcasted { get; set; }

        public ICollection<ProductDetailPromotionDto> PromotionItems { get; set; }
    }
}
