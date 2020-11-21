namespace ShopAppBackend.Models.DTOs
{
    public class UserListDto
    {
        public int Id { get; set; }

        public string Username { get; set; }

        public string PhoneNumber { get; set; }

        public string FullName { get; set; }

        public int ActiveOrders { get; set; }
    }
}