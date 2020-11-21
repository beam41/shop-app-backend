﻿using System;
using ShopAppBackend.Enums;

namespace ShopAppBackend.Models.DTOs
{
    public class BuildOrderListDto
    {
        public int Id { get; set; }

        public DateTimeOffset UpdatedDate { get; set; }

        public string OrderDescription { get; set; }

        public OrderStateEnum State { get; set; }
    }
}
