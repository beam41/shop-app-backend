using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using ShopAppBackend.Enums;

namespace ShopAppBackend.Models
{
    public class Order
    {
        public int Id { get; set; }

        public User CreatedByUser { get; set; }

        [Column(TypeName = "varchar(11)")]
        public PurchaseMethodEnum PurchaseMethod { get; set; }

        public ICollection<OrderState> OrderStates { get; set; }

        public ICollection<OrderPromotion> OrderPromotions { get; set; }

        public ICollection<OrderProduct> OrderProducts { get; set; }
    }

    public class CreateOrderDTO
    {
        public ICollection<OrderProductCreateDTO> Products { get; set; }

        public string AddressJson { get; set; }

        public PurchaseMethodEnum PurchaseMethod { get; set; }
    }
}
