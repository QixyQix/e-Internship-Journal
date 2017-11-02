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
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;

namespace E_Internship_Journal.API
{
    [Produces("application/json")]
    [Route("api/UserBatches")]
    public class UserBatchesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        public UserBatchesController(ApplicationDbContext context, UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager)
        {
             _userManager = userManager;
            _signInManager = signInManager;
            _context = context;
        }

        // GET: api/UserBatches
        [HttpGet]
        [AllowAnonymous]
        public IActionResult GetUserBatches()
        {
              List<object> userBatchList = new List<object>();
            var userBatches = _context.UserBatches
            .Include(eachUserBatchEntity => eachUserBatchEntity.User)
            .Include(eacbProjectEntity => eacbProjectEntity.Batch)
            .AsNoTracking();
            foreach (var oneUserBatch in userBatches)
            {
                //   List<int> categoryIdList = new List<int>();
                userBatchList.Add(new
                {
                    oneUserBatch.Batch.BatchId,
                    oneUserBatch.Batch.BatchName,
                     oneUserBatch.Batch.CourseId,
                      oneUserBatch.Batch.Description,
                       oneUserBatch.Batch.StartDate,
                        oneUserBatch.Batch.EndDate,

                    oneUserBatch.User.FullName,
                     oneUserBatch.User.Email
                });
            }

            return new JsonResult(userBatchList);
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
        [HttpPut("UpdateOneUserBatch/{id}")]
        public async Task<IActionResult> UpdateOneUserBatch(int id, [FromBody] string value)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            string customMessage = "";
            if (UserBatchExists(id))
            {
                var userBatchNewInput = JsonConvert.DeserializeObject<dynamic>(value);
                var foundOneUserBatch = _context.UserBatches.Find(id);

                foundOneUserBatch.BatchId = userBatchNewInput.BatchId;
                foundOneUserBatch.UserBatchId = (await _userManager.FindByEmailAsync(userBatchNewInput.Email)).Id;

                _context.UserBatches.Update(foundOneUserBatch);
                await _context.SaveChangesAsync();
            }
            else
            {

            }

            return NoContent();
        }

        // POST: api/UserBatches
        [HttpPost]
        public async Task<IActionResult> PostUserBatch([FromBody] string value)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            string customMessage = "";
           
            try
            {

                var userBatchNewInput = JsonConvert.DeserializeObject<dynamic>(value);
                UserBatch newUserBatch = new UserBatch
                {
                    BatchId = userBatchNewInput.BatchId,
                    UserId = (await _userManager.FindByEmailAsync(userBatchNewInput.Email)).Id
                };
                // newProject.SupervisorId = _userManager.FindByEmailAsync(projectNewInput.SupervisorEmail);
                //var ttt = _userManager.GetUserId(_userManager.FindByEmailAsync(projectNewInput.SupervisorEmail));

                _context.UserBatches.Add(newUserBatch);
                await _context.SaveChangesAsync();

            }
            catch (Exception exceptionObject)
            {
                customMessage = "Unable to save to database";
                //return 
            }
            var successRequestResultMessage = new
            {
                Message = "Saved UserBatch into database"
            };

            OkObjectResult httpOkResult =
            new OkObjectResult(successRequestResultMessage);
            return httpOkResult;
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