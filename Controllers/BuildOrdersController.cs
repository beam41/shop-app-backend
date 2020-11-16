using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShopAppBackend.Enums;
using ShopAppBackend.Models;
using ShopAppBackend.Models.Context;
using ShopAppBackend.Services;

namespace ShopAppBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BuildOrdersController : ControllerBase
    {
        private readonly DatabaseContext _context;

        private readonly ImageService _imageService;

        public BuildOrdersController(DatabaseContext context, ImageService imageService)
        {
            _context = context;
            _imageService = imageService;
        }

        // GET: api/BuildOrders
        [HttpGet]
        public async Task<ActionResult<IEnumerable<BuildOrder>>> GetBuildOrder()
        {
            return await _context.BuildOrder
                .Include(bo => bo.CreatedByUser)
                .Include(bo => bo.DescriptionImages)
                .Include(bo => bo.OrderStates)
                .Include(bo => bo.DistributionMethod)
                .ToListAsync();
        }

        // GET: api/BuildOrders/5
        [HttpGet("{id}")]
        public async Task<ActionResult<BuildOrder>> GetBuildOrder(int id)
        {
            var buildOrder = await _context.BuildOrder.FindAsync(id);

            if (buildOrder == null)
            {
                return NotFound();
            }

            return buildOrder;
        }

        // POST: api/BuildOrders
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPost]
        public async Task<ActionResult<BuildOrder>> PostBuildOrder([FromForm] BuildOrderCreateDTO buildOrder)
        {
            int.TryParse(User.Claims.FirstOrDefault(claim => claim.Type == "Id")?.Value, out int tokenId);

            var user = new User { Id = tokenId };
            _context.Attach(user);

            var newBuildOrder = new BuildOrder
            {
                CreatedByUser = user,
                OrderStates = new List<OrderState> { new OrderState { State = OrderStateEnum.Created } },
                FullName = buildOrder.FullName,
                PhoneNumber = buildOrder.PhoneNumber,
                OrderDescription = buildOrder.OrderDescription,
                DescriptionImages = new List<BuildOrderImage>()
            };

            var fileNameList = buildOrder.DescriptionImages.Select(async p =>
            {
                var fileName = await _imageService.Uploader(p);
                var pi = new BuildOrderImage { ImageFileName = fileName };
                newBuildOrder.DescriptionImages.Add(pi);
            }).ToArray();

            Task.WaitAll(fileNameList);

            _context.BuildOrder.Add(newBuildOrder);

            await _context.SaveChangesAsync();
            GC.Collect();

            return CreatedAtAction("GetBuildOrder", new { id = newBuildOrder.Id }, newBuildOrder);
        }

        [HttpPut("{id}/able-to-built")]
        public async Task<ActionResult> AbleToBuilt(int id, BuildOrderIsAbleToBuiltDTO data)
        {
            // verifying
            int.TryParse(User.Claims.FirstOrDefault(claim => claim.Type == "Id")?.Value, out int tokenId);

            if (tokenId != 1)
            {
                return Unauthorized();
            }

            var buildOrder = await _context.BuildOrder.Where(o =>
                o.Id == id &&
                o.OrderStates
                    .OrderByDescending(os => os.CreatedAt)
                    .First()
                    .State == OrderStateEnum.Created
            ).FirstOrDefaultAsync();

            if (buildOrder == null)
            {
                return NotFound();
            }

            // updating
            buildOrder.OrderStates = new List<OrderState>
            {
                new OrderState
                {
                    State = data.IsAbleToBuilt ? OrderStateEnum.IsAbleToBuilt : OrderStateEnum.IsUnableToBuilt,
                }
            };

            if (data.IsAbleToBuilt)
            {
                buildOrder.DepositPrice = data.DepositPrice;
                buildOrder.FullPrice = data.FullPrice;
            }
            else
            {
                buildOrder.CancelledReason = data.RejectedReason;
            }

            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpPut("{id}/add-proof-deposit")]
        public async Task<ActionResult> AddedProofOfPaymentDeposit(int id, [FromForm] OrderAddProofOfPaymentDTO data)
        {
            // verifying
            int.TryParse(User.Claims.FirstOrDefault(claim => claim.Type == "Id")?.Value, out int tokenId);

            var buildOrder = await _context.BuildOrder.Where(o =>
                o.Id == id &&
                o.CreatedByUser.Id == tokenId &&
                o.OrderStates
                    .OrderByDescending(os => os.CreatedAt)
                    .First()
                    .State == OrderStateEnum.IsAbleToBuilt
                ).FirstOrDefaultAsync();

            if (buildOrder == null)
            {
                return NotFound();
            }

            // updating
            var fileName = await _imageService.Uploader(data.Image, false);

            buildOrder.OrderStates = new List<OrderState>
            {
                new OrderState
                {
                    State = OrderStateEnum.AddedProofOfPaymentDeposit,
                }
            };

            buildOrder.ProofOfPaymentDepositImage = fileName;

            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpPut("{id}/approve-proof-deposit")]
        public async Task<ActionResult> ApprovedProofOfPaymentDeposit(int id, BuildOrderApprovedProofOfPaymentDeposit data)
        {
            // verifying
            int.TryParse(User.Claims.FirstOrDefault(claim => claim.Type == "Id")?.Value, out int tokenId);

            if (tokenId != 1)
            {
                return Unauthorized();
            }

            var buildOrder = await _context.BuildOrder.Where(o =>
                o.Id == id &&
                o.OrderStates
                    .OrderByDescending(os => os.CreatedAt)
                    .First()
                    .State == OrderStateEnum.AddedProofOfPaymentDeposit
            ).FirstOrDefaultAsync();

            if (buildOrder == null)
            {
                return NotFound();
            }

            // updating
            buildOrder.OrderStates = new List<OrderState>
            {
                new OrderState
                {
                    State = OrderStateEnum.ApprovedProofOfPaymentDeposit,
                }
            };

            buildOrder.ExpectedCompleteDate = data.ExpectedCompleteDate;

            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpPut("{id}/built-complete")]
        public async Task<ActionResult> BuiltComplete(int id)
        {
            // verifying
            int.TryParse(User.Claims.FirstOrDefault(claim => claim.Type == "Id")?.Value, out int tokenId);

            if (tokenId != 1)
            {
                return Unauthorized();
            }

            var buildOrder = await _context.BuildOrder.Where(o =>
                o.Id == id &&
                o.OrderStates
                    .OrderByDescending(os => os.CreatedAt)
                    .First()
                    .State == OrderStateEnum.ApprovedProofOfPaymentDeposit
            ).FirstOrDefaultAsync();

            if (buildOrder == null)
            {
                return NotFound();
            }

            // updating
            buildOrder.OrderStates = new List<OrderState>
            {
                new OrderState
                {
                    State = OrderStateEnum.BuiltComplete,
                }
            };

            await _context.SaveChangesAsync();
            return NoContent();
        }


        [HttpPut("{id}/add-proof-full")]
        public async Task<ActionResult> AddProofOfPaymentFull(int id, [FromForm] BuildOrderAddProofOfPaymentFullDTO data)
        {
            // verifying
            int.TryParse(User.Claims.FirstOrDefault(claim => claim.Type == "Id")?.Value, out int tokenId);

            var order = await _context.BuildOrder.Where(o =>
                o.Id == id &&
                o.CreatedByUser.Id == tokenId &&
                o.OrderStates
                    .OrderByDescending(os => os.CreatedAt)
                    .First()
                    .State == OrderStateEnum.BuiltComplete
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

            var distributionMethod = new DistributionMethod { Id = data.DistributionMethodId };
            _context.Attach(distributionMethod);

            order.ProofOfPaymentFullImage = fileName;
            order.AddressFullName = data.FullName;
            order.Address = data.Address;
            order.Province = data.Province;
            order.District = data.District;
            order.SubDistrict = data.SubDistrict;
            order.PostalCode = data.PostalCode;
            order.AddressPhoneNumber = data.PhoneNumber;
            order.DistributionMethod = distributionMethod;

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

            var order = await _context.BuildOrder.Where(o =>
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

            var order = await _context.BuildOrder.Where(o =>
                o.Id == id &&
                o.OrderStates
                    .OrderByDescending(os => os.CreatedAt)
                    .First()
                    .State == OrderStateEnum.ApprovedProofOfPaymentFull
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

            var order = await _context.BuildOrder.Where(o =>
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

            var order = await _context.BuildOrder.Where(o => o.Id == id).FirstOrDefaultAsync();

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

            var order = await _context.BuildOrder.Where(o =>
                o.Id == id &&
                o.CreatedByUser.Id == tokenId &&
                (
                    o.OrderStates
                        .OrderByDescending(os => os.CreatedAt)
                        .First()
                        .State == OrderStateEnum.Created ||
                    o.OrderStates
                        .OrderByDescending(os => os.CreatedAt)
                        .First()
                        .State == OrderStateEnum.IsAbleToBuilt)
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
