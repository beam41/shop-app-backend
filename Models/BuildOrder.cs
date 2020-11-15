using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ShopAppBackend.Models
{
    public class BuildOrder
    {
        public int Id { get; set; }

        [Required]
        public User CreatedByUser { get; set; }

        [Required]
        public ICollection<OrderState> OrderStates { get; set; }

        public DistributionMethod DistributionMethod { get; set; }

        [Required]
        public string FullName { get; set; }

        [Required]
        public string OrderDescription { get; set; }

        [Required]
        public ICollection<BuildOrderImage> DescriptionImages { get; set; }

        [Required]
        [Column(TypeName = "char(10)")]
        [StringLength(10)]
        public string PhoneNumber { get; set; }

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

        public string ProofOfPaymentFullImage { get; set; }

        public string TrackingNumber { get; set; }

        public string ReceivedMessage { get; set; }

        public bool? CancelledByAdmin { get; set; }

        public string CancelledReason { get; set; }
    }
}
