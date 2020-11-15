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
    public class BuildOrdersController : ControllerBase
    {
        private readonly DatabaseContext _context;

        public BuildOrdersController(DatabaseContext context)
        {
            _context = context;
        }

        // GET: api/BuildOrders
        [HttpGet]
        public async Task<ActionResult<IEnumerable<BuildOrder>>> GetBuildOrder()
        {
            return await _context.BuildOrder.ToListAsync();
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

        // PUT: api/BuildOrders/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPut("{id}")]
        public async Task<IActionResult> PutBuildOrder(int id, BuildOrder buildOrder)
        {
            if (id != buildOrder.Id)
            {
                return BadRequest();
            }

            _context.Entry(buildOrder).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!BuildOrderExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/BuildOrders
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPost]
        public async Task<ActionResult<BuildOrder>> PostBuildOrder(BuildOrder buildOrder)
        {
            _context.BuildOrder.Add(buildOrder);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetBuildOrder", new { id = buildOrder.Id }, buildOrder);
        }

        // DELETE: api/BuildOrders/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<BuildOrder>> DeleteBuildOrder(int id)
        {
            var buildOrder = await _context.BuildOrder.FindAsync(id);
            if (buildOrder == null)
            {
                return NotFound();
            }

            _context.BuildOrder.Remove(buildOrder);
            await _context.SaveChangesAsync();

            return buildOrder;
        }

        private bool BuildOrderExists(int id)
        {
            return _context.BuildOrder.Any(e => e.Id == id);
        }
    }
}
