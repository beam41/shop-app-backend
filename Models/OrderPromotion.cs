using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ShopAppBackend.Models
{
    public class OrderPromotion
    {
        public int Id { get; set; }

        public Order Order { get; set; }

        public Promotion Promotion { get; set; }
    }
}
