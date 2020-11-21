using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ShopAppBackend.Models
{
    public class OrderProduct
    {
        public int Id { get; set; }

        public Order Order { get; set; }

        public Product Product { get; set; }

        [Required]
        public int Amount { get; set; }

        public double SavedPrice { get; set; }

        public double? SavedNewPrice { get; set; }
    }
}
