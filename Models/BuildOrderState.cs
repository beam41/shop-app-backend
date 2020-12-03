using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using ShopAppBackend.Enums;

namespace ShopAppBackend.Models
{
    public class BuildOrderState
    {
        public int Id { get; set; }

        [Column(TypeName = "varchar(33)")]
        public OrderStateEnum State { get; set; }

        public DateTimeOffset CreatedDate { get; set; }

        public BuildOrder BuildOrder { get; set; }
    }
}
