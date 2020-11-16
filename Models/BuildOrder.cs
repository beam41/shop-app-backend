using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using ShopAppBackend.Enums;

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

        [Column(TypeName = "char(10)")]
        [StringLength(10)]
        public string AddressPhoneNumber { get; set; }

        public string AddressFullName { get; set; }

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

        public string ProofOfPaymentDepositImage { get; set; }

        public string TrackingNumber { get; set; }

        public string ReceivedMessage { get; set; }

        public bool? CancelledByAdmin { get; set; }

        public string CancelledReason { get; set; }

        public double? DepositPrice { get; set; }

        public double? FullPrice { get; set; }

        public DateTimeOffset? ExpectedCompleteDate { get; set; }
    }

    public class BuildOrderCreateDTO
    {
        [Required]
        public string FullName { get; set; }

        [Required]
        public string OrderDescription { get; set; }

        public ICollection<IFormFile> DescriptionImages { get; set; }

        [Required]
        [StringLength(10)]
        public string PhoneNumber { get; set; }
    }

    public class BuildOrderIsAbleToBuiltDTO
    {
        [Required]
        public bool IsAbleToBuilt { get; set; }

        public double DepositPrice { get; set; }

        public double FullPrice { get; set; }

        public string RejectedReason { get; set; }
    }

    public class BuildOrderAddProofOfPaymentFullDTO
    {
        public IFormFile Image { get; set; }

        [Required]
        public string FullName { get; set; }

        [Required]
        public string Address { get; set; }

        [Required]
        public string Province { get; set; }

        [Required]
        public string District { get; set; }

        [Required]
        public string SubDistrict { get; set; }

        [Required]
        [StringLength(5)]
        public string PostalCode { get; set; }

        [Required]
        [StringLength(10)]
        public string PhoneNumber { get; set; }

        [Required]
        public int DistributionMethodId { get; set; }
    }

    public class BuildOrderApprovedProofOfPaymentDepositDTO
    {
        [Required]
        public DateTimeOffset ExpectedCompleteDate { get; set; }
    }

    public class BuildOrderListDTO
    {
        public int Id { get; set; }

        public DateTimeOffset UpdatedDate { get; set; }

        public string OrderDescription { get; set; }

        public OrderStateEnum State { get; set; }
    }
}
