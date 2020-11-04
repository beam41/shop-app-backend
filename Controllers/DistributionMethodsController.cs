using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShopAppBackend.Models;
using ShopAppBackend.Models.Context;

namespace ShopAppBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DistributionMethodsController : ControllerBase
    {
        private readonly DatabaseContext _context;

        public DistributionMethodsController(DatabaseContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<DistributionMethod>>> GetDistributionMethod()
        {
            return await _context.DistributionMethod.Where(d => !d.Archived).ToListAsync();
        }

        [HttpPut]
        public async Task<IActionResult> PutDistributionMethod(IEnumerable<DistributionMethod> distributionMethod)
        {
            int.TryParse(User.Claims.FirstOrDefault(claim => claim.Type == "Id")?.Value, out int tokenId);

            if (tokenId != 1)
            {
                return Unauthorized();
            }

            (await _context.DistributionMethod.Where(d => !d.Archived)
                .ToListAsync())
                .ForEach(d => d.Archived = true);

            _context.DistributionMethod.AddRange(distributionMethod);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
