using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ShopAppBackend.Models
{
    public class ProductImage
    {
        public int Id { get; set; }

        [Required]
        public Product Product { get; set; }

        [Required]
        public string ImageFileName { get; set; }
    }

    public class ProductImageUrlDTO
    {
        public int Id { get; set; }

        public string ImageUrl { get; set; }
    }
}
