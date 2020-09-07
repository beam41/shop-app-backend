﻿using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ShopAppBackend.Models
{
    public class Product
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public int Price { get; set; }

        [Required]
        public string Description { get; set; }

        [Required]
        public bool IsVisible { get; set; }

        [Required]
        public ProductType Type { get; set; }

        public ICollection<ProductImage> ProductImages { get; set; }

        public ICollection<PromotionItem> PromotionItems { get; set; }
    }

    public class ProductFormDTO
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public int Price { get; set; }

        public string Description { get; set; }

        public bool IsVisible { get; set; }

        public int TypeId { get; set; }

        public ICollection<IFormFile> Images { get; set; }

        public static implicit operator Product(ProductFormDTO p)
        {
            return new Product
            {
                Id = p.Id,
                Name = p.Name,
                Price = p.Price,
                Description = p.Description,
                IsVisible = p.IsVisible
            };
        }
    }

    public class ProductDisplayDTO
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public int Price { get; set; }

        public int? NewPrice { get; set; }

        public string ImageUrl { get; set; }
    }

    public class ProductDetailDTO
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public int Price { get; set; }

        public int? NewPrice { get; set; }

        public ICollection<ProductImageUrlDTO> ImageUrls { get; set; }

        public string Description { get; set; }

        public PromotionInProductDetailDTO Promotion { get; set; }
    }
}
