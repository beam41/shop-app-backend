using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using ShopAppBackend.Enums;

namespace ShopAppBackend.Models
{
    public class OrderState
    {
        public int Id { get; set; }

        [Column(TypeName = "varchar(MAX)")]
        public OrderStateEnum State { get; set; }

        public Order Order { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public DateTime CreatedAt { get; set; }

        public string StateDataJson { get; set; }
    }
}
