namespace ShopAppBackend.Models.DTOs
{
    public class ProductDetailPromotionDto
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public double Price { get; set; }

        public double? NewPrice { get; set; }

        public bool IsVisible { get; set; }

        public bool OnSale { get; set; }

        public bool? OnSaleCurrPromotion { get; set; }
    }
}
