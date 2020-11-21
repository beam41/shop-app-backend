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
using Microsoft.Extensions.Primitives;

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
                .Where(p => p.IsVisible && !p.Archived)
                .Select(p => new ProductDisplayDTO
                {
                    Id = p.Id,
                    Name = p.Name,
                    Price = p.Price,
                    NewPrice = p.PromotionItems.FirstOrDefault(pi => pi.Promotion.IsBroadcasted && !pi.Promotion.Archived).NewPrice,
                    ImageUrl = p.ProductImages
                        .Select(pi => new ImageUrlDTO
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
                .Where(p => p.IsVisible && !p.Archived && p.Type.Name == type)
                .Select(p => new ProductDisplayDTO
                {
                    Id = p.Id,
                    Name = p.Name,
                    Price = p.Price,
                    NewPrice = p.PromotionItems.FirstOrDefault(pi => pi.Promotion.IsBroadcasted && !pi.Promotion.Archived).NewPrice,
                    ImageUrl = p.ProductImages
                        .Select(pi => new ImageUrlDTO
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
                .Where(pt => !pt.Archived && pt.Products.Any(p => p.IsVisible && !p.Archived))
                .Select(pt => new ProductTypeDisplayDTO
                {
                    Id = pt.Id,
                    Name = pt.Name,
                    ProductList = (ICollection<ProductDisplayDTO>)pt.Products
                        .Where(p => p.IsVisible && !p.Archived)
                        .Select(p => new ProductDisplayDTO
                        {
                            Id = p.Id,
                            Name = p.Name,
                            Price = p.Price,
                            NewPrice = p.PromotionItems.FirstOrDefault(pi => pi.Promotion.IsBroadcasted && !pi.Promotion.Archived).NewPrice,
                            ImageUrl = p.ProductImages
                                .Select(pi => new ImageUrlDTO
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
                .Where(p => p.IsBroadcasted && !p.Archived && p.PromotionItems.Any(pi => pi.InPromotionProduct.IsVisible && !pi.InPromotionProduct.Archived))
                .Select(p => new PromotionDisplayDTO
                {
                    Id = p.Id,
                    Name = p.Name,
                    Description = p.Description,
                    ProductList = (ICollection<ProductDisplayDTO>)p.PromotionItems
                        .Where(pi => pi.InPromotionProduct.IsVisible && !pi.InPromotionProduct.Archived)
                        .Select(pro => new ProductDisplayDTO
                        {
                            Id = pro.InPromotionProduct.Id,
                            Name = pro.InPromotionProduct.Name,
                            Price = pro.InPromotionProduct.Price,
                            NewPrice = pro.NewPrice,
                            ImageUrl = pro.InPromotionProduct.ProductImages
                                .Select(pi => new ImageUrlDTO
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
                .Where(p => !p.Archived)
                .Select(p => new ProductDetailDTO
                {
                    Id = p.Id,
                    Name = p.Name,
                    Price = p.Price,
                    NewPrice = p.PromotionItems.FirstOrDefault(pi => pi.Promotion.IsBroadcasted && !pi.Promotion.Archived).NewPrice,
                    ImageUrls = (ICollection<ImageUrlDTO>)p.ProductImages
                        .Select(pi => new ImageUrlDTO
                        {
                            Id = pi.Id,
                            ImageUrl = _imageService.GetImageUrl(pi.ImageFileName)
                        }),
                    Description = p.Description,
                    Promotion = p.PromotionItems.FirstOrDefault(pi => pi.Promotion.IsBroadcasted && !pi.Promotion.Archived).Promotion
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
                .Where(p => !p.Archived)
                .Select(p => new ProductListDTO
                {
                    Id = p.Id,
                    Name = p.Name,
                    Price = p.Price,
                    Type = p.Type.Name,
                    IsVisible = p.IsVisible,
                    InPromotion = p.PromotionItems.Any(pi => pi.Promotion.IsBroadcasted && !pi.Promotion.Archived),
                    NewPrice = p.PromotionItems.FirstOrDefault(pi => pi.Promotion.IsBroadcasted && !pi.Promotion.Archived).NewPrice
                })
                .ToListAsync();
        }

        [HttpGet("{id}/admin")]
        public async Task<ActionResult<ProductDetailAdminDTO>> GetProductAdmin(int id)
        {
            int.TryParse(User.Claims.FirstOrDefault(claim => claim.Type == "Id")?.Value, out int tokenId);

            if (tokenId != 1)
            {
                return Unauthorized();
            }

            var product = await _context.Product
                .Where(p => !p.Archived)
                .Select(p => new ProductDetailAdminDTO
                {
                    Id = p.Id,
                    Name = p.Name,
                    Price = p.Price,
                    Images = (ICollection<ImageUrlDTO>)p.ProductImages
                        .Select(pi => new ImageUrlDTO
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
        public async Task<ActionResult<List<ProductListDTO>>> SearchProduct()
        {
            int.TryParse(User.Claims.FirstOrDefault(claim => claim.Type == "Id")?.Value, out int tokenId);

            if (tokenId != 1)
            {
                return Unauthorized();
            }

            var query = Request.Query["q"];
            if (StringValues.IsNullOrEmpty(query)) return BadRequest();

            var product = await _context.Product
                .Where(p => !p.Archived && !p.PromotionItems.Any(pi => pi.Promotion.IsBroadcasted && !pi.Promotion.Archived) && p.Name.Contains(query))
                .Select(p => new ProductListDTO
                {
                    Id = p.Id,
                    Name = p.Name,
                    Price = p.Price,
                    IsVisible = p.IsVisible,
                })
                .ToListAsync();

            return product;
        }

        [HttpPost]
        public async Task<ActionResult<Product>> AddProduct([FromForm] ProductAddFormDTO productAdd)
        {
            int.TryParse(User.Claims.FirstOrDefault(claim => claim.Type == "Id")?.Value, out int tokenId);

            if (tokenId != 1)
            {
                return Unauthorized();
            }

            var type = new ProductType { Id = productAdd.TypeId };
            _context.Attach(type);
            Product newProduct = productAdd;
            newProduct.Type = type;

            newProduct.ProductImages = new List<ProductImage>();
            var fileNameList = productAdd.Images.Select(async p =>
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

        [HttpPut("{id}")]
        public async Task<ActionResult<Product>> EditProduct(int id, [FromForm] ProductEditFormDTO productEdit)
        {
            int.TryParse(User.Claims.FirstOrDefault(claim => claim.Type == "Id")?.Value, out int tokenId);

            if (tokenId != 1)
            {
                return Unauthorized();
            }

            var product = await _context.Product.Include(p => p.ProductImages).FirstOrDefaultAsync(p => p.Id == id && !p.Archived);

            if (product == null)
            {
                return NotFound();
            }

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
                product.ProductImages = product.ProductImages.Where(pi => !productEdit.MarkForDeleteId.Contains(pi.Id)).ToList();
            }

            var fileNameList = productEdit.Images?.Select(async p =>
            {
                var fileName = await _imageService.Uploader(p);
                var pi = new ProductImage { ImageFileName = fileName };
                product.ProductImages.Add(pi);
            }).ToArray();

            if (fileNameList != null)
            {
                Task.WaitAll(fileNameList);
            }

            await _context.SaveChangesAsync();

            foreach (var productImage in markForDelImages)
            {
                _imageService.DeleteFile(productImage.ImageFileName);
            }

            GC.Collect();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<Product>> ArchiveProduct(int id)
        {
            int.TryParse(User.Claims.FirstOrDefault(claim => claim.Type == "Id")?.Value, out int tokenId);

            if (tokenId != 1)
            {
                return Unauthorized();
            }

            var product = await _context.Product.Include(p => p.ProductImages).FirstOrDefaultAsync(p => p.Id == id);

            if (product == null)
            {
                return NotFound();
            }

            product.IsVisible = false;
            product.Archived = true;

            foreach (var productImage in product.ProductImages)
            {
                _imageService.DeleteFile(productImage.ImageFileName);
            }

            product.ProductImages = new List<ProductImage>();

            await _context.SaveChangesAsync();

            return product;
        }

        private Task<bool> ProductTypeExists(string name)
        {
            return _context.ProductType.AnyAsync(e => e.Name == name);
        }
    }
}
