namespace ShopAppBackend.Models.DTOs
{
    public class ProductDisplayDto
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public double Price { get; set; }

        public double? NewPrice { get; set; }

        public string ImageUrl { get; set; }
    }
}
