using System.ComponentModel.DataAnnotations;

namespace ShopAppBackend.Models.DTOs
{
    public class UserEditDto
    {
        [MinLength(6)]
        public string Password { get; set; }

        [MinLength(6)]
        public string NewPassword { get; set; }

        public string PhoneNumber { get; set; }

        public string FullName { get; set; }

        public string Address { get; set; }

        public string Province { get; set; }

        public string District { get; set; }

        public string SubDistrict { get; set; }

        [StringLength(5)]
        public string PostalCode { get; set; }
    }
}