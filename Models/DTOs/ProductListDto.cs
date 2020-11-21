namespace ShopAppBackend.Models.DTOs
{
    public class ProductListDto
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public double Price { get; set; }

        public string Type { get; set; }

        public bool IsVisible { get; set; }

        public bool InPromotion { get; set; }

        public double? NewPrice { get; set; }
    }
}