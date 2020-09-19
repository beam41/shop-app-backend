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

        // GET: api/Promotions
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Promotion>>> GetPromotion()
        {
            return await _context.Promotion.Include(p => p.PromotionItems).ToListAsync();
        }

        // GET: api/Promotions/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Promotion>> GetPromotion(int id)
        {
            var promotion = await _context.Promotion.FindAsync(id);

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
            var productIdList = promotion.PromotionItems.Select(p => p.ProductId);

            if (!await ProductPromotionIsActive(productIdList))
            {
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

            return BadRequest();
        }

        private Task<bool> ProductPromotionIsActive(IEnumerable<int> productId)
        {
            return _context.PromotionItem
                .AnyAsync(pi => pi.Promotion.IsBroadcasted && productId.Contains(pi.InPromotionProduct.Id));
        }

        private Task<bool> PromotionExists(int id)
        {
            return _context.Promotion.AnyAsync(e => e.Id == id);
        }
    }
}
