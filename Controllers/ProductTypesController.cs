using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShopAppBackend.Models;
using ShopAppBackend.Models.Context;
using ShopAppBackend.Models.DTOs;

namespace ShopAppBackend.Controllers
{
    [Route("api/types")]
    [ApiController]
    [Authorize]
    public class ProductTypesController : ControllerBase
    {
        private readonly DatabaseContext _context;

        public ProductTypesController(DatabaseContext context)
        {
            _context = context;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<ProductType>>> GetProductType()
        {
            return await _context.ProductType.Where(pt => !pt.Archived && pt.Products.Count(p => !p.Archived) > 0)
                .ToListAsync();
        }

        [HttpGet("admin")]
        public async Task<ActionResult<IEnumerable<ProductType>>> GetProductTypeAdmin()
        {
            int.TryParse(User.Claims.FirstOrDefault(claim => claim.Type == "Id")?.Value, out var tokenId);

            if (tokenId != 1) return Unauthorized();

            return await _context.ProductType.Where(pt => !pt.Archived).ToListAsync();
        }

        [HttpGet("list")]
        public async Task<ActionResult<IEnumerable<ProductTypeListDto>>> GetProductTypeList()
        {
            int.TryParse(User.Claims.FirstOrDefault(claim => claim.Type == "Id")?.Value, out var tokenId);

            if (tokenId != 1) return Unauthorized();

            return await _context.ProductType
                .Where(pt => !pt.Archived)
                .Select(pt => new ProductTypeListDto
                {
                    Id = pt.Id,
                    Name = pt.Name,
                    ProductCount = pt.Products.Count(p => !p.Archived)
                }).ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ProductTypeDetailDto>> GetProductType(int id)
        {
            int.TryParse(User.Claims.FirstOrDefault(claim => claim.Type == "Id")?.Value, out var tokenId);

            if (tokenId != 1) return Unauthorized();

            var productType = await _context.ProductType
                .Where(pt => !pt.Archived)
                .Select(pt => new ProductTypeDetailDto
                {
                    Id = pt.Id,
                    Name = pt.Name,
                    ProductList = (ICollection<ProductListInTypeDto>) pt.Products
                        .Where(p => !p.Archived)
                        .Select(pro => new ProductListInTypeDto
                        {
                            Id = pro.Id,
                            Name = pro.Name,
                            IsVisible = pro.IsVisible
                        })
                }).FirstOrDefaultAsync(p => p.Id == id);

            if (productType == null) return NotFound();

            return productType;
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> EditProductType(int id, ProductTypeInputDto productType)
        {
            int.TryParse(User.Claims.FirstOrDefault(claim => claim.Type == "Id")?.Value, out var tokenId);

            if (tokenId != 1) return Unauthorized();

            var productTypeOld = new ProductType { Id = id, Name = productType.Name };
            _context.Entry(productTypeOld).State = EntityState.Modified;

            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpPost]
        public async Task<ActionResult<ProductType>> AddProductType(ProductTypeInputDto productType)
        {
            int.TryParse(User.Claims.FirstOrDefault(claim => claim.Type == "Id")?.Value, out var tokenId);

            if (tokenId != 1) return Unauthorized();

            var newProductType = new ProductType { Name = productType.Name };

            _context.ProductType.Add(newProductType);

            await _context.SaveChangesAsync();

            return CreatedAtAction("GetProductType", new { id = newProductType.Id }, productType);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<ProductType>> ArchiveProductType(int id)
        {
            int.TryParse(User.Claims.FirstOrDefault(claim => claim.Type == "Id")?.Value, out var tokenId);

            if (tokenId != 1) return Unauthorized();

            var productType = await _context.ProductType.Where(pt => pt.Products.All(p => p.Archived))
                .FirstOrDefaultAsync(pt => pt.Id == id);

            if (productType == null) return NotFound();

            productType.Archived = true;

            await _context.SaveChangesAsync();

            return productType;
        }
    }
}
