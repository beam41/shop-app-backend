using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ShopAppBackend.Models
{
    public class OrderProduct
    {
        public int Id { get; set; }

        public Order Order { get; set; }

        public Product Product { get; set; }

        public int Amount { get; set; }
    }
}
