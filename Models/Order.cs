using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using ShopAppBackend.Enums;

namespace ShopAppBackend.Models
{
    public class Order // this only work for storefront order, for now
    {
        public int Id { get; set; }

        public User CreatedByUser { get; set; }

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

        [Column(TypeName = "varchar(MAX)")]
        public PurchaseMethodEnum PurchaseMethod { get; set; }

        public string ProofOfPaymentImgPath { get; set; }

        [Column(TypeName = "varchar(MAX)")]
        public DistributionMethodEnum DistributionMethod { get; set; }

        public string ParcelNumber { get; set; }

        public bool IsCancelledBySeller { get; set; }

        public ICollection<OrderState> OrderStates { get; set; }

        public ICollection<OrderPromotion> OrderPromotions { get; set; }

        public ICollection<OrderProduct> OrderProducts { get; set; }

    }
}
