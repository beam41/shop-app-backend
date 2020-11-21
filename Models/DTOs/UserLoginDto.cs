namespace ShopAppBackend.Models.DTOs
{
    public class UserLoginDto
    {
        public int Id { get; set; }

        public string Username { get; set; }

        public string PhoneNumber { get; set; }

        public string FullName { get; set; }

        public string Address { get; set; }

        public string Province { get; set; }

        public string District { get; set; }

        public string SubDistrict { get; set; }

        public string PostalCode { get; set; }

        public string Token { get; set; }

        public static implicit operator UserLoginDto(User u)
        {
            if (u != null)
            {
                return new UserLoginDto()
                {
                    Id = u.Id,
                    Username = u.Username,
                    PhoneNumber = u.PhoneNumber,
                    FullName = u.FullName,
                    Address = u.Address,
                    Province = u.Province,
                    District = u.District,
                    SubDistrict = u.SubDistrict,
                    PostalCode = u.PostalCode
                };
            }
            return null;
        }
    }
}