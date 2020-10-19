using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShopAppBackend.Enums;
using ShopAppBackend.Models;
using ShopAppBackend.Models.Context;

namespace ShopAppBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class OrdersController : ControllerBase
    {
        private readonly DatabaseContext _context;

        public OrdersController(DatabaseContext context)
        {
            _context = context;
        }

        // GET: api/Orders
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Order>>> GetOrder()
        {
            return await _context.Order.ToListAsync();
        }

        // GET: api/Orders/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ViewOrderDTO>> GetOrder(int id)
        {
            var order = await _context.Order
                .Select(o => new ViewOrderDTO
                {
                    Id = o.Id,
                    PurchaseMethod = o.PurchaseMethod,
                    OrderStates = (ICollection<OrderStateDTO>)o.OrderStates.Select(os => new OrderStateDTO
                    {
                        Id = os.Id,
                        CreatedAt = os.CreatedAt,
                        State = os.State,
                        StateDataJson = os.StateDataJson,
                    }),
                    Products = (ICollection<ProductDetailDTO>)o.OrderProducts.Select(op => new ProductDetailDTO
                    {
                        Id = op.Product.Id,
                        Name = op.Product.Name,
                        Price = op.SavedPrice,
                        NewPrice = op.SavedNewPrice
                    })
                })
                .FirstOrDefaultAsync(o => o.Id == id);

            if (order == null)
            {
                return NotFound();
            }

            return order;
        }

        [HttpPost("new")]
        public async Task<ActionResult<Order>> CreateOrder(CreateOrderDTO order)
        {
            int.TryParse(User.Claims.FirstOrDefault(claim => claim.Type == "Id")?.Value, out int tokenId);

            var user = new User { Id = tokenId };
            _context.Attach(user);

            var newOrder = new Order
            {
                PurchaseMethod = order.PurchaseMethod,
                CreatedByUser = user,
                OrderStates = new List<OrderState>
                {
                    new OrderState { State = OrderStateEnum.Created, StateDataJson = order.AddressJson }
                }
            };

            _context.Order.Add(newOrder);

            
            var orderProductsId = order.Products.Select(p => p.ProductId);

            var products = await _context.Product
                .Where(p => orderProductsId.Contains(p.Id))
                .Include(p => p.PromotionItems)
                .ThenInclude(pi => pi.Promotion)
                .ToListAsync();

            // add product
            var orderProducts = order.Products.Select(op =>
            {
                var product = products.Find(p => p.Id == op.ProductId);
                return new OrderProduct
                {
                    Product = product,
                    Amount = op.Amount,
                    Order = newOrder,
                    SavedPrice = product.Price,
                    SavedNewPrice = product.PromotionItems.FirstOrDefault(pi => pi.Promotion.IsBroadcasted && !pi.Promotion.Archived)?.NewPrice
                };
            });

            // add promotion
            var promotions = products
                .Select(p =>  p.PromotionItems.FirstOrDefault(pi => pi.Promotion.IsBroadcasted && !pi.Promotion.Archived)?.Promotion)
                .Where(p => p != null)
                .Distinct()
                .ToList();

            var orderPromotion = promotions.Select(p => new OrderPromotion{ Order = newOrder, Promotion = p });

            _context.AttachRange(products);
            _context.AttachRange(promotions);
            _context.OrderProduct.AddRange(orderProducts);
            _context.OrderPromotion.AddRange(orderPromotion);

            await _context.SaveChangesAsync();

            return CreatedAtAction("GetOrder", new { id = newOrder.Id }, new { id = newOrder.Id });
        }

    }
}
