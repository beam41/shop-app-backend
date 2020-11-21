using System;
using ShopAppBackend.Enums;

namespace ShopAppBackend.Models.DTOs
{
    public class OrderListAdminDto
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
}