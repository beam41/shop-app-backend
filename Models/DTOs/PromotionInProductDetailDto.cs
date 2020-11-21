namespace ShopAppBackend.Models.DTOs
{
    public class PromotionInProductDetailDto
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public static implicit operator PromotionInProductDetailDto(Promotion p)
        {
            if (p != null)
            {
                return new PromotionInProductDetailDto
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