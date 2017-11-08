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
using System.IO;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using System.Text;

namespace E_Internship_Journal.API
{
    [Produces("application/json")]
    [Route("api/Batches")]
    public class BatchesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        public BatchesController(ApplicationDbContext context, UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _context = context;
        }

        // GET: api/Batches
        [HttpGet]
        [AllowAnonymous]
        public JsonResult GetBatches()
        {
            List<object> batchList = new List<object>();
            var batches = _context.Batches
           .Include(eachBatchEntity => eachBatchEntity.Course).AsNoTracking();


            foreach (var onebatch in batches)
            {
                //   List<int> categoryIdList = new List<int>();
                batchList.Add(new
                {
                    onebatch.BatchId,
                    onebatch.BatchName,
                    onebatch.Description,
                    onebatch.StartDate,
                    onebatch.EndDate
                });
            }

            return new JsonResult(batchList);
        }
        [HttpGet("getBatches")]
        [Authorize(Roles = "ADMIN")]
        public async Task<JsonResult> getBatches()
        {
            List<object> userBatchList = new List<object>();
            var userBatches = _context.UserBatches
            .Include(eachUserBatchEntity => eachUserBatchEntity.User)
            .Include(eachUserBatchEntity => eachUserBatchEntity.Batch)
            .Include(eachUserBatchEntity => eachUserBatchEntity.Batch.Course)
            .AsNoTracking();

            foreach (ApplicationUser eachUser in _userManager.Users)
            {
                var roles = (await _userManager.GetRolesAsync(eachUser));
                if (roles.Contains("SLO"))
                {
                    userBatches = userBatches.Where(eachUserBatchEntity => eachUserBatchEntity.UserId == eachUser.Id);

                    foreach (var oneUserBatch in userBatches)
                    {
                        //var CourseName = _context.Courses.Select(courseItem => new { CourseName = courseItem.CourseName, CourseId = courseItem.CourseId }).Where(t => t.CourseId == oneUserBatch.Batch.CourseId);
                        //   List<int> categoryIdList = new List<int>();
                        userBatchList.Add(new
                        {
                            oneUserBatch.Batch.BatchId,
                            oneUserBatch.Batch.BatchName,
                            oneUserBatch.Batch.Description,
                            oneUserBatch.Batch.StartDate,
                            oneUserBatch.Batch.EndDate,
                            oneUserBatch.Batch.Course.CourseCode,
                            //CourseName,
                            //_context.Courses.Select(roleItem => new { CourseName = roleItem.CourseName }) },
                            oneUserBatch.User.FullName,
                            oneUserBatch.User.Email

                        });
                    }
                }
            }

            // var batches = _context.Batches
            //.Include(eachBatchEntity => eachBatchEntity.Course)
            //.Include(eachBatchEntity => eachBatchEntity.UserBatches)
            //.AsNoTracking();


            // foreach (var onebatch in batches)
            // {

            //     //   List<int> categoryIdList = new List<int>();
            //     batchList.Add(new
            //     {
            //         onebatch.BatchId,
            //         onebatch.BatchName,
            //         onebatch.Description,
            //         onebatch.StartDate,
            //         onebatch.EndDate
            //     });
            // }

            return new JsonResult(userBatchList);
        }
        [HttpGet("GetUserCourseInformation")]
        [Authorize(Roles = "ADMIN")]
        public async Task<IActionResult> GetUserCourseInformation(string returnUrl = null)
        {
            var usersList = new List<Object>();
            var courseList = new List<Object>();
            foreach (ApplicationUser eachUser in _userManager.Users)
            {
                var roles = (await _userManager.GetRolesAsync(eachUser));
                if (roles.Contains("SLO"))
                {
                    usersList.Add(new
                    {
                        Name = eachUser.FullName,
                        Email = eachUser.Email

                    });
                }
            }
            var courses = _context.Courses
            .AsNoTracking();

            foreach (var eachCourse in courses)
            {
                courseList.Add(new
                {
                    CourseId = eachCourse.CourseId,
                    CourseCode = eachCourse.CourseCode,
                    // Course

                });
            }

            return new JsonResult(new { UserList = usersList, CourseList = courseList });
        }

