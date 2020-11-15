using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ShopAppBackend.Models
{
    public class BuildOrderImage
    {
        public int Id { get; set; }

        [Required]
        public BuildOrder BuildOrder { get; set; }

        [Required]
        public string ImageFileName { get; set; }
    }
}
