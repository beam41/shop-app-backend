using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShopAppBackend.Models;
using ShopAppBackend.Models.Context;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

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

        // GET: api/ProductTypes
        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<ProductType>>> GetProductType()
        {
            return await _context.ProductType.Where(pt => !pt.Archived && pt.Products.Count > 0).ToListAsync();
        }

        [HttpGet("admin")]
        public async Task<ActionResult<IEnumerable<ProductType>>> GetProductTypeAdmin()
        {
            int.TryParse(User.Claims.FirstOrDefault(claim => claim.Type == "Id")?.Value, out int tokenId);

            if (tokenId != 1)
            {
                return Unauthorized();
            }

            return await _context.ProductType.Where(pt => !pt.Archived).ToListAsync();
        }

        [HttpGet("list")]
        public async Task<ActionResult<IEnumerable<ProductTypeListDTO>>> GetProductTypeList()
        {
            int.TryParse(User.Claims.FirstOrDefault(claim => claim.Type == "Id")?.Value, out int tokenId);

            if (tokenId != 1)
            {
                return Unauthorized();
            }

            return await _context.ProductType
                .Where(pt => !pt.Archived)
                .Select(pt => new ProductTypeListDTO
                {
                    Id = pt.Id,
                    Name = pt.Name,
                    ProductCount = pt.Products.Count
                }).ToListAsync();
        }

        // GET: api/ProductTypes/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ProductTypeDetailDTO>> GetProductType(int id)
        {
            int.TryParse(User.Claims.FirstOrDefault(claim => claim.Type == "Id")?.Value, out int tokenId);

            if (tokenId != 1)
            {
                return Unauthorized();
            }

            var productType = await _context.ProductType
                .Where(pt => !pt.Archived)
                .Select(p => new ProductTypeDetailDTO
                {
                    Id = p.Id,
                    Name = p.Name,
                    ProductList = (ICollection<ProductListInTypeDTO>) p.Products.Select(pro => new ProductListInTypeDTO
                    {
                        Id = pro.Id,
                        Name = pro.Name,
                        IsVisible = pro.IsVisible
                    })
                }).FirstOrDefaultAsync(p => p.Id == id);

            if (productType == null)
            {
                return NotFound();
            }

            return productType;
        }

        // PUT: api/ProductTypes/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPut("{id}")]
        public async Task<IActionResult> EditProductType(int id, ProductTypeInputDTO productType)
        {
            int.TryParse(User.Claims.FirstOrDefault(claim => claim.Type == "Id")?.Value, out int tokenId);

            if (tokenId != 1)
            {
                return Unauthorized();
            }

            var productTypeOld = new ProductType() { Id = id };
            _context.Attach(productTypeOld);

            productTypeOld.Name = productType.Name;

            await _context.SaveChangesAsync();

            return NoContent();
        }

        // POST: api/ProductTypes
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPost]
        public async Task<ActionResult<ProductType>> AddProductType(ProductTypeInputDTO productType)
        {
            int.TryParse(User.Claims.FirstOrDefault(claim => claim.Type == "Id")?.Value, out int tokenId);

            if (tokenId != 1)
            {
                return Unauthorized();
            }

            var newProductType = new ProductType() { Name = productType.Name };

            _context.ProductType.Add(newProductType);

            await _context.SaveChangesAsync();

            return CreatedAtAction("GetProductType", new { id = newProductType.Id }, productType);
        }

        // DELETE: api/ProductTypes/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<ProductType>> ArchiveProductType(int id)
        {
            int.TryParse(User.Claims.FirstOrDefault(claim => claim.Type == "Id")?.Value, out int tokenId);

            if (tokenId != 1)
            {
                return Unauthorized();
            }

            var productType = await _context.ProductType.Include(pt => pt.Products).FirstOrDefaultAsync(pt => pt.Id == id);

            if (productType == null)
            {
                return NotFound();
            }

            if (productType.Products.Count > 0)
            {
                return BadRequest();
            }

            productType.Archived = true;

            await _context.SaveChangesAsync();

            return productType;
        }
    }
}
