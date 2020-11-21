using System.Collections.Generic;

namespace ShopAppBackend.Models.DTOs
{
    public class PromotionFormDto
    {
        public string Name { get; set; }

        public string Description { get; set; }

        public bool IsBroadcasted { get; set; }

        public ICollection<PromotionItemsDto> PromotionItems { get; set; }

        public static implicit operator Promotion(PromotionFormDto p)
        {
            return new Promotion
            {
                Name = p.Name,
                Description = p.Description,
                IsBroadcasted = p.IsBroadcasted
            };
        }
    }
}