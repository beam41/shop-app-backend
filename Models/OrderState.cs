using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using ShopAppBackend.Enums;

namespace ShopAppBackend.Models
{
    public class OrderState
    {
        public int Id { get; set; }

        [Column(TypeName = "varchar(33)")]
        public OrderStateEnum State { get; set; }

        public DateTimeOffset CreatedAt { get; set; }

        public Order Order { get; set; }

        public BuildOrder BuildOrder { get; set; }
    }
}
