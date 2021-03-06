﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShopAppBackend.Models;
using ShopAppBackend.Models.Context;
using ShopAppBackend.Models.DTOs;
using ShopAppBackend.Services;

namespace ShopAppBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ProductsController : ControllerBase
    {
        private readonly DatabaseContext _context;

        private readonly ImageService _imageService;

        public ProductsController(DatabaseContext context, ImageService imageService)
        {
            _context = context;
            _imageService = imageService;
        }

        [HttpGet("recommend")]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<ProductDisplayDto>>> GetRecommend([FromQuery] int amount)
        {
            return await _context.Product
                .Where(p => p.IsVisible && !p.Archived)
                .Select(p => new ProductDisplayDto
                {
                    Id = p.Id,
                    Name = p.Name,
                    Price = p.Price,
                    NewPrice = p.PromotionItems
                        .FirstOrDefault(pi => pi.Promotion.IsBroadcasted && !pi.Promotion.Archived).NewPrice,
                    ImageUrl = p.ProductImages
                        .Select(pi => new ImageUrlDto
                        {
                            Id = pi.Id,
                            ImageUrl = _imageService.GetImageUrl(pi.ImageFileName)
                        })
                        .FirstOrDefault()
                        .ImageUrl
                })
                .OrderBy(p => Guid.NewGuid())
                .Take(amount)
                .ToListAsync();
        }

        [HttpGet("type/{type}")]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<ProductDisplayDto>>> GetByType(string type)
        {
            if (!await ProductTypeExists(type)) return NotFound();

            var query = _context.Product
                .Where(p => p.IsVisible && !p.Archived && p.Type.Name == type)
                .Select(p => new ProductDisplayDto
                {
                    Id = p.Id,
                    Name = p.Name,
                    Price = p.Price,
                    NewPrice = p.PromotionItems
                        .FirstOrDefault(pi => pi.Promotion.IsBroadcasted && !pi.Promotion.Archived).NewPrice,
                    ImageUrl = p.ProductImages
                        .Select(pi => new ImageUrlDto
                        {
                            Id = pi.Id,
                            ImageUrl = _imageService.GetImageUrl(pi.ImageFileName)
                        })
                        .FirstOrDefault()
                        .ImageUrl
                });

            return await query.ToListAsync();
        }

        [HttpGet("type")]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<ProductTypeDisplayDto>>> GetAllTypeAndProduct([FromQuery] int amount)
        {
            return await _context.ProductType
                .Where(pt => !pt.Archived && pt.Products.Any(p => p.IsVisible && !p.Archived))
                .Select(pt => new ProductTypeDisplayDto
                {
                    Id = pt.Id,
                    Name = pt.Name,
                    ProductList = (ICollection<ProductDisplayDto>) pt.Products
                        .Where(p => p.IsVisible && !p.Archived)
                        .Select(p => new ProductDisplayDto
                        {
                            Id = p.Id,
                            Name = p.Name,
                            Price = p.Price,
                            NewPrice = p.PromotionItems
                                .FirstOrDefault(pi => pi.Promotion.IsBroadcasted && !pi.Promotion.Archived).NewPrice,
                            ImageUrl = p.ProductImages
                                .Select(pi => new ImageUrlDto
                                {
                                    Id = pi.Id,
                                    ImageUrl = _imageService.GetImageUrl(pi.ImageFileName)
                                })
                                .FirstOrDefault()
                                .ImageUrl
                        })
                        .Take(amount)
                })
                .ToListAsync();
        }

        [HttpGet("promotion")]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<PromotionDisplayDto>>> GetAllPromotionAndProduct()
        {
            return await _context.Promotion
                .Where(p => p.IsBroadcasted && !p.Archived && p.PromotionItems.Any(pi =>
                    pi.InPromotionProduct.IsVisible && !pi.InPromotionProduct.Archived))
                .Select(p => new PromotionDisplayDto
                {
                    Id = p.Id,
                    Name = p.Name,
                    Description = p.Description,
                    ProductList = (ICollection<ProductDisplayDto>) p.PromotionItems
                        .Where(pi => pi.InPromotionProduct.IsVisible && !pi.InPromotionProduct.Archived)
                        .Select(pro => new ProductDisplayDto
                        {
                            Id = pro.InPromotionProduct.Id,
                            Name = pro.InPromotionProduct.Name,
                            Price = pro.InPromotionProduct.Price,
                            NewPrice = pro.NewPrice,
                            ImageUrl = pro.InPromotionProduct.ProductImages
                                .Select(pi => new ImageUrlDto
                                {
                                    Id = pi.Id,
                                    ImageUrl = _imageService.GetImageUrl(pi.ImageFileName)
                                })
                                .FirstOrDefault()
                                .ImageUrl
                        })
                })
                .ToListAsync();
        }

        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<ActionResult<ProductDetailDto>> GetProduct(string id)
        {
            var product = await _context.Product
                .Where(p => !p.Archived)
                .Select(p => new ProductDetailDto
                {
                    Id = p.Id,
                    Name = p.Name,
                    Price = p.Price,
                    NewPrice = p.PromotionItems
                        .FirstOrDefault(pi => pi.Promotion.IsBroadcasted && !pi.Promotion.Archived).NewPrice,
                    ImageUrls = (ICollection<ImageUrlDto>) p.ProductImages
                        .Select(pi => new ImageUrlDto
                        {
                            Id = pi.Id,
                            ImageUrl = _imageService.GetImageUrl(pi.ImageFileName)
                        }),
                    Description = p.Description,
                    Promotion = p.PromotionItems
                        .Where(pi => pi.Promotion.IsBroadcasted && !pi.Promotion.Archived)
                        .Select(pi => new PromotionInProductDetailDto
                        {
                            Id = pi.Promotion.Id,
                            Description = pi.Promotion.Description,
                            Name = pi.Promotion.Name
                        })
                        .FirstOrDefault()
                })
                .FirstOrDefaultAsync(i => i.Id == id);

            if (product == null) return NotFound();

            return product;
        }

        [HttpGet("list")]
        public async Task<ActionResult<IEnumerable<ProductListDto>>> GetProductList()
        {
            int.TryParse(User.Claims.FirstOrDefault(claim => claim.Type == "Id")?.Value, out var tokenId);

            if (tokenId != 1) return Unauthorized();

            return await _context.Product
                .Where(p => !p.Archived)
                .Select(p => new ProductListDto
                {
                    Id = p.Id,
                    Name = p.Name,
                    Price = p.Price,
                    Type = p.Type.Name,
                    IsVisible = p.IsVisible,
                    InPromotion = p.PromotionItems.Any(pi => pi.Promotion.IsBroadcasted && !pi.Promotion.Archived),
                    NewPrice = p.PromotionItems
                        .FirstOrDefault(pi => pi.Promotion.IsBroadcasted && !pi.Promotion.Archived).NewPrice
                })
                .ToListAsync();
        }

        [HttpGet("{id}/admin")]
        public async Task<ActionResult<ProductDetailAdminDto>> GetProductAdmin(string id)
        {
            int.TryParse(User.Claims.FirstOrDefault(claim => claim.Type == "Id")?.Value, out var tokenId);

            if (tokenId != 1) return Unauthorized();

            var product = await _context.Product
                .Where(p => !p.Archived)
                .Select(p => new ProductDetailAdminDto
                {
                    Id = p.Id,
                    Name = p.Name,
                    Price = p.Price,
                    Images = (ICollection<ImageUrlDto>) p.ProductImages
                        .Select(pi => new ImageUrlDto
                        {
                            Id = pi.Id,
                            ImageUrl = _imageService.GetImageUrl(pi.ImageFileName)
                        }),
                    Description = p.Description,
                    IsVisible = p.IsVisible,
                    TypeId = p.Type.Id
                })
                .FirstOrDefaultAsync(p => p.Id == id);

            if (product == null) return NotFound();

            return product;
        }

        [HttpGet("search")]
        [AllowAnonymous]
        public async Task<ActionResult<List<ProductListDto>>> SearchProduct([FromQuery]string q)
        {
            int.TryParse(User.Claims.FirstOrDefault(claim => claim.Type == "Id")?.Value, out var tokenId);

            if (tokenId != 1) return Unauthorized();

            var product = await _context.Product
                .Where(p => !p.Archived &&
                            !p.PromotionItems.Any(pi => pi.Promotion.IsBroadcasted && !pi.Promotion.Archived) &&
                            p.Name.Contains(q))
                .Select(p => new ProductListDto
                {
                    Id = p.Id,
                    Name = p.Name,
                    Price = p.Price,
                    IsVisible = p.IsVisible
                })
                .ToListAsync();

            return product;
        }

        [HttpPost]
        public async Task<ActionResult<Product>> AddProduct([FromForm] ProductAddFormDto productAdd)
        {
            int.TryParse(User.Claims.FirstOrDefault(claim => claim.Type == "Id")?.Value, out var tokenId);

            if (tokenId != 1) return Unauthorized();

            var type = new ProductType { Id = productAdd.TypeId };
            _context.Attach(type);
            var newProduct = new Product
            {
                Id = productAdd.Id,
                Name = productAdd.Name,
                Price = productAdd.Price,
                Description = productAdd.Description,
                IsVisible = productAdd.IsVisible,
                Type = type,
                ProductImages = new List<ProductImage>()
            };

            var fileNameList = productAdd.Images.Select(async p =>
            {
                var fileName = await _imageService.Uploader(p);
                var pi = new ProductImage { ImageFileName = fileName };
                newProduct.ProductImages.Add(pi);
            }).ToArray();

            Task.WaitAll(fileNameList);

            

            _context.Product.Add(newProduct);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (await ProductExists(productAdd.Id))
                {
                    return BadRequest(new { Message = "duplicate"});
                }

                throw;
            }
            
            GC.Collect();
            return CreatedAtAction("GetProductAdmin", new { id = newProduct.Id }, newProduct);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<Product>> EditProduct(string id, [FromForm] ProductEditFormDto productEdit)
        {
            int.TryParse(User.Claims.FirstOrDefault(claim => claim.Type == "Id")?.Value, out var tokenId);

            if (tokenId != 1) return Unauthorized();

            var product = await _context.Product.Include(p => p.ProductImages)
                .FirstOrDefaultAsync(p => p.Id == id && !p.Archived);

            if (product == null) return NotFound();

            var type = new ProductType { Id = productEdit.TypeId };
            _context.Attach(type);

            _context.Attach(product);
            product.Name = productEdit.Name;
            product.Price = productEdit.Price;
            product.Description = productEdit.Description;
            product.Type = type;
            product.IsVisible = productEdit.IsVisible;

            IEnumerable<ProductImage> markForDelImages = new List<ProductImage>();
            if (productEdit.MarkForDeleteId != null)
            {
                markForDelImages = product.ProductImages.Where(pi => productEdit.MarkForDeleteId.Contains(pi.Id));
                product.ProductImages = product.ProductImages.Where(pi => !productEdit.MarkForDeleteId.Contains(pi.Id))
                    .ToList();
            }

            var fileNameList = productEdit.Images?.Select(async p =>
            {
                var fileName = await _imageService.Uploader(p);
                var pi = new ProductImage { ImageFileName = fileName };
                product.ProductImages.Add(pi);
            }).ToArray();

            if (fileNameList != null) Task.WaitAll(fileNameList);

            await _context.SaveChangesAsync();

            foreach (var productImage in markForDelImages) _imageService.DeleteFile(productImage.ImageFileName);

            GC.Collect();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<Product>> ArchiveProduct(string id)
        {
            int.TryParse(User.Claims.FirstOrDefault(claim => claim.Type == "Id")?.Value, out var tokenId);

            if (tokenId != 1) return Unauthorized();

            var product = await _context.Product.Include(p => p.ProductImages).FirstOrDefaultAsync(p => p.Id == id);

            if (product == null) return NotFound();

            product.IsVisible = false;
            product.Archived = true;

            foreach (var productImage in product.ProductImages) _imageService.DeleteFile(productImage.ImageFileName);

            product.ProductImages = new List<ProductImage>();

            await _context.SaveChangesAsync();

            return product;
        }

        private Task<bool> ProductExists(string id)
        {
            return _context.Product.AnyAsync(e => e.Id == id);
        }

        private Task<bool> ProductTypeExists(string name)
        {
            return _context.ProductType.AnyAsync(e => e.Name == name);
        }
    }
}
