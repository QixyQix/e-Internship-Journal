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
using System.Globalization;

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
        [Authorize(Roles = "SLO")]
        public async Task<IActionResult> GetBatches()
        {
            try
            {
                List<object> batchList = new List<object>();

                var userId = _userManager.GetUserId(User);

                var userBatches = _context.UserBatches
                    .Include(ub => ub.Batch)
                    .ThenInclude(batch => batch.Course)
                    .Include(ub => ub.User)
                    .Where(ub => ub.UserId.Equals(userId))
                    .ToList();


                foreach (var ub in userBatches)
                {
                    if (await _userManager.IsInRoleAsync(ub.User, "SLO"))
                    {
                        batchList.Add(new
                        {
                            BatchId = ub.Batch.BatchId,
                            BatchName = ub.Batch.BatchName,
                            CourseId = ub.Batch.Course.CourseId,
                            CourseName = ub.Batch.Course.CourseName,
                            Desciption = ub.Batch.Description,
                            StartDate = ub.Batch.StartDate,
                            EndDate = ub.Batch.EndDate
                        });
                    }

                }

                return new JsonResult(batchList);
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = ex.ToString() });
            }

        }

        [HttpGet("getBatchStudents/{id}")]
        [Authorize(Roles = "SLO")]
        public async Task<IActionResult> getBatchStudents(int id)
        {

            var users = _context.UserBatches.Where(ub => ub.BatchId == id)
                .Include(ub => ub.User)
                .Include(ub => ub.InternshipRecord)
                .ThenInclude(ir => ir.Project)
                .ThenInclude(p => p.Company)
                .Include(ub => ub.InternshipRecord)
                .ThenInclude(ir => ir.LiaisonOfficer)
                .ToList();

            List<object> studentObjects = new List<object>();

            foreach (var ub in users)
            {
                if (await _userManager.IsInRoleAsync(ub.User, "STUDENT"))
                {
                    string studentName = ub.User.FullName;
                    string studentUserId = ub.User.StudentId;
                    string studentEmail = ub.User.Email;
                    string studentPhonenumber = ub.User.PhoneNumber;
                    if (ub.InternshipRecord != null)
                    {
                        string LOName = ub.InternshipRecord.LiaisonOfficer.FullName;
                        string LOUserId = ub.InternshipRecord.LiaisonOfficer.Id;
                        string LOEmail = ub.InternshipRecord.LiaisonOfficer.Email;
                        int projectId = ub.InternshipRecord.ProjectId;
                        string projectName = ub.InternshipRecord.Project.ProjectName;
                        string companyName = ub.InternshipRecord.Project.Company.CompanyName;
                        studentObjects.Add(new
                        {
                            ub.UserBatchId,
                            StudentName = studentName,
                            StudentUserId = studentUserId,
                            StudentEmail = studentEmail,
                            StudentPhoneNumber = studentPhonenumber,
                            LOName = LOName,
                            LOUserId = LOUserId,
                            LOEmail = LOEmail,
                            ProjectId = projectId,
                            ProjectName = projectName,
                            CompanyName = companyName,
                            ub.Designation,
                            ub.Allowance
                        });
                    }
                    else
                    {
                        studentObjects.Add(new
                        {
                            ub.UserBatchId,
                            StudentName = studentName,
                            StudentUserId = studentUserId,
                            StudentEmail = studentEmail,
                            StudentPhoneNumber = studentPhonenumber,
                            LOName = "",
                            LOUserId = "",
                            LOEmail = "",
                            ProjectId = "",
                            ProjectName = "",
                            CompanyName = "",
                            Designation = "",
                            Allowance = ""
                            
                        });
                    }
                }
            }

            return new JsonResult(studentObjects);
        }

        [HttpGet("getAllBatches")]
        [Authorize(Roles = "ADMIN")]
        public async Task<JsonResult> getAllBatches()
        {
            List<object> userBatchList = new List<object>();

            var batches = _context.Batches.Include(eachBatchEntity => eachBatchEntity.Course).Include(eachUserBatchEntity => eachUserBatchEntity.UserBatches).AsNoTracking();

            foreach (var eachBatch in batches)
            {

                List<object> UserList = new List<object>();

                foreach (var eachUserBatch in eachBatch.UserBatches)
                {
                    if ((await _userManager.GetRolesAsync(await _userManager.FindByIdAsync(eachUserBatch.UserId))).Contains("SLO"))
                    {
                        var user = _context.Users.Where(eachUser => eachUser.Id.Equals(eachUserBatch.UserId)).Select(selection => selection.FullName).ToList<string>();
                        UserList.Add(user);
                    }
                }
                userBatchList.Add(new
                {
                    eachBatch.BatchId,
                    eachBatch.BatchName,
                    eachBatch.StartDate,
                    eachBatch.EndDate,
                    eachBatch.Course.CourseCode,
                    UserList,
                });

            }
            return new JsonResult(userBatchList);
        }
        [HttpGet("GetUserCourseInformation")]
        [Authorize(Roles = "ADMIN")]
        public async Task<IActionResult> GetUserCourseInformation(string returnUrl = null)
        {
            var usersList = new List<Object>();
            //var courseList = new List<Object>();
            foreach (ApplicationUser eachUser in _userManager.Users)
            {
                //var roles = (await _userManager.GetRolesAsync(eachUser));
                if ((await _userManager.GetRolesAsync(eachUser)).Contains("SLO"))
                {
                    usersList.Add(new
                    {
                        Name = eachUser.FullName,
                        Email = eachUser.Email

                    });
                }
            }
            //var courses = _context.Courses
            //.AsNoTracking();

            //foreach (var eachCourse in courses)
            //{
            //    courseList.Add(new
            //    {
            //        CourseId = eachCourse.CourseId,
            //        CourseCode = eachCourse.CourseCode,
            //        // Course

            //    });
            //}
            var courseList = _context.Courses.Select(courseItem => new { CourseId = courseItem.CourseId, CourseCode = courseItem.CourseCode });

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

                    var response = new Object();
                    var batches = _context.Batches
                        .Where(eachBatchEntity => eachBatchEntity.BatchId == id)
                        .Include(eachCourseEntity => eachCourseEntity.Course)
                        .Include(eachUserBatchEntity => eachUserBatchEntity.UserBatches)
                        .AsNoTracking();

                    foreach (var eachBatch in batches)
                    {

                        List<object> UserList = new List<object>();

                        foreach (var eachUserBatch in eachBatch.UserBatches)
                        {
                            if ((await _userManager.GetRolesAsync(await _userManager.FindByIdAsync(eachUserBatch.UserId))).Contains("SLO"))
                            {
                                var user = _context.Users.Where(eachUser => eachUser.Id.Equals(eachUserBatch.UserId)).Select(selection => selection.Email).ToList<string>();
                                UserList.Add(user);
                            }
                        }
                        response = new
                        {
                            eachBatch.BatchId,
                            eachBatch.BatchName,
                            eachBatch.Description,
                            eachBatch.StartDate,
                            eachBatch.EndDate,
                            eachBatch.Course.CourseId,
                            eachBatch.Course.CourseCode,
                            UserList,
                        };

                    }
                    return new JsonResult(response);
                }
                catch (Exception exceptionObject)
                {
                    //Create a fail message anonymous object
                    //This anonymous object only has one Message property 
                    //which contains a simple string message
                    object httpFailRequestResultMessage =
                    new { Message = "Unable to obtain batch information." };
                    //Return a bad http response message to the client
                    return BadRequest(httpFailRequestResultMessage);
                }
            }
            else
            {
                object httpFailRequestResultMessage =
                new { Message = "Unable to obtain batch information." };
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
            object response = null;
            if (BatchExists(id))
            {
                try
                {
                    var batchNewInput = JsonConvert.DeserializeObject<dynamic>(value);
                    var foundUserBatch = _context.UserBatches.Where(eachUserBatch => eachUserBatch.BatchId == id).AsNoTracking();
                    //var foundOneBatch = _context.Batches.Find(id);
                    var foundOneBatch = _context.Batches.Where(batch => batch.BatchId == id).Include(userBatch => userBatch.UserBatches).SingleOrDefault();
                    foreach (var eachUserBatch in foundOneBatch.UserBatches)
                    {
                        if ((await _userManager.GetRolesAsync(await _userManager.FindByIdAsync(eachUserBatch.UserId))).Contains("SLO"))
                        {
                            _context.UserBatches.Remove(eachUserBatch);
                        }
                    }
                    await _context.SaveChangesAsync();

                    foundOneBatch.BatchName = batchNewInput.BatchName.Value;
                    foundOneBatch.Description = batchNewInput.BatchDescription.Value;
                    foundOneBatch.StartDate = DateTime.ParseExact(batchNewInput.StartDate.Value, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                    foundOneBatch.EndDate = DateTime.ParseExact(batchNewInput.EndDate.Value, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                    foundOneBatch.CourseId = Convert.ToInt32(batchNewInput.CourseAssigned.Value.ToString());

                    var userList = batchNewInput.SLOAssigned;
                    foreach (var eachSLO in userList)
                    {
                        UserBatch batchUser = new UserBatch
                        {
                            BatchId = foundOneBatch.BatchId,
                            UserId = (await _userManager.FindByEmailAsync(eachSLO.Value)).Id
                        };
                        foundOneBatch.UserBatches.Add(batchUser);
                    }
                    await _context.SaveChangesAsync();
                    response = new { Status = "success", Message = "Saved new user record." };
                }
                catch (Exception ex)
                {
                    object httpFailRequestResultMessage = new { Message = ex };
                    //Return a bad http response message to the client
                    return BadRequest(httpFailRequestResultMessage);

                }

            }
            else
            {
                object httpFailRequestResultMessage =
                new { Message = "Invalid Batch ID" };
                //Return a bad http response message to the client
                return BadRequest(httpFailRequestResultMessage);

            }
            return new JsonResult(response);
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
                Message = "Saved Batches into database"
            };

            OkObjectResult httpOkResult =
            new OkObjectResult(successRequestResultMessage);
            return httpOkResult;
        }

        // DELETE: api/Batches/5
       

        private bool BatchExists(int id)
        {
            return _context.Batches.Any(e => e.BatchId == id);
        }
    }
}