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

        public ICollection<OrderPromotion> OrderPromotions { get; set; }

        public ICollection<OrderProduct> OrderProducts { get; set; }

        public DistributionMethod DistributionMethod { get; set; }
    }

    public class OrderCreateDTO
    {
        public ICollection<OrderProductCreateDTO> Products { get; set; }

        public JObject AddressJson { get; set; }

        public PurchaseMethodEnum PurchaseMethod { get; set; }

        public int DistributionMethodId { get; set; }
    }

    public class OrderViewDTO
    {
        public int Id { get; set; }

        public ICollection<ProductDetailDTO> Products { get; set; }

        public PurchaseMethodEnum PurchaseMethod { get; set; }

        public ICollection<OrderStateDTO> OrderStates { get; set; }

        public DistributionMethod DistributionMethod { get; set; }
    }

    public class OrderListAdminDTO
    {
        public int Id { get; set; }

        public string CreatedByUserFullName { get; set; }

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
    }

    public class OrderAddProofOfPaymentFullDTO
    {
        public IFormFile Image { get; set; }
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
