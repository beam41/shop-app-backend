﻿using System;

namespace ShopAppBackend.Models.DTOs
{
    public class BuildOrderListAdminDto
    {
        public string Id { get; set; }

        public User CreatedByUser { get; set; }

        public string FullName { get; set; }

        public string OrderDescription { get; set; }

        public DateTimeOffset? ExpectedCompleteDate { get; set; }

        public DateTimeOffset CreatedDate { get; set; }

        public DateTimeOffset UpdatedDate { get; set; }
    }
}
