using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace ShopAppBackend.Models
{
    public class User
    {
        public int Id { get; set; }

        [Required]
        [Column(TypeName = "varchar(64)")]
        [StringLength(64)]
        public string Username { get; set; }

        [Required]
        [Column(TypeName = "char(44)")]
        [StringLength(44)]
        public string Password { get; set; }

        [Required]
        [Column(TypeName = "char(10)")]
        [StringLength(10)]
        public string PhoneNumber { get; set; }

        public string FullName { get; set; }

        public string Address { get; set; }

        [Column(TypeName = "varchar(32)")]
        public string Province { get; set; }

        [Column(TypeName = "varchar(32)")]
        public string District { get; set; }

        [Column(TypeName = "varchar(32)")]
        public string SubDistrict { get; set; }

        [Column(TypeName = "char(5)")]
        [StringLength(5)]
        public string PostalCode { get; set; }

        [NotMapped]
        public string Token { get; set; }
    }
}
