using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ShopAppBackend.Models
{
    public class User
    {
        public int Id { get; set; }

        [Required]
        [Column(TypeName = "varchar(64)")]
        [StringLength(64, MinimumLength = 6)]
        public string Username { get; set; }

        [Required]
        [Column(TypeName = "char(44)")]
        [StringLength(44, MinimumLength = 6)]
        public string Password { get; set; }

        [Required]
        [Column(TypeName = "char(10)")]
        [StringLength(10)]
        public string PhoneNumber { get; set; }

        public string FullName { get; set; }

        public string Address { get; set; }

        [Column(TypeName = "nvarchar(32)")]
        public string Province { get; set; }

        [Column(TypeName = "nvarchar(32)")]
        public string District { get; set; }

        [Column(TypeName = "nvarchar(32)")]
        public string SubDistrict { get; set; }

        [Column(TypeName = "char(5)")]
        [StringLength(5)]
        public string PostalCode { get; set; }

        public ICollection<Order> Orders { get; set; }
    }

    public class UserLoginDTO
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

        public static implicit operator UserLoginDTO(User u)
        {
            if (u != null)
            {
                return new UserLoginDTO()
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

    public class UserLoginFormDTO
    {
        [Required]
        [MinLength(6)]
        public string Username { get; set; }

        [Required]
        [MinLength(6)]
        public string Password { get; set; }
    }

    public class UserEditDTO
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

    public class UserListDTO
    {
        public int Id { get; set; }

        public string Username { get; set; }

        public string PhoneNumber { get; set; }

        public string FullName { get; set; }

        public int ActiveOrders { get; set; }
    }

    public class UserFormDTO
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
    }
}
