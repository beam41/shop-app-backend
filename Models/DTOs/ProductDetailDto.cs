using System.Collections.Generic;

namespace ShopAppBackend.Models.DTOs
{
    public class ProductDetailDto
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public double Price { get; set; }

        public double? NewPrice { get; set; }

        public ICollection<ImageUrlDto> ImageUrls { get; set; }

        public string Description { get; set; }

        public PromotionInProductDetailDto Promotion { get; set; }
    }
}
