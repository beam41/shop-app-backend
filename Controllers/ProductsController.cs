using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShopAppBackend.Models;
using ShopAppBackend.Models.Context;
using ShopAppBackend.Services;
using ShopAppBackend.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

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

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Product>>> GetProduct()
        {
            return await _context.Product
                .Include(p => p.ProductImages)
                .Include(p => p.Type)
                .Include(p => p.PromotionItems)
                .ThenInclude(pi => pi.Promotion)
                .ToListAsync();
        }

        [HttpGet("recommend")]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<ProductDisplayDTO>>> GetRecommend()
        {
            if (!int.TryParse(Request.Query["amount"], out var amount)) return BadRequest();

            return await _context.Product
                .Where(p => p.IsVisible)
                .Select(p => new ProductDisplayDTO
                {
                    Id = p.Id,
                    Name = p.Name,
                    Price = p.Price,
                    NewPrice = p.PromotionItems.FirstOrDefault(pi => pi.Promotion.IsBroadcasted).NewPrice,
                    ImageUrl = p.ProductImages
                        .Select(pi => new ProductImageUrlDTO
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
        public async Task<ActionResult<IEnumerable<ProductDisplayDTO>>> GetByType(string type)
        {
            if (!await ProductTypeExists(type)) return NotFound();

            var query = _context.Product
                .Where(p => p.IsVisible && p.Type.Name == type)
                .Select(p => new ProductDisplayDTO
                {
                    Id = p.Id,
                    Name = p.Name,
                    Price = p.Price,
                    NewPrice = p.PromotionItems.FirstOrDefault(pi => pi.Promotion.IsBroadcasted).NewPrice,
                    ImageUrl = p.ProductImages
                        .Select(pi => new ProductImageUrlDTO
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
        public async Task<ActionResult<IEnumerable<ProductTypeDisplayDTO>>> GetAllTypeAndProduct()
        {
            if (!int.TryParse(Request.Query["amount"], out var amount)) return BadRequest();

            return await _context.ProductType
                .Where(pt => pt.Products.Any())
                .Select(pt => new ProductTypeDisplayDTO
                {
                    Id = pt.Id,
                    Name = pt.Name,
                    ProductList = (ICollection<ProductDisplayDTO>)pt.Products
                        .Where(p => p.IsVisible)
                        .Select(p => new ProductDisplayDTO
                        {
                            Id = p.Id,
                            Name = p.Name,
                            Price = p.Price,
                            NewPrice = p.PromotionItems.FirstOrDefault(pi => pi.Promotion.IsBroadcasted).NewPrice,
                            ImageUrl = p.ProductImages
                                .Select(pi => new ProductImageUrlDTO
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
        public async Task<ActionResult<IEnumerable<PromotionDisplayDTO>>> GetAllPromotionAndProduct()
        {
            return await _context.Promotion
                .Where(p => p.IsBroadcasted)
                .Select(p => new PromotionDisplayDTO
                {
                    Id = p.Id,
                    Name = p.Name,
                    Description = p.Description,
                    ProductList = (ICollection<ProductDisplayDTO>)p.PromotionItems
                        .Where(pi => pi.InPromotionProduct.IsVisible)
                        .Select(pro => new ProductDisplayDTO
                        {
                            Id = pro.InPromotionProduct.Id,
                            Name = pro.InPromotionProduct.Name,
                            Price = pro.InPromotionProduct.Price,
                            NewPrice = pro.NewPrice,
                            ImageUrl = pro.InPromotionProduct.ProductImages
                                .Select(pi => new ProductImageUrlDTO
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
        public async Task<ActionResult<ProductDetailDTO>> GetProduct(int id)
        {
            var product = await _context.Product
                .Select(p => new ProductDetailDTO
                {
                    Id = p.Id,
                    Name = p.Name,
                    Price = p.Price,
                    NewPrice = p.PromotionItems.FirstOrDefault(pi => pi.Promotion.IsBroadcasted).NewPrice,
                    ImageUrls = (ICollection<ProductImageUrlDTO>)p.ProductImages
                        .Select(pi => new ProductImageUrlDTO
                        {
                            Id = pi.Id,
                            ImageUrl = _imageService.GetImageUrl(pi.ImageFileName)
                        }),
                    Description = p.Description,
                    Promotion = p.PromotionItems.FirstOrDefault(pi => pi.Promotion.IsBroadcasted).Promotion
                })
                .FirstOrDefaultAsync(i => i.Id == id);

            if (product == null) return NotFound();

            return product;
        }

        [HttpGet("list")]
        public async Task<ActionResult<IEnumerable<ProductListDTO>>> GetProductList()
        {
            int.TryParse(User.Claims.FirstOrDefault(claim => claim.Type == "Id")?.Value, out int tokenId);

            if (tokenId != 1)
            {
                return Unauthorized();
            }

            return await _context.Product
                .Select(p => new ProductListDTO
                {
                    Id = p.Id,
                    Name = p.Name,
                    Price = p.Price,
                    Type = p.Type.Name,
                    IsVisible = p.IsVisible,
                    InPromotion = p.PromotionItems.Any(pi => pi.Promotion.IsBroadcasted),
                    NewPrice = p.PromotionItems.FirstOrDefault(pi => pi.Promotion.IsBroadcasted).NewPrice
                })
                .ToListAsync();
        }

        [HttpGet("{id}/admin")]
        [AllowAnonymous]
        public async Task<ActionResult<ProductDetailAdminDTO>> GetProductAdmin(int id)
        {
            var product = await _context.Product
                .Select(p => new ProductDetailAdminDTO
                {
                    Id = p.Id,
                    Name = p.Name,
                    Price = p.Price,
                    Images = (ICollection<ProductImageUrlDTO>)p.ProductImages
                        .Select(pi => new ProductImageUrlDTO
                        {
                            Id = pi.Id,
                            ImageUrl = _imageService.GetImageUrl(pi.ImageFileName)
                        }),
                    Description = p.Description,
                    IsVisible = p.IsVisible,
                    TypeId = p.Type.Id
                })
                .FirstOrDefaultAsync(i => i.Id == id);

            if (product == null) return NotFound();

            return product;
        }

        [HttpPost]
        public async Task<ActionResult<Product>> PostProduct([FromForm] ProductFormDTO product)
        {
            int.TryParse(User.Claims.FirstOrDefault(claim => claim.Type == "Id")?.Value, out int tokenId);

            if (tokenId != 1)
            {
                return Unauthorized();
            }

            var type = new ProductType { Id = product.TypeId };
            _context.Attach(type);
            Product newProduct = product;
            newProduct.Type = type;

            newProduct.ProductImages = new List<ProductImage>();
            var fileNameList = product.Images.Select(async p =>
            {
                var fileName = await _imageService.Uploader(p);
                var pi = new ProductImage { ImageFileName = fileName };
                newProduct.ProductImages.Add(pi);
            }).ToArray();

            Task.WaitAll(fileNameList);

            _context.Product.Add(newProduct);

            await _context.SaveChangesAsync();
            GC.Collect();
            return CreatedAtAction("GetProductAdmin", new { id = newProduct.Id }, newProduct);
        }

        private Task<bool> ProductExists(int id)
        {
            return _context.Product.AnyAsync(e => e.Id == id);
        }

        private Task<bool> ProductTypeExists(string name)
        {
            return _context.ProductType.AnyAsync(e => e.Name == name);
        }
    }
}
