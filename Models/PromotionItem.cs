﻿using System.ComponentModel.DataAnnotations;

namespace ShopAppBackend.Models
{
    public class PromotionItem
    {
        public int Id { get; set; }

        [Required]
        public Promotion Promotion { get; set; }

        [Required]
        public Product InPromotionProduct { get; set; }

        [Required]
        public double NewPrice { get; set; }
    }
}
