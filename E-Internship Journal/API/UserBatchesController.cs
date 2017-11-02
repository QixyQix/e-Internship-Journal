using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using E_Internship_Journal.Data;
using E_Internship_Journal.Models;

namespace E_Internship_Journal.API
{
    [Produces("application/json")]
    [Route("api/UserBatches")]
    public class UserBatchesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public UserBatchesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/UserBatches
        [HttpGet]
        public IEnumerable<UserBatch> GetUserBatches()
        {
            return _context.UserBatches;
        }

        // GET: api/UserBatches/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetUserBatch([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var userBatch = await _context.UserBatches.SingleOrDefaultAsync(m => m.UserBatchId == id);

            if (userBatch == null)
            {
                return NotFound();
            }

            return Ok(userBatch);
        }

        // PUT: api/UserBatches/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutUserBatch([FromRoute] int id, [FromBody] UserBatch userBatch)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != userBatch.UserBatchId)
            {
                return BadRequest();
            }

            _context.Entry(userBatch).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UserBatchExists(id))
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

        // POST: api/UserBatches
        [HttpPost]
        public async Task<IActionResult> PostUserBatch([FromBody] UserBatch userBatch)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.UserBatches.Add(userBatch);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetUserBatch", new { id = userBatch.UserBatchId }, userBatch);
        }

        // DELETE: api/UserBatches/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUserBatch([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var userBatch = await _context.UserBatches.SingleOrDefaultAsync(m => m.UserBatchId == id);
            if (userBatch == null)
            {
                return NotFound();
            }

            _context.UserBatches.Remove(userBatch);
            await _context.SaveChangesAsync();

            return Ok(userBatch);
        }

        private bool UserBatchExists(int id)
        {
            return _context.UserBatches.Any(e => e.UserBatchId == id);
        }
    }
}