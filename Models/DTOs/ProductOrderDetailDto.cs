namespace ShopAppBackend.Models.DTOs
{
    public class ProductOrderDetailDto
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public double Price { get; set; }

        public double? NewPrice { get; set; }

        public int Amount { get; set; }
    }
}
