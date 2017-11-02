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
    [Route("api/Competencies")]
    public class CompetenciesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CompetenciesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/Competencies
        [HttpGet]
        public IEnumerable<Competency> GetCompetencies()
        {
            return _context.Competencies;
        }

        // GET: api/Competencies/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetCompetency([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var competency = await _context.Competencies.SingleOrDefaultAsync(m => m.CompetencyId == id);

            if (competency == null)
            {
                return NotFound();
            }

            return Ok(competency);
        }

        // PUT: api/Competencies/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCompetency([FromRoute] int id, [FromBody] Competency competency)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != competency.CompetencyId)
            {
                return BadRequest();
            }

            _context.Entry(competency).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CompetencyExists(id))
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

        // POST: api/Competencies
        [HttpPost]
        public async Task<IActionResult> PostCompetency([FromBody] Competency competency)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.Competencies.Add(competency);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetCompetency", new { id = competency.CompetencyId }, competency);
        }

        // DELETE: api/Competencies/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCompetency([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var competency = await _context.Competencies.SingleOrDefaultAsync(m => m.CompetencyId == id);
            if (competency == null)
            {
                return NotFound();
            }

            _context.Competencies.Remove(competency);
            await _context.SaveChangesAsync();

            return Ok(competency);
        }

        private bool CompetencyExists(int id)
        {
            return _context.Competencies.Any(e => e.CompetencyId == id);
        }
    }
}