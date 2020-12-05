using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ShopAppBackend.Enums;

namespace ShopAppBackend.Models
{
    public class Order
    {
        public string Id { get; set; }

        [Required]
        public User CreatedByUser { get; set; }

        [Required]
        [Column(TypeName = "varchar(11)")]
        public PurchaseMethodEnum PurchaseMethod { get; set; }

        [Required]
        public ICollection<OrderState> OrderStates { get; set; }

        [Required]
        public ICollection<OrderProduct> OrderProducts { get; set; }

        [Required]
        public DistributionMethod DistributionMethod { get; set; }

        public string TrackingNumber { get; set; }

        [Required]
        [Column(TypeName = "char(10)")]
        [StringLength(10)]
        public string PhoneNumber { get; set; }

        [Required]
        public string FullName { get; set; }

        [Required]
        public string Address { get; set; }

        [Required]
        [Column(TypeName = "nvarchar(32)")]
        public string Province { get; set; }

        [Required]
        [Column(TypeName = "nvarchar(32)")]
        public string District { get; set; }

        [Required]
        [Column(TypeName = "nvarchar(32)")]
        public string SubDistrict { get; set; }

        [Required]
        [Column(TypeName = "char(5)")]
        [StringLength(5)]
        public string PostalCode { get; set; }

        public string ProofOfPaymentFullImage { get; set; }

        public string ReceivedMessage { get; set; }

        public bool? CancelledByAdmin { get; set; }

        public string CancelledReason { get; set; }
    }
}
