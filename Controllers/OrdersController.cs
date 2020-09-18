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
        public async Task<ActionResult<Order>> GetOrder(int id)
        {
            var order = await _context.Order.FindAsync(id);

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

            // add product
            var orderProducts = new List<OrderProduct>();
            var products = new List<Product>();
            var productsId = new List<int>();
            foreach (var orderProductDto in order.Products)
            {
                var product = new Product { Id = orderProductDto.ProductId };
                products.Add(product);
                productsId.Add(orderProductDto.ProductId);
                var orderProduct = new OrderProduct
                {
                    Order = newOrder,
                    Product = product,
                    Amount = orderProductDto.Amount
                };
                orderProducts.Add(orderProduct);
            }

            

            // add promotion
            var promotions = await _context.Product
                .Where(p => productsId.Contains(p.Id))
                .Select(p =>  p.PromotionItems.FirstOrDefault(pi => pi.Promotion.IsBroadcasted).Promotion)
                .Distinct()
                .ToListAsync();

            var orderPromotion = promotions.Select(p => new OrderPromotion{ Order = newOrder, Promotion = p });

            _context.AttachRange(products);
            _context.AttachRange(promotions);
            _context.OrderProduct.AddRange(orderProducts);
            _context.OrderPromotion.AddRange(orderPromotion);

            await _context.SaveChangesAsync();

            return CreatedAtAction("GetOrder", new { id = newOrder.Id }, newOrder);
        }

    }
}
