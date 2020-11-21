using System;
using ShopAppBackend.Enums;

namespace ShopAppBackend.Models.DTOs
{
    public class OrderStateDto
    {
        public int Id { get; set; }

        public OrderStateEnum State { get; set; }

        public DateTimeOffset CreatedAt { get; set; }
    }
}
