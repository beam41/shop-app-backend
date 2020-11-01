using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ShopAppBackend.Models
{
    public class PaymentMethod
    {
        public int Id { get; set; }

        public string Bank { get; set; }

        public string AccountNumber { get; set; }

        public string AccountName { get; set; }
    }
}
