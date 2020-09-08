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

        public ICollection<PromotionItem> PromotionItems { get; set; }
    }

    public class PromotionFormDTO
    {
        public string Name { get; set; }

        public string Description { get; set; }

        public bool IsBroadcasted { get; set; }

        public ICollection<PromotionItemsDTO> PromotionItems { get; set; }

        public static implicit operator Promotion(PromotionFormDTO p)
        {
            return new Promotion
            {
                Name = p.Name,
                Description = p.Description,
                IsBroadcasted = p.IsBroadcasted
            };
        }
    }

    public class PromotionDisplayDTO
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public ICollection<ProductDisplayDTO> ProductList { get; set; }
    }

    public class PromotionInProductDetailDTO
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public static implicit operator PromotionInProductDetailDTO(Promotion p)
        {
            if (p != null)
            {
                return new PromotionInProductDetailDTO
                {
                    Id = p.Id,
                    Name = p.Name,
                    Description = p.Description,
                };
            }
            return null;
        }
    }
}
