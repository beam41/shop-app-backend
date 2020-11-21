namespace ShopAppBackend.Models
{
    public class PaymentMethod
    {
        public int Id { get; set; }

        public string Bank { get; set; }

        public string AccountNumber { get; set; }

        public string AccountName { get; set; }
    }
}
