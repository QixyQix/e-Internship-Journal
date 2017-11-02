using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using E_Internship_Journal.Data;
using E_Internship_Journal.Models;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Authorization;

namespace E_Internship_Journal.API
{
    [Produces("application/json")]
    [Route("api/Batches")]
    public class BatchesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public BatchesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/Batches
        [HttpGet]
        [AllowAnonymous]
        public IEnumerable<Batch> GetBatches()
        {
            return _context.Batches;
        }

       
        // PUT: api/Batches/5
        [HttpPut("UpdateOneBatch/{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> UpdateOneBatch(int id, [FromBody] string value)
        {
            
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            string customMessage = "";
            if (BatchExists(id))
            {
                var batchNewInput = JsonConvert.DeserializeObject<dynamic>(value);
                var foundOneBatch = _context.Batches.Find(id);

                foundOneBatch.BatchName = batchNewInput.BatchName;
                foundOneBatch.Description = batchNewInput.Description;
                foundOneBatch.StartDate = batchNewInput.StartDate;
                foundOneBatch.EndDate = batchNewInput.EndDate;
                foundOneBatch.CourseId = batchNewInput.CourseId.Value;

                _context.Batches.Update(foundOneBatch);
                await _context.SaveChangesAsync();
            }
            else
            {

            }
            

            return NoContent();
        }

        [HttpPost("SaveNewBatchInformation")]
        public async Task<IActionResult> SaveNewBatchInformation([FromBody] string value)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            string customMessage = "";
            var batchNewInput = JsonConvert.DeserializeObject<dynamic>(value);
            Batch newBatch = new Batch();
            try
            {
                newBatch.BatchName = batchNewInput.BatchName.Value;
                newBatch.Description = batchNewInput.Description.Value;
                newBatch.StartDate = batchNewInput.StartDate.Value;
                newBatch.EndDate = batchNewInput.EndDate.Value;
                newBatch.CourseId = batchNewInput.CourseId.Value;

                _context.Batches.Add(newBatch);
                await _context.SaveChangesAsync();

            }

            catch (Exception exceptionObject)
            {
                customMessage = "Unable to save to database";
                //return 
            }
            var successRequestResultMessage = new
            {
                Message = "Saved Course into database"
            };

            OkObjectResult httpOkResult =
new OkObjectResult(successRequestResultMessage);
            return httpOkResult;
        }

        // DELETE: api/Batches/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBatch([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var batch = await _context.Batches.SingleOrDefaultAsync(m => m.BatchId == id);
            if (batch == null)
            {
                return NotFound();
            }

            _context.Batches.Remove(batch);
            await _context.SaveChangesAsync();

            return Ok(batch);
        }

        private bool BatchExists(int id)
        {
            return _context.Batches.Any(e => e.BatchId == id);
        }
    }
}