﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShopAppBackend.Enums;
using ShopAppBackend.Models;
using ShopAppBackend.Models.Context;
using ShopAppBackend.Models.DTOs;
using ShopAppBackend.Services;
using ShopAppBackend.Utils;

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

        [HttpGet("list/admin/{state}")]
        public async Task<ActionResult<IEnumerable<OrderListAdminDto>>> GetOrderList(string state)
        {
            int.TryParse(User.Claims.FirstOrDefault(claim => claim.Type == "Id")?.Value, out var tokenId);

            if (tokenId != 1) return Unauthorized();

            var stateString = CaseChanger.UnderscoreToPascal(state);

            if (!Enum.TryParse(stateString, out OrderStateEnum stateEnum)) return BadRequest();

            return await _context.Order
                .Where(o => o
                        .OrderStates
                        .OrderByDescending(os => os.CreatedDate)
                        .First()
                        .State == stateEnum
                )
                .Select(o => new OrderListAdminDto
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
                    TotalPrice = o.OrderProducts.Sum(op => (op.SavedNewPrice ?? op.SavedPrice) * op.Amount) +
                                 o.DistributionMethod.Price,
                    CreatedDate = o.OrderStates.Min(os => os.CreatedDate),
                    UpdatedDate = o.OrderStates.Max(os => os.CreatedDate)
                })
                .ToListAsync();
        }

        [HttpGet("list")]
        public async Task<ActionResult<IEnumerable<OrderListDto>>> GetOrderList()
        {
            int.TryParse(User.Claims.FirstOrDefault(claim => claim.Type == "Id")?.Value, out var tokenId);

            return await _context.Order
                .Where(o => o.CreatedByUser.Id == tokenId)
                .Select(o => new OrderListDto
                {
                    Id = o.Id,
                    ProductsName = (ICollection<string>) o.OrderProducts.Select(op => op.Product.Name),
                    TotalPrice = o.OrderProducts.Sum(op => (op.SavedNewPrice ?? op.SavedPrice) * op.Amount) +
                                 o.DistributionMethod.Price,
                    UpdatedDate = o.OrderStates.Max(os => os.CreatedDate),
                    State = o.OrderStates.OrderByDescending(os => os.CreatedDate).First().State,
                    PurchaseMethod = o.PurchaseMethod
                })
                .OrderByDescending(os => os.UpdatedDate)
                .ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<OrderViewDto>> GetOrder(string id)
        {
            int.TryParse(User.Claims.FirstOrDefault(claim => claim.Type == "Id")?.Value, out var tokenId);

            var order = await _context.Order
                .Where(o => tokenId == 1 || o.CreatedByUser.Id == tokenId)
                .Select(o => new OrderViewDto
                {
                    Id = o.Id,
                    PurchaseMethod = o.PurchaseMethod,
                    DistributionMethod = o.DistributionMethod,
                    OrderStates = (ICollection<OrderStateDto>) o.OrderStates.Select(os => new OrderStateDto
                    {
                        Id = os.Id,
                        CreatedDate = os.CreatedDate,
                        State = os.State
                    }).OrderBy(os => os.CreatedDate),
                    Products = (ICollection<ProductOrderDetailDto>) o.OrderProducts.Select(op =>
                        new ProductOrderDetailDto
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
                    ProofOfPaymentFullImage = o.ProofOfPaymentFullImage.Length > 0
                        ? _imageService.GetImageUrl(o.ProofOfPaymentFullImage)
                        : null,
                    ReceivedMessage = o.ReceivedMessage,
                    CancelledByAdmin = o.CancelledByAdmin,
                    CancelledReason = o.CancelledReason,
                    CreatedByUser = tokenId == 1
                        ? new User
                        {
                            Id = o.CreatedByUser.Id, Username = o.CreatedByUser.Username,
                            FullName = o.CreatedByUser.FullName
                        }
                        : null
                })
                .FirstOrDefaultAsync(o => o.Id == id);

            if (order == null) return NotFound();

            return order;
        }

        [HttpPost("new")]
        public async Task<ActionResult<Order>> CreateOrder(OrderCreateDto order)
        {
            int.TryParse(User.Claims.FirstOrDefault(claim => claim.Type == "Id")?.Value, out var tokenId);

            var user = new User { Id = tokenId };
            _context.Attach(user);

            var distributionMethod = new DistributionMethod { Id = order.DistributionMethodId };
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
                    SavedNewPrice = product.PromotionItems
                        .FirstOrDefault(pi => pi.Promotion.IsBroadcasted && !pi.Promotion.Archived)?.NewPrice
                };
            });

            _context.AttachRange(products);
            _context.OrderProduct.AddRange(orderProducts);

            await _context.SaveChangesAsync();

            return CreatedAtAction("GetOrder", new { id = newOrder.Id }, new { id = newOrder.Id });
        }

        [HttpPut("{id}/add-proof-full")]
        public async Task<ActionResult> AddProofOfPaymentFull(string id, [FromForm] OrderAddProofOfPaymentDto data)
        {
            // verifying
            int.TryParse(User.Claims.FirstOrDefault(claim => claim.Type == "Id")?.Value, out var tokenId);

            var order = await _context.Order.Where(o =>
                o.Id == id &&
                o.CreatedByUser.Id == tokenId &&
                o.OrderStates
                    .OrderByDescending(os => os.CreatedDate)
                    .First()
                    .State == OrderStateEnum.Created
            ).FirstOrDefaultAsync();

            if (order == null) return NotFound();

            // updating
            var fileName = await _imageService.Uploader(data.Image, false);

            order.OrderStates = new List<OrderState>
            {
                new OrderState
                {
                    State = OrderStateEnum.AddedProofOfPaymentFull
                }
            };

            order.ProofOfPaymentFullImage = fileName;

            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpPut("{id}/approve-proof-full")]
        public async Task<ActionResult> ApproveProofOfPaymentFull(string id)
        {
            // verifying
            int.TryParse(User.Claims.FirstOrDefault(claim => claim.Type == "Id")?.Value, out var tokenId);

            if (tokenId != 1) return Unauthorized();

            var order = await _context.Order.Where(o =>
                o.Id == id &&
                o.OrderStates
                    .OrderByDescending(os => os.CreatedDate)
                    .First()
                    .State == OrderStateEnum.AddedProofOfPaymentFull
            ).FirstOrDefaultAsync();

            if (order == null) return NotFound();

            // updating
            order.OrderStates = new List<OrderState>
            {
                new OrderState
                {
                    State = OrderStateEnum.ApprovedProofOfPaymentFull
                }
            };

            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpPut("{id}/sent")]
        public async Task<ActionResult> Sent(string id, OrderSentDto data)
        {
            // verifying
            int.TryParse(User.Claims.FirstOrDefault(claim => claim.Type == "Id")?.Value, out var tokenId);

            if (tokenId != 1) return Unauthorized();

            var order = await _context.Order.Where(o =>
                o.Id == id &&
                o.OrderStates
                    .OrderByDescending(os => os.CreatedDate)
                    .First()
                    .State == (o.PurchaseMethod == PurchaseMethodEnum.Bank
                    ? OrderStateEnum.ApprovedProofOfPaymentFull
                    : OrderStateEnum.Created)
            ).FirstOrDefaultAsync();

            if (order == null) return NotFound();

            // updating
            order.OrderStates = new List<OrderState>
            {
                new OrderState
                {
                    State = OrderStateEnum.Sent
                }
            };

            order.TrackingNumber = data.TrackingNumber;

            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpPut("{id}/received")]
        public async Task<ActionResult> Received(string id, OrderReceivedDto data)
        {
            // verifying
            int.TryParse(User.Claims.FirstOrDefault(claim => claim.Type == "Id")?.Value, out var tokenId);

            var order = await _context.Order.Where(o =>
                o.Id == id &&
                o.CreatedByUser.Id == tokenId &&
                o.OrderStates
                    .OrderByDescending(os => os.CreatedDate)
                    .First()
                    .State == OrderStateEnum.Sent
            ).FirstOrDefaultAsync();

            if (order == null) return NotFound();

            // updating
            order.OrderStates = new List<OrderState>
            {
                new OrderState
                {
                    State = OrderStateEnum.Received
                }
            };

            order.ReceivedMessage = data.Message;


            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpPut("{id}/cancelled/admin")]
        public async Task<ActionResult> Cancelled(string id, OrderCancelledDto data)
        {
            int.TryParse(User.Claims.FirstOrDefault(claim => claim.Type == "Id")?.Value, out var tokenId);

            if (tokenId != 1) return Unauthorized();

            var order = await _context.Order.Where(o => o.Id == id).FirstOrDefaultAsync();

            if (order == null) return NotFound();

            // updating
            order.OrderStates = new List<OrderState>
            {
                new OrderState
                {
                    State = OrderStateEnum.Cancelled
                }
            };

            order.CancelledByAdmin = true;
            order.CancelledReason = data.Reason;

            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpPut("{id}/cancelled/user")]
        public async Task<ActionResult> Cancelled(string id)
        {
            // verifying
            int.TryParse(User.Claims.FirstOrDefault(claim => claim.Type == "Id")?.Value, out var tokenId);

            var order = await _context.Order.Where(o =>
                o.Id == id &&
                o.CreatedByUser.Id == tokenId &&
                o.OrderStates
                    .OrderByDescending(os => os.CreatedDate)
                    .First()
                    .State == OrderStateEnum.Created
            ).FirstOrDefaultAsync();

            if (order == null) return NotFound();

            // updating
            order.OrderStates = new List<OrderState>
            {
                new OrderState
                {
                    State = OrderStateEnum.Cancelled
                }
            };

            order.CancelledByAdmin = false;

            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
