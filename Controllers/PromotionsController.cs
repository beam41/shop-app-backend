using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShopAppBackend.Models;
using ShopAppBackend.Models.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ShopAppBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PromotionsController : ControllerBase
    {
        private readonly DatabaseContext _context;

        public PromotionsController(DatabaseContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Promotion>>> GetPromotion()
        {
            return await _context.Promotion.Include(p => p.PromotionItems).ToListAsync();
        }

        [HttpGet("list")]
        public async Task<ActionResult<IEnumerable<PromotionListDTO>>> GetPromotionList()
        {
            int.TryParse(User.Claims.FirstOrDefault(claim => claim.Type == "Id")?.Value, out int tokenId);

            if (tokenId != 1)
            {
                return Unauthorized();
            }

            return await _context.Promotion
                .Where(p => !p.Archived)
                .Select(p => new PromotionListDTO()
                {
                    Id = p.Id,
                    IsBroadcasted = p.IsBroadcasted,
                    Name = p.Name,
                    ItemsCount = p.PromotionItems.Count(p => !p.InPromotionProduct.Archived)
                }).ToListAsync();
        }

        // GET: api/Promotions/5
        [HttpGet("{id}")]
        public async Task<ActionResult<PromotionDetailDTO>> GetPromotion(int id)
        {
            int.TryParse(User.Claims.FirstOrDefault(claim => claim.Type == "Id")?.Value, out int tokenId);

            if (tokenId != 1)
            {
                return Unauthorized();
            }

            var promotion = await _context.Promotion.Select(pro => new PromotionDetailDTO()
            {
                Id = pro.Id,
                Description = pro.Description,
                IsBroadcasted = pro.IsBroadcasted,
                Name = pro.Name,
                PromotionItems = (ICollection<ProductDetailPromotionDTO>)pro.PromotionItems
                    .Where(pi => pi.InPromotionProduct.IsVisible && !pi.InPromotionProduct.Archived)
                    .Select(pi => new ProductDetailPromotionDTO
                    {
                        Id = pi.InPromotionProduct.Id,
                        Name = pi.InPromotionProduct.Name,
                        Price = pi.InPromotionProduct.Price,
                        NewPrice = pi.NewPrice,
                        IsVisible = pi.InPromotionProduct.IsVisible,
                        OnSale = pi
                            .InPromotionProduct
                            .PromotionItems
                            .Any(pip => pip.Promotion.IsBroadcasted && !pip.Promotion.Archived),
                        OnSaleCurrPromotion =
                            pi
                                .InPromotionProduct
                                .PromotionItems
                                .Any(pip => pip.Promotion.IsBroadcasted && !pip.Promotion.Archived)
                                ? pi
                                    .InPromotionProduct
                                    .PromotionItems
                                    .FirstOrDefault(pip => pip.Promotion.IsBroadcasted && !pip.Promotion.Archived)
                                    .Promotion
                                    .Id == id
                                : (bool?)null
                    })
            }).FirstOrDefaultAsync(p => p.Id == id);

            if (promotion == null)
            {
                return NotFound();
            }

            return promotion;
        }

        // POST: api/Promotions
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPost]
        public async Task<ActionResult<Promotion>> PostPromotion(PromotionFormDTO promotion)
        {
            int.TryParse(User.Claims.FirstOrDefault(claim => claim.Type == "Id")?.Value, out int tokenId);

            if (tokenId != 1)
            {
                return Unauthorized();
            }

            var productIdList = promotion.PromotionItems.Select(p => p.ProductId);

            if (promotion.IsBroadcasted && await ProductPromotionIsActive(productIdList)) return BadRequest();

            Promotion newPromotion = promotion;
            _context.Promotion.Add(newPromotion);
            var promotionItems = new List<PromotionItem>();
            var products = new List<Product>();
            foreach (PromotionItemsDTO promotionItemDTO in promotion.PromotionItems)
            {
                var product = new Product { Id = promotionItemDTO.ProductId };
                products.Add(product);
                var promotionItem = new PromotionItem
                {
                    Promotion = newPromotion,
                    InPromotionProduct = product,
                    NewPrice = promotionItemDTO.NewPrice
                };

                promotionItems.Add(promotionItem);
            }

            _context.AttachRange(products);
            _context.PromotionItem.AddRange(promotionItems);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetPromotion", new { id = newPromotion.Id }, newPromotion);

        }

        [HttpPut("{id}")]
        public async Task<ActionResult<Promotion>> PutPromotion(int id, PromotionFormDTO promotion)
        {
            int.TryParse(User.Claims.FirstOrDefault(claim => claim.Type == "Id")?.Value, out int tokenId);

            if (tokenId != 1)
            {
                return Unauthorized();
            }

            var productIdList = promotion.PromotionItems.Select(p => p.ProductId);

            if (promotion.IsBroadcasted && await ProductPromotionIsActive(productIdList, id)) return BadRequest();

            var newPromotion = await _context.Promotion.Include(p => p.PromotionItems).FirstOrDefaultAsync(p => p.Id == id && !p.Archived);

            if (newPromotion == null)
            {
                return NotFound();
            }

            _context.Promotion.Attach(newPromotion);

            var promotionItems = new List<PromotionItem>();
            var products = new List<Product>();
            foreach (PromotionItemsDTO promotionItemDTO in promotion.PromotionItems)
            {
                var product = new Product { Id = promotionItemDTO.ProductId };
                products.Add(product);
                var promotionItem = new PromotionItem
                {
                    Promotion = newPromotion,
                    InPromotionProduct = product,
                    NewPrice = promotionItemDTO.NewPrice
                };

                promotionItems.Add(promotionItem);
            }

            _context.AttachRange(products);

            newPromotion.Name = promotion.Name;
            newPromotion.Description = promotion.Description;
            newPromotion.IsBroadcasted = promotion.IsBroadcasted;
            newPromotion.PromotionItems = promotionItems;

            await _context.SaveChangesAsync();

            return NoContent();

        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<Promotion>> ArchivePromotion(int id)
        {
            int.TryParse(User.Claims.FirstOrDefault(claim => claim.Type == "Id")?.Value, out int tokenId);

            if (tokenId != 1)
            {
                return Unauthorized();
            }

            var promotion = await _context.Promotion.FirstOrDefaultAsync(p => p.Id == id);

            if (promotion == null)
            {
                return NotFound();
            }

            promotion.IsBroadcasted = false;
            promotion.Archived = true;

            await _context.SaveChangesAsync();

            return promotion;
        }

        private Task<bool> ProductPromotionIsActive(IEnumerable<int> productId)
        {
            return _context.PromotionItem
                .AnyAsync(pi => pi.Promotion.IsBroadcasted && !pi.Promotion.Archived && productId.Contains(pi.InPromotionProduct.Id));
        }

        private Task<bool> ProductPromotionIsActive(IEnumerable<int> productId, int promotionId)
        {
            return _context.PromotionItem
                .AnyAsync(pi => pi.Promotion.IsBroadcasted && !pi.Promotion.Archived && pi.Promotion.Id != promotionId && productId.Contains(pi.InPromotionProduct.Id));
        }

        private Task<bool> PromotionExists(int id)
        {
            return _context.Promotion.AnyAsync(e => e.Id == id);
        }
    }
}
