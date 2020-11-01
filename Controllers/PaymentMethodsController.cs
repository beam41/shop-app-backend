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
    public class PaymentMethodsController : ControllerBase
    {
        private readonly DatabaseContext _context;

        public PaymentMethodsController(DatabaseContext context)
        {
            _context = context;
        }

        // GET: api/PaymentMethods
        [HttpGet]
        public async Task<ActionResult<IEnumerable<PaymentMethod>>> GetPaymentMethod()
        {
            return await _context.PaymentMethod.ToListAsync();
        }

        [HttpPut]
        public async Task<ActionResult> UpdatePaymentMethod(IEnumerable<PaymentMethod> paymentMethod)
        {
            int.TryParse(User.Claims.FirstOrDefault(claim => claim.Type == "Id")?.Value, out int tokenId);

            if (tokenId != 1)
            {
                return Unauthorized();
            }

            await _context.Database.ExecuteSqlRawAsync("TRUNCATE TABLE dbo.PaymentMethod");
            _context.PaymentMethod.AddRange(paymentMethod);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetPaymentMethod", paymentMethod);
        }
    }
}
