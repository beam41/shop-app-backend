using System.ComponentModel.DataAnnotations;

namespace ShopAppBackend.Models.DTOs
{
    public class BuildOrderIsAbleToBuiltDto
    {
        [Required]
        public bool IsAbleToBuilt { get; set; }

        public double DepositPrice { get; set; }

        public double FullPrice { get; set; }

        public string RejectedReason { get; set; }
    }
}