﻿using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ShopAppBackend.Models
{
    public class Product
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public double Price { get; set; }

        [Required]
        public string Description { get; set; }

        [Required]
        public bool IsVisible { get; set; }

        public bool Archived { get; set; }

        [Required]
        public ProductType Type { get; set; }

        public ICollection<ProductImage> ProductImages { get; set; }

        public ICollection<PromotionItem> PromotionItems { get; set; }
    }

    public class ProductAddFormDTO
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public double Price { get; set; }

        [Required]
        public string Description { get; set; }

        [Required]
        public bool IsVisible { get; set; }

        [Required]
        public int TypeId { get; set; }

        [Required]
        public ICollection<IFormFile> Images { get; set; }

        public static implicit operator Product(ProductAddFormDTO p)
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

    public class ProductEditFormDTO
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public double Price { get; set; }

        [Required]
        public string Description { get; set; }

        [Required]
        public bool IsVisible { get; set; }

        [Required]
        public int TypeId { get; set; }

        public ICollection<IFormFile> Images { get; set; }

        public ICollection<int> MarkForDeleteId { get; set; }

        public static implicit operator Product(ProductEditFormDTO p)
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

        public double Price { get; set; }

        public double? NewPrice { get; set; }

        public string ImageUrl { get; set; }
    }

    public class ProductDetailDTO
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public double Price { get; set; }

        public double? NewPrice { get; set; }

        public ICollection<ImageUrlDTO> ImageUrls { get; set; }

        public string Description { get; set; }

        public PromotionInProductDetailDTO Promotion { get; set; }
    }

    public class ProductListDTO
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public double Price { get; set; }

        public string Type { get; set; }

        public bool IsVisible { get; set; }

        public bool InPromotion { get; set; }

        public double? NewPrice { get; set; }
    }

    public class ProductDetailAdminDTO
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public double Price { get; set; }

        public string Description { get; set; }

        public bool IsVisible { get; set; }

        public int TypeId { get; set; }

        public ICollection<ImageUrlDTO> Images { get; set; }
    }

    public class ProductDetailPromotionDTO
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public double Price { get; set; }

        public double? NewPrice { get; set; }

        public bool IsVisible { get; set; }

        public bool OnSale { get; set; }

        public bool? OnSaleCurrPromotion { get; set; }
    }

    public class ProductListInTypeDTO
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public bool IsVisible { get; set; }
    }

    public class ProductOrderDetailDTO
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public double Price { get; set; }

        public double? NewPrice { get; set; }

        public int Amount { get; set; }
    }
}
