namespace ShopAppBackend.Models.DTOs
{
    public class ProductDisplayDto
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public double Price { get; set; }

        public double? NewPrice { get; set; }

        public string ImageUrl { get; set; }
    }
}