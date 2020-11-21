using System.ComponentModel.DataAnnotations;

namespace ShopAppBackend.Models.DTOs
{
    public class UserLoginFormDto
    {
        [Required]
        [MinLength(6)]
        public string Username { get; set; }

        [Required]
        [MinLength(6)]
        public string Password { get; set; }
    }
}