        // GET api/Brands/5
        [HttpGet("GetOneBatches/{id}")]
        public async Task<IActionResult> GetOneBatches(int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            if (BatchExists(id))
            {
                try
                {
                    //var foundCourses = _context.Batches
                    //.Where(eachCourse => eachCourse.CourseId == id)
                    //.Include(eachCourse => eachCourse.Course).Single();
                    //var response = new
                    //{
                    //    BatchId = foundCourses.BatchId,
                    //    CourseId = foundCourses.CourseId,
                    //    Description = foundCourses.Description,
                    //    StartDate = foundCourses.StartDate,
                    //    EndDate = foundCourses.EndDate,
                    //    CourseName = foundCourses.Course.CourseName,
                    //    CourseCode = foundCourses.Course.CourseCode
                    //};//end of creation of the response object
                    List<object> userBatchList = new List<object>();
                    var userBatches = _context.UserBatches
                        .Where(eachUserBatchEntity => eachUserBatchEntity.BatchId == id)
                    .Include(eachUserBatchEntity => eachUserBatchEntity.User)
                    .Include(eacbProjectEntity => eacbProjectEntity.Batch)
                    .Include(eacbProjectEntity => eacbProjectEntity.Batch.Course)
                    .AsNoTracking();
                    var response = new Object();
                    foreach (ApplicationUser eachUser in _userManager.Users)
                    {
                        var roles = (await _userManager.GetRolesAsync(eachUser));
                        if (roles.Contains("SLO"))
                        {
                            userBatches = userBatches.Where(eachUserBatchEntity => eachUserBatchEntity.UserId == eachUser.Id);

                            foreach (var oneUserBatch in userBatches)
                            {
                                //var CourseName = _context.Courses.Select(courseItem => new { CourseName = courseItem.CourseName, CourseId = courseItem.CourseId }).Where(t => t.CourseId == oneUserBatch.Batch.CourseId);
                                //   List<int> categoryIdList = new List<int>();
                                response = new
                                {
                                    oneUserBatch.Batch.BatchId,
                                    oneUserBatch.Batch.BatchName,
                                    oneUserBatch.Batch.Description,
                                    oneUserBatch.Batch.StartDate,
                                    oneUserBatch.Batch.EndDate,
                                    //CourseName,
                                    oneUserBatch.Batch.Course.CourseId,
                                    oneUserBatch.Batch.Course.CourseCode,
                                    //_context.Courses.Select(roleItem => new { CourseName = roleItem.CourseName }) },
                                    oneUserBatch.User.FullName,
                                    oneUserBatch.User.Email
                                };

                            }
                        }
                    }
                    return new JsonResult(response);
                }
                catch (Exception exceptionObject)
                {
                    //Create a fail message anonymous object
                    //This anonymous object only has one Message property 
                    //which contains a simple string message
                    object httpFailRequestResultMessage =
                    new { Message = "Unable to obtain brand information." };
                    //Return a bad http response message to the client
                    return BadRequest(httpFailRequestResultMessage);
                }
            }
            else
            {
                object httpFailRequestResultMessage =
                new { Message = "Unable to obtain brand information." };
                //Return a bad http response message to the client
                return BadRequest(httpFailRequestResultMessage);
            }

        }//End of Get(id) Web API method


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

                foundOneBatch.BatchName = batchNewInput.BatchName.Value;
                foundOneBatch.Description = batchNewInput.Description.Value;
                foundOneBatch.StartDate = batchNewInput.StartDate.Value;
                foundOneBatch.EndDate = batchNewInput.EndDate.Value;
                foundOneBatch.CourseId = batchNewInput.CourseId.Value;

                _context.Batches.Update(foundOneBatch);
                await _context.SaveChangesAsync();
            }
            else
            {
                object httpFailRequestResultMessage =
                new { Message = "Invalid Batch ID" };
                //Return a bad http response message to the client
                return BadRequest(httpFailRequestResultMessage);

            }
            return NoContent();
        }



        [HttpPost("SaveNewBatchInformation")]
        [Authorize(Roles = "ADMIN")]
        public async Task<IActionResult> SaveNewBatchInformation([FromBody] string value)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            string customMessage = "";
            var batchNewInput = JsonConvert.DeserializeObject<dynamic>(value);
            Batch newBatch = new Batch();
            UserBatch newUserBatch = new UserBatch();
            //var courseAssigned = batchNewInput.CourseAssigned;
            try
            {
                newBatch.BatchName = batchNewInput.BatchName.Value;
                newBatch.Description = batchNewInput.BatchDescription.Value;
                newBatch.StartDate = batchNewInput.StartDate.Value;
                newBatch.EndDate = batchNewInput.EndDate.Value;
                newBatch.CourseId = Convert.ToInt32(batchNewInput.CourseAssigned.Value.ToString());

                _context.Batches.Add(newBatch);
                await _context.SaveChangesAsync();

                newUserBatch.BatchId = newBatch.BatchId;
                newUserBatch.UserId = (await _userManager.FindByEmailAsync(batchNewInput.SLOAssigned.Value)).Id;

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