using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ShopAppBackend.Models
{
    public class DistributionMethod
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public double Price { get; set; }

        public bool Archived { get; set; }

        public ICollection<Order> Orders { get; set; }

        public ICollection<BuildOrder> BuildOrders { get; set; }
    }
}
