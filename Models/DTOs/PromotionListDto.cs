namespace ShopAppBackend.Models.DTOs
{
    public class PromotionListDto
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public bool IsBroadcasted { get; set; }

        public int ItemsCount { get; set; }
    }
}