using System.ComponentModel.DataAnnotations;

namespace ShopAppBackend.Models.DTOs
{
    public class BuildOrderIsAbleToBuiltDto
    {
        [Required]
        public double DepositPrice { get; set; }

        [Required]
        public double FullPrice { get; set; }
    }
}
