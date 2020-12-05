using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
    public class BuildOrdersController : ControllerBase
    {
        private readonly DatabaseContext _context;

        private readonly ImageService _imageService;

        public BuildOrdersController(DatabaseContext context, ImageService imageService)
        {
            _context = context;
            _imageService = imageService;
        }

        [HttpGet("list/admin/{state}")]
        public async Task<ActionResult<IEnumerable<BuildOrderListAdminDto>>> GetOrderList(string state)
        {
            int.TryParse(User.Claims.FirstOrDefault(claim => claim.Type == "Id")?.Value, out var tokenId);

            if (tokenId != 1) return Unauthorized();

            var stateString = CaseChanger.UnderscoreToPascal(state);

            if (!Enum.TryParse(stateString, out OrderStateEnum stateEnum)) return BadRequest();

            return await _context.BuildOrder
                .Where(o => o
                        .OrderStates
                        .OrderByDescending(os => os.CreatedDate)
                        .First()
                        .State == stateEnum
                )
                .Select(o => new BuildOrderListAdminDto
                {
                    Id = o.Id,
                    CreatedByUser = new User
                    {
                        Id = o.CreatedByUser.Id,
                        Username = o.CreatedByUser.Username,
                        FullName = o.CreatedByUser.FullName
                    },
                    CreatedDate = o.OrderStates.Min(os => os.CreatedDate),
                    UpdatedDate = o.OrderStates.Max(os => os.CreatedDate),
                    ExpectedCompleteDate = o.ExpectedCompleteDate,
                    FullName = o.FullName,
                    OrderDescription = o.OrderDescription
                })
                .ToListAsync();
        }

        [HttpGet("list")]
        public async Task<ActionResult<IEnumerable<BuildOrderListDto>>> GetBuildOrderList()
        {
            int.TryParse(User.Claims.FirstOrDefault(claim => claim.Type == "Id")?.Value, out var tokenId);

            return await _context.BuildOrder
                .Where(o => o.CreatedByUser.Id == tokenId)
                .Select(o => new BuildOrderListDto
                {
                    Id = o.Id,
                    UpdatedDate = o.OrderStates.Max(os => os.CreatedDate),
                    State = o.OrderStates.OrderByDescending(os => os.CreatedDate).First().State,
                    OrderDescription = o.OrderDescription
                })
                .OrderByDescending(os => os.UpdatedDate)
                .ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<BuildOrderViewDto>> GetBuildOrder(string id)
        {
            int.TryParse(User.Claims.FirstOrDefault(claim => claim.Type == "Id")?.Value, out var tokenId);

            var buildOrder = await _context.BuildOrder
                .Where(o => tokenId == 1 || o.CreatedByUser.Id == tokenId)
                .Select(o => new BuildOrderViewDto
                {
                    Id = o.Id,
                    DistributionMethod = o.DistributionMethod,
                    OrderStates = (ICollection<OrderStateDto>) o.OrderStates.Select(os => new OrderStateDto
                    {
                        Id = os.Id,
                        CreatedDate = os.CreatedDate,
                        State = os.State
                    }).OrderBy(os => os.CreatedDate),
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
                    ProofOfPaymentDepositImage = o.ProofOfPaymentDepositImage.Length > 0
                        ? _imageService.GetImageUrl(o.ProofOfPaymentDepositImage)
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
                        : null,
                    DepositPrice = o.DepositPrice,
                    FullPrice = o.FullPrice,
                    ExpectedCompleteDate = o.ExpectedCompleteDate,
                    AddressFullName = o.AddressFullName,
                    AddressPhoneNumber = o.AddressPhoneNumber,
                    DescriptionImagesUrl = (ICollection<ImageUrlDto>) o.DescriptionImages.Select(im => new ImageUrlDto
                    {
                        Id = im.Id,
                        ImageUrl = _imageService.GetImageUrl(im.ImageFileName)
                    }),
                    OrderDescription = o.OrderDescription
                })
                .FirstOrDefaultAsync(o => o.Id == id);

            if (buildOrder == null) return NotFound();

            return buildOrder;
        }

        [HttpPost]
        public async Task<ActionResult<BuildOrder>> PostBuildOrder([FromForm] BuildOrderCreateDto buildOrder)
        {
            int.TryParse(User.Claims.FirstOrDefault(claim => claim.Type == "Id")?.Value, out var tokenId);

            var user = new User { Id = tokenId };
            _context.Attach(user);

            var newBuildOrder = new BuildOrder
            {
                CreatedByUser = user,
                OrderStates = new List<BuildOrderState> { new BuildOrderState { State = OrderStateEnum.Created } },
                FullName = buildOrder.FullName,
                PhoneNumber = buildOrder.PhoneNumber,
                OrderDescription = buildOrder.OrderDescription,
                DescriptionImages = new List<BuildOrderImage>()
            };

            if (buildOrder.DescriptionImages != null)
            {
                var fileNameList = buildOrder.DescriptionImages.Select(async p =>
                {
                    var fileName = await _imageService.Uploader(p);
                    var pi = new BuildOrderImage { ImageFileName = fileName };
                    newBuildOrder.DescriptionImages.Add(pi);
                }).ToArray();

                Task.WaitAll(fileNameList);
            }

            _context.BuildOrder.Add(newBuildOrder);

            await _context.SaveChangesAsync();
            GC.Collect();

            return CreatedAtAction("GetBuildOrder", new { id = newBuildOrder.Id }, newBuildOrder);
        }

        [HttpPut("{id}/is-able-to-built")]
        public async Task<ActionResult> IsAbleToBuilt(string id, BuildOrderIsAbleToBuiltDto data)
        {
            // verifying
            int.TryParse(User.Claims.FirstOrDefault(claim => claim.Type == "Id")?.Value, out var tokenId);

            if (tokenId != 1) return Unauthorized();

            var buildOrder = await _context.BuildOrder.Where(o =>
                o.Id == id &&
                o.OrderStates
                    .OrderByDescending(os => os.CreatedDate)
                    .First()
                    .State == OrderStateEnum.Created
            ).FirstOrDefaultAsync();

            if (buildOrder == null) return NotFound();

            // updating
            buildOrder.OrderStates = new List<BuildOrderState>
            {
                new BuildOrderState
                {
                    State = OrderStateEnum.IsAbleToBuilt
                }
            };

            buildOrder.DepositPrice = data.DepositPrice;
            buildOrder.FullPrice = data.FullPrice;

            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpPut("{id}/add-proof-deposit")]
        public async Task<ActionResult> AddedProofOfPaymentDeposit(string id, [FromForm] OrderAddProofOfPaymentDto data)
        {
            // verifying
            int.TryParse(User.Claims.FirstOrDefault(claim => claim.Type == "Id")?.Value, out var tokenId);

            var buildOrder = await _context.BuildOrder.Where(o =>
                o.Id == id &&
                o.CreatedByUser.Id == tokenId &&
                o.OrderStates
                    .OrderByDescending(os => os.CreatedDate)
                    .First()
                    .State == OrderStateEnum.IsAbleToBuilt
            ).FirstOrDefaultAsync();

            if (buildOrder == null) return NotFound();

            // updating
            var fileName = await _imageService.Uploader(data.Image, false);

            buildOrder.OrderStates = new List<BuildOrderState>
            {
                new BuildOrderState
                {
                    State = OrderStateEnum.AddedProofOfPaymentDeposit
                }
            };

            buildOrder.ProofOfPaymentDepositImage = fileName;

            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpPut("{id}/approve-proof-deposit")]
        public async Task<ActionResult> ApprovedProofOfPaymentDeposit(string id,
            BuildOrderApprovedProofOfPaymentDepositDto data)
        {
            // verifying
            int.TryParse(User.Claims.FirstOrDefault(claim => claim.Type == "Id")?.Value, out var tokenId);

            if (tokenId != 1) return Unauthorized();

            var buildOrder = await _context.BuildOrder.Where(o =>
                o.Id == id &&
                o.OrderStates
                    .OrderByDescending(os => os.CreatedDate)
                    .First()
                    .State == OrderStateEnum.AddedProofOfPaymentDeposit
            ).FirstOrDefaultAsync();

            if (buildOrder == null) return NotFound();

            // updating
            buildOrder.OrderStates = new List<BuildOrderState>
            {
                new BuildOrderState
                {
                    State = OrderStateEnum.ApprovedProofOfPaymentDeposit
                }
            };

            buildOrder.ExpectedCompleteDate = data.ExpectedCompleteDate;

            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpPut("{id}/built-complete")]
        public async Task<ActionResult> BuiltComplete(string id)
        {
            // verifying
            int.TryParse(User.Claims.FirstOrDefault(claim => claim.Type == "Id")?.Value, out var tokenId);

            if (tokenId != 1) return Unauthorized();

            var buildOrder = await _context.BuildOrder.Where(o =>
                o.Id == id &&
                o.OrderStates
                    .OrderByDescending(os => os.CreatedDate)
                    .First()
                    .State == OrderStateEnum.ApprovedProofOfPaymentDeposit
            ).FirstOrDefaultAsync();

            if (buildOrder == null) return NotFound();

            // updating
            buildOrder.OrderStates = new List<BuildOrderState>
            {
                new BuildOrderState
                {
                    State = OrderStateEnum.BuiltComplete
                }
            };

            await _context.SaveChangesAsync();
            return NoContent();
        }


        [HttpPut("{id}/add-proof-full")]
        public async Task<ActionResult> AddProofOfPaymentFull(string id,
            [FromForm] BuildOrderAddProofOfPaymentFullDto data)
        {
            // verifying
            int.TryParse(User.Claims.FirstOrDefault(claim => claim.Type == "Id")?.Value, out var tokenId);

            var order = await _context.BuildOrder.Where(o =>
                o.Id == id &&
                o.CreatedByUser.Id == tokenId &&
                o.OrderStates
                    .OrderByDescending(os => os.CreatedDate)
                    .First()
                    .State == OrderStateEnum.BuiltComplete
            ).FirstOrDefaultAsync();

            if (order == null) return NotFound();

            // updating
            var fileName = await _imageService.Uploader(data.Image, false);

            order.OrderStates = new List<BuildOrderState>
            {
                new BuildOrderState
                {
                    State = OrderStateEnum.AddedProofOfPaymentFull
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
        public async Task<ActionResult> ApproveProofOfPaymentFull(string id)
        {
            // verifying
            int.TryParse(User.Claims.FirstOrDefault(claim => claim.Type == "Id")?.Value, out var tokenId);

            if (tokenId != 1) return Unauthorized();

            var order = await _context.BuildOrder.Where(o =>
                o.Id == id &&
                o.OrderStates
                    .OrderByDescending(os => os.CreatedDate)
                    .First()
                    .State == OrderStateEnum.AddedProofOfPaymentFull
            ).FirstOrDefaultAsync();

            if (order == null) return NotFound();

            // updating
            order.OrderStates = new List<BuildOrderState>
            {
                new BuildOrderState
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

            var order = await _context.BuildOrder.Where(o =>
                o.Id == id &&
                o.OrderStates
                    .OrderByDescending(os => os.CreatedDate)
                    .First()
                    .State == OrderStateEnum.ApprovedProofOfPaymentFull
            ).FirstOrDefaultAsync();

            if (order == null) return NotFound();

            // updating
            order.OrderStates = new List<BuildOrderState>
            {
                new BuildOrderState
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

            var order = await _context.BuildOrder.Where(o =>
                o.Id == id &&
                o.CreatedByUser.Id == tokenId &&
                o.OrderStates
                    .OrderByDescending(os => os.CreatedDate)
                    .First()
                    .State == OrderStateEnum.Sent
            ).FirstOrDefaultAsync();

            if (order == null) return NotFound();

            // updating
            order.OrderStates = new List<BuildOrderState>
            {
                new BuildOrderState
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

            var order = await _context.BuildOrder.Include(o => o.OrderStates).Where(o => o.Id == id)
                .FirstOrDefaultAsync();

            if (order == null) return NotFound();

            var isUnableToBuilt = order.OrderStates
                .OrderByDescending(os => os.CreatedDate)
                .First()
                .State == OrderStateEnum.Created;

            // updating
            order.OrderStates.Add(new BuildOrderState
            { State = isUnableToBuilt ? OrderStateEnum.IsUnableToBuilt : OrderStateEnum.Cancelled });

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

            var order = await _context.BuildOrder.Where(o =>
                o.Id == id &&
                o.CreatedByUser.Id == tokenId &&
                (
                    o.OrderStates
                        .OrderByDescending(os => os.CreatedDate)
                        .First()
                        .State == OrderStateEnum.Created ||
                    o.OrderStates
                        .OrderByDescending(os => os.CreatedDate)
                        .First()
                        .State == OrderStateEnum.IsAbleToBuilt)
            ).FirstOrDefaultAsync();

            if (order == null) return NotFound();

            // updating
            order.OrderStates = new List<BuildOrderState>
            {
                new BuildOrderState
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
