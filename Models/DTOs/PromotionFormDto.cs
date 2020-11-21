using System.Collections.Generic;

namespace ShopAppBackend.Models.DTOs
{
    public class PromotionFormDto
    {
        public string Name { get; set; }

        public string Description { get; set; }

        public bool IsBroadcasted { get; set; }

        public ICollection<PromotionItemsDto> PromotionItems { get; set; }
    }
}
