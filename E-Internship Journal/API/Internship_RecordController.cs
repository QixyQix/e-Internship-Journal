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
    [Route("api/Internship_Record")]
    public class Internship_RecordController : Controller
    {
        private readonly ApplicationDbContext _context;

        public Internship_RecordController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/Internship_Record
        [HttpGet]
        public IEnumerable<Internship_Record> GetInternship_Records()
        {
            return _context.Internship_Records;
        }

        // GET: api/Internship_Record/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetInternship_Record([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var internship_Record = await _context.Internship_Records.SingleOrDefaultAsync(m => m.InternshipRecordId == id);

            if (internship_Record == null)
            {
                return NotFound();
            }

            return Ok(internship_Record);
        }

        // PUT: api/Internship_Record/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutInternship_Record([FromRoute] int id, [FromBody] Internship_Record internship_Record)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != internship_Record.InternshipRecordId)
            {
                return BadRequest();
            }

            _context.Entry(internship_Record).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!Internship_RecordExists(id))
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

        // POST: api/Internship_Record
        [HttpPost]
        public async Task<IActionResult> PostInternship_Record([FromBody] Internship_Record internship_Record)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.Internship_Records.Add(internship_Record);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetInternship_Record", new { id = internship_Record.InternshipRecordId }, internship_Record);
        }

        // DELETE: api/Internship_Record/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteInternship_Record([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var internship_Record = await _context.Internship_Records.SingleOrDefaultAsync(m => m.InternshipRecordId == id);
            if (internship_Record == null)
            {
                return NotFound();
            }

            _context.Internship_Records.Remove(internship_Record);
            await _context.SaveChangesAsync();

            return Ok(internship_Record);
        }

        private bool Internship_RecordExists(int id)
        {
            return _context.Internship_Records.Any(e => e.InternshipRecordId == id);
        }
    }
}