using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShopAppBackend.Models;
using ShopAppBackend.Models.Context;
using ShopAppBackend.Models.DTOs;

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

        [HttpGet("list")]
        public async Task<ActionResult<IEnumerable<PromotionListDto>>> GetPromotionList()
        {
            int.TryParse(User.Claims.FirstOrDefault(claim => claim.Type == "Id")?.Value, out var tokenId);

            if (tokenId != 1) return Unauthorized();

            return await _context.Promotion
                .Where(p => !p.Archived)
                .Select(p => new PromotionListDto
                {
                    Id = p.Id,
                    IsBroadcasted = p.IsBroadcasted,
                    Name = p.Name,
                    ItemsCount = p.PromotionItems.Count(pi => !pi.InPromotionProduct.Archived)
                }).ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<PromotionDetailDto>> GetPromotion(int id)
        {
            int.TryParse(User.Claims.FirstOrDefault(claim => claim.Type == "Id")?.Value, out var tokenId);

            if (tokenId != 1) return Unauthorized();

            var promotion = await _context.Promotion.Select(pro => new PromotionDetailDto
            {
                Id = pro.Id,
                Description = pro.Description,
                IsBroadcasted = pro.IsBroadcasted,
                Name = pro.Name,
                PromotionItems = (ICollection<ProductDetailPromotionDto>) pro.PromotionItems
                    .Where(pi => pi.InPromotionProduct.IsVisible && !pi.InPromotionProduct.Archived)
                    .Select(pi => new ProductDetailPromotionDto
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
                                : (bool?) null
                    })
            }).FirstOrDefaultAsync(p => p.Id == id);

            if (promotion == null) return NotFound();

            return promotion;
        }

        [HttpPost]
        public async Task<ActionResult<Promotion>> PostPromotion(PromotionFormDto promotion)
        {
            int.TryParse(User.Claims.FirstOrDefault(claim => claim.Type == "Id")?.Value, out var tokenId);

            if (tokenId != 1) return Unauthorized();

            var productIdList = promotion.PromotionItems.Select(p => p.ProductId);

            if (promotion.IsBroadcasted && await ProductPromotionIsActive(productIdList)) return BadRequest();

            var newPromotion = new Promotion
            {
                Name = promotion.Name,
                Description = promotion.Description,
                IsBroadcasted = promotion.IsBroadcasted
            };

            _context.Promotion.Add(newPromotion);
            var promotionItems = new List<PromotionItem>();
            var products = new List<Product>();
            foreach (var promotionItemDto in promotion.PromotionItems)
            {
                var product = new Product { Id = promotionItemDto.ProductId };
                products.Add(product);
                var promotionItem = new PromotionItem
                {
                    Promotion = newPromotion,
                    InPromotionProduct = product,
                    NewPrice = promotionItemDto.NewPrice
                };

                promotionItems.Add(promotionItem);
            }

            _context.AttachRange(products);
            _context.PromotionItem.AddRange(promotionItems);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetPromotion", new { id = newPromotion.Id }, newPromotion);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<Promotion>> PutPromotion(int id, PromotionFormDto promotion)
        {
            int.TryParse(User.Claims.FirstOrDefault(claim => claim.Type == "Id")?.Value, out var tokenId);

            if (tokenId != 1) return Unauthorized();

            var productIdList = promotion.PromotionItems.Select(p => p.ProductId);

            if (promotion.IsBroadcasted && await ProductPromotionIsActive(productIdList, id)) return BadRequest();

            var newPromotion = new Promotion
            {
                Id = id,
                Name = promotion.Name,
                Description = promotion.Description,
                IsBroadcasted = promotion.IsBroadcasted
            };

            _context.Entry(newPromotion).State = EntityState.Modified;
            _context.PromotionItem.RemoveRange(_context.PromotionItem.Where(x => x.Promotion.Id == id));

            var promotionItems = new List<PromotionItem>();
            var products = new List<Product>();
            foreach (var promotionItemDto in promotion.PromotionItems)
            {
                var product = new Product { Id = promotionItemDto.ProductId };
                products.Add(product);
                var promotionItem = new PromotionItem
                {
                    Promotion = newPromotion,
                    InPromotionProduct = product,
                    NewPrice = promotionItemDto.NewPrice
                };

                promotionItems.Add(promotionItem);
            }

            _context.AttachRange(products);

            newPromotion.PromotionItems = promotionItems;


            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<Promotion>> ArchivePromotion(int id)
        {
            int.TryParse(User.Claims.FirstOrDefault(claim => claim.Type == "Id")?.Value, out var tokenId);

            if (tokenId != 1) return Unauthorized();

            var promotion = new Promotion { Id = id, IsBroadcasted = false, Archived = true };
            _context.Entry(promotion).State = EntityState.Modified;

            await _context.SaveChangesAsync();

            return promotion;
        }

        private Task<bool> ProductPromotionIsActive(IEnumerable<string> productId)
        {
            return _context.PromotionItem
                .AnyAsync(pi =>
                    pi.Promotion.IsBroadcasted && !pi.Promotion.Archived &&
                    productId.Contains(pi.InPromotionProduct.Id));
        }

        private Task<bool> ProductPromotionIsActive(IEnumerable<string> productId, int ignorePromotionId)
        {
            return _context.PromotionItem
                .AnyAsync(pi =>
                    pi.Promotion.IsBroadcasted &&
                    !pi.Promotion.Archived &&
                    pi.Promotion.Id != ignorePromotionId &&
                    productId.Contains(pi.InPromotionProduct.Id));
        }
    }
}
