using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Azure.Storage.Blobs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;
using ShopAppBackend.Models;
using ShopAppBackend.Models.Context;
using ShopAppBackend.Services;
using ShopAppBackend.Settings;

namespace ShopAppBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly DatabaseContext _context;

        private readonly ImageService _imageService;

        private readonly IImageSettings _imageSettings;


        public ProductsController(DatabaseContext context, ImageService imageService, IImageSettings imageSettings)
        {
            _context = context;
            _imageService = imageService;
            _imageSettings = imageSettings;
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
        public async Task<ActionResult<IEnumerable<ProductDisplayDTO>>> GetRecommend()
        {
            return await _context.Product
                .Where(p => p.IsVisible)
                .Select(p => new ProductDisplayDTO
                {
                    Id = p.Id,
                    Name = p.Name,
                    Price = p.Price,
                    NewPrice = p.PromotionItems.FirstOrDefault(pi => pi.Promotion.IsBroadcasted).NewPrice,
                    ImgUrl = p.ProductImages
                        .Select(pi => new ProductImageUrlDTO
                        {
                            Id = pi.Id,
                            ImageUrl = Flurl.Url.Combine(_imageSettings.BlobPath, _imageSettings.ContainerName, pi.ImageFileName)
                        })
                        .FirstOrDefault()
                        .ImageUrl,
                })
                .OrderBy(p => Guid.NewGuid())
                .Take(int.Parse(Request.Query["amount"]))
                .ToListAsync();
        }

        [HttpGet("type/{type}")]
        public async Task<ActionResult<IEnumerable<ProductDisplayDTO>>> GetByType(string type)
        {
            if (!await ProductTypeExists(type))
            {
                return NotFound();
            }

            var query = _context.Product
                .Where(p => p.IsVisible && p.Type.Name == type)
                .Select(p => new ProductDisplayDTO
                {
                    Id = p.Id,
                    Name = p.Name,
                    Price = p.Price,
                    NewPrice = p.PromotionItems.FirstOrDefault(pi => pi.Promotion.IsBroadcasted).NewPrice,
                    ImgUrl = p.ProductImages
                        .Select(pi => new ProductImageUrlDTO
                        {
                            Id = pi.Id,
                            ImageUrl = Flurl.Url.Combine(_imageSettings.BlobPath, _imageSettings.ContainerName, pi.ImageFileName)
                        })
                        .FirstOrDefault()
                        .ImageUrl,
                });

            if (Request.Query["amount"] == StringValues.Empty)
            {
                return await query.ToListAsync();
            }

            return await query.Take(int.Parse(Request.Query["amount"])).ToListAsync();
        }

        [HttpGet("type")]
        public async Task<ActionResult<IEnumerable<ProductTypeDisplayDTO>>> GetAllTypeAndProduct()
        {
            return await _context.ProductType
                .Where(pt => pt.Products.Count() > 0)
                .Select(pt => new ProductTypeDisplayDTO
                {
                    Id = pt.Id,
                    Name = pt.Name,
                    ProductList = (ICollection<ProductDisplayDTO>) pt.Products
                        .Where(p => p.IsVisible)
                        .Select(p => new ProductDisplayDTO
                        {
                            Id = p.Id,
                            Name = p.Name,
                            Price = p.Price,
                            NewPrice = p.PromotionItems.FirstOrDefault(pi => pi.Promotion.IsBroadcasted).NewPrice,
                            ImgUrl = p.ProductImages
                                .Select(pi => new ProductImageUrlDTO
                                {
                                    Id = pi.Id,
                                    ImageUrl = Flurl.Url.Combine(_imageSettings.BlobPath, _imageSettings.ContainerName, pi.ImageFileName)
                                })
                                .FirstOrDefault()
                                .ImageUrl,
                        })
                        .Take(int.Parse(Request.Query["amount"]))
                })
                .ToListAsync();
        }

        [HttpGet("promotion")]
        public async Task<ActionResult<IEnumerable<PromotionDisplayDTO>>> GetAllPromotionAndProduct()
        {
            return await _context.Promotion
                .Where(p => p.IsBroadcasted)
                .Select(p => new PromotionDisplayDTO
                {
                    Id = p.Id,
                    Name = p.Name,
                    Description = p.Description,
                    ProductList = (ICollection<ProductDisplayDTO>) p.PromotionItems
                        .Where(pi => pi.InPromotionProduct.IsVisible)
                        .Select(pro => new ProductDisplayDTO
                        {
                            Id = pro.InPromotionProduct.Id,
                            Name = pro.InPromotionProduct.Name,
                            Price = pro.InPromotionProduct.Price,
                            NewPrice = pro.NewPrice,
                            ImgUrl = pro.InPromotionProduct.ProductImages
                                .Select(pi => new ProductImageUrlDTO
                                {
                                    Id = pi.Id,
                                    ImageUrl = Flurl.Url.Combine(_imageSettings.BlobPath, _imageSettings.ContainerName, pi.ImageFileName)
                                })
                                .FirstOrDefault()
                                .ImageUrl,
                        })
                })
                .ToListAsync();
        }

        // GET: api/Products/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ProductDetailDTO>> GetProduct(int id)
        {
            var product = await _context.Product
                .Select(p => new ProductDetailDTO
                {
                    Id = p.Id,
                    Name = p.Name,
                    Price = p.Price,
                    NewPrice = p.PromotionItems.FirstOrDefault(pi => pi.Promotion.IsBroadcasted).NewPrice,
                    ImgUrls = (ICollection<ProductImageUrlDTO>) p.ProductImages
                        .Select(pi => new ProductImageUrlDTO
                        {
                            Id = pi.Id,
                            ImageUrl = Flurl.Url.Combine(_imageSettings.BlobPath, _imageSettings.ContainerName, pi.ImageFileName)
                        }),
                    Description = p.Description,
                    Promotion = p.PromotionItems.FirstOrDefault(pi => pi.Promotion.IsBroadcasted).Promotion
                }).FirstOrDefaultAsync(i => i.Id == id);

            if (product == null)
            {
                return NotFound();
            }

            return product;
        }

        [HttpPost]
        public async Task<ActionResult<Product>> PostProduct(ProductFormDTO product)
        {
            var type = new ProductType { Id = product.TypeId };
            _context.Attach(type);
            Product newProduct = product;
            newProduct.Type = type;
            _context.Product.Add(newProduct);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetProduct", new { id = newProduct.Id }, newProduct);
        }

        [HttpPost("img")]
        public async Task<ActionResult<Product>> PostProductImg([FromForm] ProductFormDTO product)
        {
            var type = new ProductType { Id = product.TypeId };
            _context.Attach(type);
            Product newProduct = product;
            newProduct.Type = type;

            newProduct.ProductImages = new List<ProductImage>();
            var fileNameList = product.Images.Select(async p => {
                var fileName = await _imageService.Uploader(p);
                ProductImage pi = new ProductImage { ImageFileName = fileName };
                newProduct.ProductImages.Add(pi);
            }).ToArray();

            Task.WaitAll(fileNameList);

            _context.Product.Add(newProduct);

            await _context.SaveChangesAsync();
            GC.Collect();
            return CreatedAtAction("GetProduct", new { id = newProduct.Id }, newProduct);
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
