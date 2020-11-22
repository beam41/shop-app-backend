using System;
using System.ComponentModel.DataAnnotations.Schema;
using ShopAppBackend.Enums;

namespace ShopAppBackend.Models
{
    public class OrderState
    {
        public int Id { get; set; }

        [Column(TypeName = "varchar(33)")]
        public OrderStateEnum State { get; set; }

        public DateTimeOffset CreatedDate { get; set; }

        public Order Order { get; set; }

        public BuildOrder BuildOrder { get; set; }
    }
}
