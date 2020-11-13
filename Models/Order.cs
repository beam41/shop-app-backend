using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json.Linq;
using ShopAppBackend.Enums;

namespace ShopAppBackend.Models
{
    public class Order
    {
        public int Id { get; set; }

        public User CreatedByUser { get; set; }

        [Column(TypeName = "varchar(11)")]
        public PurchaseMethodEnum PurchaseMethod { get; set; }

        public ICollection<OrderState> OrderStates { get; set; }

        public ICollection<OrderProduct> OrderProducts { get; set; }

        public DistributionMethod DistributionMethod { get; set; }

        public string TrackingNumber { get; set; }

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

        public string ProofOfPaymentFullImage { get; set; }

        public string ReceivedMessage { get; set; }

        public bool? CancelledByAdmin { get; set; }

        public string CancelledReason { get; set; }
    }

    public class OrderCreateDTO
    {
        public ICollection<OrderProductCreateDTO> Products { get; set; }

        [StringLength(10)]
        public string PhoneNumber { get; set; }

        public string FullName { get; set; }

        public string Address { get; set; }

        public string Province { get; set; }

        public string District { get; set; }

        public string SubDistrict { get; set; }

        [StringLength(5)]
        public string PostalCode { get; set; }

        public PurchaseMethodEnum PurchaseMethod { get; set; }

        public int DistributionMethodId { get; set; }
    }

    public class OrderViewDTO
    {
        public int Id { get; set; }

        public ICollection<ProductOrderDetailDTO> Products { get; set; }

        public PurchaseMethodEnum PurchaseMethod { get; set; }

        public ICollection<OrderStateDTO> OrderStates { get; set; }

        public DistributionMethod DistributionMethod { get; set; }

        public string TrackingNumber { get; set; }

        public string PhoneNumber { get; set; }

        public string FullName { get; set; }

        public string Address { get; set; }

        public string Province { get; set; }

        public string District { get; set; }

        public string SubDistrict { get; set; }

        public string PostalCode { get; set; }

        public string ProofOfPaymentFullImage { get; set; }

        public string ReceivedMessage { get; set; }

        public bool? CancelledByAdmin { get; set; }

        public string CancelledReason { get; set; }

        public User CreatedByUser { get; set; }
    }

    public class OrderListAdminDTO
    {
        public int Id { get; set; }

        public User CreatedByUser { get; set; }

        public DateTimeOffset CreatedDate { get; set; }

        public DateTimeOffset UpdatedDate { get; set; }

        public PurchaseMethodEnum PurchaseMethod { get; set; }

        public int ProductsCount { get; set; }

        public int AmountCount { get; set; }

        public double TotalPrice { get; set; }
    }

    public class OrderListDTO
    {
        public int Id { get; set; }

        public DateTimeOffset UpdatedDate { get; set; }

        public ICollection<string> ProductsName { get; set; }

        public int AmountCount { get; set; }

        public double TotalPrice { get; set; }

        public OrderStateEnum State { get; set; }

        public PurchaseMethodEnum PurchaseMethod { get; set; }
    }

    public class OrderAddProofOfPaymentFullDTO
    {
        public IFormFile Image { get; set; }
    }

    public class OrderSentDTO
    {
        public string TrackingNumber { get; set; }
    }

    public class OrderReceivedDTO
    {
        public string Message { get; set; }
    }

    public class OrderCancelledDTO
    {
        public string Reason { get; set; }
    }
}
