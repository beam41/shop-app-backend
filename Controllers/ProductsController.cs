using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;
using ShopAppBackend.Models;
using ShopAppBackend.Models.Context;

namespace ShopAppBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly DatabaseContext _context;

        public ProductsController(DatabaseContext context)
        {
            _context = context;
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
                })
                .OrderBy(p => Guid.NewGuid())
                .Take(int.Parse(Request.Query["amount"]))
                .ToListAsync();
        }

        [HttpGet("type/{type}")]
        public async Task<ActionResult<IEnumerable<ProductDisplayDTO>>> GetByType(string type)
        {
            if (!ProductTypeExists(type))
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
                        .Select(p => new ProductDisplayDTO
                        {
                            Id = p.Id,
                            Name = p.Name,
                            Price = p.Price,
                            NewPrice = p.PromotionItems.FirstOrDefault(pi => pi.Promotion.IsBroadcasted).NewPrice,
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
                        })
                })
                .ToListAsync();
        }

        // GET: api/Products/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Product>> GetProduct(int id)
        {
            var product = await _context.Product.Include(p => p.ProductImages).FirstOrDefaultAsync(i => i.Id == id);

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

        private bool ProductExists(int id)
        {
            return _context.Product.Any(e => e.Id == id);
        }

        private bool ProductTypeExists(string name)
        {
            return _context.ProductType.Any(e => e.Name == name);
        }
    }
}
