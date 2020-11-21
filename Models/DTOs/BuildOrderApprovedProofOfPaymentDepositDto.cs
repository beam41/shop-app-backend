using System;
using System.ComponentModel.DataAnnotations;

namespace ShopAppBackend.Models.DTOs
{
    public class BuildOrderApprovedProofOfPaymentDepositDto
    {
        [Required]
        public DateTimeOffset ExpectedCompleteDate { get; set; }
    }
}
