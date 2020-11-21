using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ShopAppBackend.Models
{
    public class Promotion
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public string Description { get; set; }

        [Required]
        public bool IsBroadcasted { get; set; }

        public bool Archived { get; set; }

        public ICollection<PromotionItem> PromotionItems { get; set; }
    }
}
