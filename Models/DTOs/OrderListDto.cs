using System;
using System.Collections.Generic;
using ShopAppBackend.Enums;

namespace ShopAppBackend.Models.DTOs
{
    public class OrderListDto
    {
        public string Id { get; set; }

        public DateTimeOffset UpdatedDate { get; set; }

        public ICollection<string> ProductsName { get; set; }

        public double TotalPrice { get; set; }

        public OrderStateEnum State { get; set; }

        public PurchaseMethodEnum PurchaseMethod { get; set; }
    }
}
