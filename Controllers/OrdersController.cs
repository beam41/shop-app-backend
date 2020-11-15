using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using ShopAppBackend.Enums;
using ShopAppBackend.Models;
using ShopAppBackend.Models.Context;
using ShopAppBackend.Services;
using ShopAppBackend.Util;

namespace ShopAppBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class OrdersController : ControllerBase
    {
        private readonly DatabaseContext _context;

        private readonly ImageService _imageService;

        public OrdersController(DatabaseContext context, ImageService imageService)
        {
            _context = context;
            _imageService = imageService;
        }

        // GET: api/Orders
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Order>>> GetOrder()
        {
            return await _context.Order.ToListAsync();
        }

        [HttpGet("list/admin/{state}")]
        public async Task<ActionResult<IEnumerable<OrderListAdminDTO>>> GetOrderList(string state)
        {
            int.TryParse(User.Claims.FirstOrDefault(claim => claim.Type == "Id")?.Value, out int tokenId);

            if (tokenId != 1)
            {
                return Unauthorized();
            }

            var stateString = CaseChanger.UnderscoreToPascal(state);

            if (!Enum.TryParse(stateString, out OrderStateEnum stateEnum)) return BadRequest();

            return await _context.Order
                .Where(o => o
                    .OrderStates
                    .OrderByDescending(os => os.CreatedAt)
                    .First()
                    .State == stateEnum
                )
                .Select(o => new OrderListAdminDTO
                {
                    Id = o.Id,
                    CreatedByUser = new User
                    {
                        Id = o.CreatedByUser.Id, 
                        Username = o.CreatedByUser.Username, 
                        FullName = o.CreatedByUser.FullName
                    },
                    ProductsCount = o.OrderProducts.Count,
                    AmountCount = o.OrderProducts.Sum(op => op.Amount),
                    PurchaseMethod = o.PurchaseMethod,
                    TotalPrice = o.OrderProducts.Sum(op => (op.SavedNewPrice ?? op.SavedPrice) * op.Amount) + o.DistributionMethod.Price,
                    CreatedDate = o.OrderStates.Min(os => os.CreatedAt),
                    UpdatedDate = o.OrderStates.Max(os => os.CreatedAt),
                })
                .ToListAsync();
        }

        [HttpGet("list")]
        public async Task<ActionResult<IEnumerable<OrderListDTO>>> GetOrderList()
        {
            int.TryParse(User.Claims.FirstOrDefault(claim => claim.Type == "Id")?.Value, out int tokenId);

            return await _context.Order
                .Where(o => o.CreatedByUser.Id == tokenId)
                .Select(o => new OrderListDTO
                {
                    Id = o.Id,
                    ProductsName = (ICollection<string>) o.OrderProducts.Select(op => op.Product.Name),
                    AmountCount = o.OrderProducts.Sum(op => op.Amount),
                    TotalPrice = o.OrderProducts.Sum(op => (op.SavedNewPrice ?? op.SavedPrice) * op.Amount) + o.DistributionMethod.Price,
                    UpdatedDate = o.OrderStates.Max(os => os.CreatedAt),
                    State = o.OrderStates.OrderByDescending(os => os.CreatedAt).First().State,
                    PurchaseMethod = o.PurchaseMethod
                })
                .OrderByDescending(os => os.UpdatedDate)
                .ToListAsync();
        }

        // GET: api/Orders/5
        [HttpGet("{id}")]
        public async Task<ActionResult<OrderViewDTO>> GetOrder(int id)
        {
            int.TryParse(User.Claims.FirstOrDefault(claim => claim.Type == "Id")?.Value, out int tokenId);

            var order = await _context.Order
                .Where(o => tokenId == 1 || o.CreatedByUser.Id == tokenId)
                .Select(o => new OrderViewDTO
                {
                    Id = o.Id,
                    PurchaseMethod = o.PurchaseMethod,
                    DistributionMethod = o.DistributionMethod,
                    OrderStates = (ICollection<OrderStateDTO>)o.OrderStates.Select(os => new OrderStateDTO
                    {
                        Id = os.Id,
                        CreatedAt = os.CreatedAt,
                        State = os.State,
                    }).OrderBy(os => os.CreatedAt),
                    Products = (ICollection<ProductOrderDetailDTO>)o.OrderProducts.Select(op => new ProductOrderDetailDTO
                    {
                        Id = op.Product.Id,
                        Name = op.Product.Name,
                        Price = op.SavedPrice,
                        NewPrice = op.SavedNewPrice,
                        Amount = op.Amount
                    }),
                    TrackingNumber = o.TrackingNumber,
                    FullName = o.FullName,
                    Address = o.Address,
                    Province = o.Province,
                    District = o.District,
                    SubDistrict = o.SubDistrict,
                    PostalCode = o.PostalCode,
                    PhoneNumber = o.PhoneNumber,
                    ProofOfPaymentFullImage = o.ProofOfPaymentFullImage.Length > 0 ? _imageService.GetImageUrl(o.ProofOfPaymentFullImage) : null,
                    ReceivedMessage = o.ReceivedMessage,
                    CancelledByAdmin = o.CancelledByAdmin,
                    CancelledReason = o.CancelledReason,
                    CreatedByUser = tokenId == 1 ? new User { Id = o.CreatedByUser.Id, Username = o.CreatedByUser.Username, FullName = o.CreatedByUser.FullName } : null
                })
                .FirstOrDefaultAsync(o => o.Id == id);

            if (order == null)
            {
                return NotFound();
            }

            return order;
        }

        [HttpPost("new")]
        public async Task<ActionResult<Order>> CreateOrder(OrderCreateDTO order)
        {
            int.TryParse(User.Claims.FirstOrDefault(claim => claim.Type == "Id")?.Value, out int tokenId);

            var user = new User { Id = tokenId };
            _context.Attach(user);

            var distributionMethod = new DistributionMethod {Id = order.DistributionMethodId};
            _context.Attach(distributionMethod);

            var newOrder = new Order
            {
                PurchaseMethod = order.PurchaseMethod,
                CreatedByUser = user,
                OrderStates = new List<OrderState>
                {
                    new OrderState { State = OrderStateEnum.Created }
                },
                DistributionMethod = distributionMethod,
                FullName = order.FullName,
                Address = order.Address,
                Province = order.Province,
                District = order.District,
                SubDistrict = order.SubDistrict,
                PostalCode = order.PostalCode,
                PhoneNumber = order.PhoneNumber
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

            _context.AttachRange(products);
            _context.OrderProduct.AddRange(orderProducts);

            await _context.SaveChangesAsync();

            return CreatedAtAction("GetOrder", new { id = newOrder.Id }, new { id = newOrder.Id });
        }

        [HttpPut("{id}/add-proof-full")]
        public async Task<ActionResult> AddProofOfPaymentFull(int id, [FromForm] OrderAddProofOfPaymentDTO data)
        {
            // verifying
            int.TryParse(User.Claims.FirstOrDefault(claim => claim.Type == "Id")?.Value, out int tokenId);

            var order = await _context.Order.Where(o => 
                o.Id == id &&
                o.CreatedByUser.Id == tokenId &&
                o.OrderStates
                    .OrderByDescending(os => os.CreatedAt)
                    .First()
                    .State == OrderStateEnum.Created
                ).FirstOrDefaultAsync();

            if (order == null)
            {
                return NotFound();
            }

            // updating
            var fileName = await _imageService.Uploader(data.Image, false);

            order.OrderStates = new List<OrderState>
            {
                new OrderState
                {
                    State = OrderStateEnum.AddedProofOfPaymentFull,
                }
            };

            order.ProofOfPaymentFullImage = fileName;

            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpPut("{id}/approve-proof-full")]
        public async Task<ActionResult> ApproveProofOfPaymentFull(int id)
        {
            // verifying
            int.TryParse(User.Claims.FirstOrDefault(claim => claim.Type == "Id")?.Value, out int tokenId);

            if (tokenId != 1)
            {
                return Unauthorized();
            }

            var order = await _context.Order.Where(o =>
                o.Id == id &&
                o.OrderStates
                    .OrderByDescending(os => os.CreatedAt)
                    .First()
                    .State == OrderStateEnum.AddedProofOfPaymentFull
            ).FirstOrDefaultAsync();

            if (order == null)
            {
                return NotFound();
            }

            // updating
            order.OrderStates = new List<OrderState>
            {
                new OrderState
                {
                    State = OrderStateEnum.ApprovedProofOfPaymentFull,
                }
            };

            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpPut("{id}/sent")]
        public async Task<ActionResult> Sent(int id, OrderSentDTO data)
        {
            // verifying
            int.TryParse(User.Claims.FirstOrDefault(claim => claim.Type == "Id")?.Value, out int tokenId);

            if (tokenId != 1)
            {
                return Unauthorized();
            }

            var order = await _context.Order.Where(o =>
                o.Id == id &&
                o.OrderStates
                    .OrderByDescending(os => os.CreatedAt)
                    .First()
                    .State == (o.PurchaseMethod == PurchaseMethodEnum.Bank 
                        ? OrderStateEnum.ApprovedProofOfPaymentFull 
                        : OrderStateEnum.Created)
            ).FirstOrDefaultAsync();

            if (order == null)
            {
                return NotFound();
            }

            // updating
            order.OrderStates = new List<OrderState>
            {
                new OrderState
                {
                    State = OrderStateEnum.Sent,
                }
            };

            order.TrackingNumber = data.TrackingNumber;

            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpPut("{id}/received")]
        public async Task<ActionResult> Received(int id, OrderReceivedDTO data)
        {
            // verifying
            int.TryParse(User.Claims.FirstOrDefault(claim => claim.Type == "Id")?.Value, out int tokenId);

            var order = await _context.Order.Where(o =>
                o.Id == id &&
                o.CreatedByUser.Id == tokenId &&
                o.OrderStates
                    .OrderByDescending(os => os.CreatedAt)
                    .First()
                    .State == OrderStateEnum.Sent
            ).FirstOrDefaultAsync();

            if (order == null)
            {
                return NotFound();
            }

            // updating
            order.OrderStates = new List<OrderState>
            {
                new OrderState
                {
                    State = OrderStateEnum.Received,
                }
            };

            order.ReceivedMessage = data.Message;


            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpPut("{id}/cancelled/admin")]
        public async Task<ActionResult> Cancelled(int id, OrderCancelledDTO data)
        {
            int.TryParse(User.Claims.FirstOrDefault(claim => claim.Type == "Id")?.Value, out int tokenId);

            if (tokenId != 1)
            {
                return Unauthorized();
            }

            var order = await _context.Order.Where(o => o.Id == id).FirstOrDefaultAsync();

            if (order == null)
            {
                return NotFound();
            }

            // updating
            order.OrderStates = new List<OrderState>
            {
                new OrderState
                {
                    State = OrderStateEnum.Cancelled,
                }
            };

            order.CancelledByAdmin = true;
            order.CancelledReason = data.Reason;

            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpPut("{id}/cancelled/user")]
        public async Task<ActionResult> Cancelled(int id)
        {
            // verifying
            int.TryParse(User.Claims.FirstOrDefault(claim => claim.Type == "Id")?.Value, out int tokenId);

            var order = await _context.Order.Where(o =>
                o.Id == id &&
                o.CreatedByUser.Id == tokenId &&
                o.OrderStates
                    .OrderByDescending(os => os.CreatedAt)
                    .First()
                    .State == OrderStateEnum.Created
            ).FirstOrDefaultAsync();

            if (order == null)
            {
                return NotFound();
            }

            // updating
            order.OrderStates = new List<OrderState>
            {
                new OrderState
                {
                    State = OrderStateEnum.Cancelled,
                }
            };

            order.CancelledByAdmin = false;

            await _context.SaveChangesAsync();
            return NoContent();
        }

    }
}
