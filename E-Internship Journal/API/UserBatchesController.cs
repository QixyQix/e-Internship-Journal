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
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using System.Text;

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
            //var courses = 
            foreach (var oneUserBatch in userBatches)
            {
                var CourseName = _context.Courses.Select(courseItem => new { CourseName = courseItem.CourseName, CourseId = courseItem.CourseId }).Where(t => t.CourseId == oneUserBatch.Batch.CourseId);
                //   List<int> categoryIdList = new List<int>();
                userBatchList.Add(new
                {
                    oneUserBatch.Batch.BatchId,
                    oneUserBatch.Batch.BatchName,
                    oneUserBatch.Batch.Description,
                    oneUserBatch.Batch.StartDate,
                    oneUserBatch.Batch.EndDate,
                    CourseName,
                    //_context.Courses.Select(roleItem => new { CourseName = roleItem.CourseName }) },
                    oneUserBatch.User.FullName,
                    oneUserBatch.User.Email
                });
            }

            return new JsonResult(userBatchList);
        }
        [HttpGet("GetStudentBatches")]
        [Authorize(Roles = "SLO")]
        public async Task<IActionResult> GetStudentBatches(int BatchId, string StudentId)
        {
            List<object> userBatchList = new List<object>();
            var studentId = (await _userManager.FindByEmailAsync(StudentId)).Id;
            var userBatches = _context.UserBatches
            .Include(eachUserBatchEntity => eachUserBatchEntity.User)
            .Include(eacbProjectEntity => eacbProjectEntity.Batch)
            .Include(ir => ir.InternshipRecord)
            .ThenInclude(p => p.Project)
            .Where(ub => ub.BatchId == BatchId)
            .Where(ub => ub.UserId.Equals(studentId))
            .SingleOrDefault();
            //var courses = 
            // var response = new object();
            if (userBatches.InternshipRecord != null)
            {
                var response = new
                {
                    userBatches.UserBatchId,
                    userBatches.User.StudentId,
                    userBatches.User.FullName,
                    userBatches.User.PhoneNumber,
                    userBatches.User.Email,
                    userBatches.InternshipRecord.Project.ProjectName,
                    userBatches.InternshipRecord.ProjectId,
                    SupervisorName = _context.Users.Where(input => input.Id.Equals(userBatches.InternshipRecord.LiaisonOfficerId)).Select(u => u.FullName).SingleOrDefault(),
                    SupervisorEmail = _context.Users.Where(input => input.Id.Equals(userBatches.InternshipRecord.LiaisonOfficerId)).Select(u => u.Email).SingleOrDefault(),

                    // SupervisorName = _context.Users.Where(input =>input.Id.Equals(userBatches.InternshipRecord.LiaisonOfficerId)
                    userBatches.Allowance,
                    userBatches.Designation
                };
                return new JsonResult(response);
            }
            else
            {
                var response = new
                {
                    userBatches.UserBatchId,
                    userBatches.User.StudentId,
                    userBatches.User.FullName,
                    userBatches.User.PhoneNumber,
                    userBatches.User.Email,
                    ProjectName = "",
                    ProjectId = "",
                    SupervisorName = "",
                    SupervisorEmail = "",

                    // SupervisorName = _context.Users.Where(input =>input.Id.Equals(userBatches.InternshipRecord.LiaisonOfficerId)
                    userBatches.Allowance,
                    userBatches.Designation
                };
                return new JsonResult(response);
            }

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

                foundOneUserBatch.BatchId = userBatchNewInput.BatchId.Value;
                foundOneUserBatch.UserBatchId = (await _userManager.FindByEmailAsync(userBatchNewInput.Email.Value)).Id;

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
                    BatchId = userBatchNewInput.BatchId.Value,
                    UserId = (await _userManager.FindByEmailAsync(userBatchNewInput.Email.Value)).Id
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
        [HttpDelete("DeleteStudents/bulk")]
        [Authorize(Roles = "SLO")]
        public async Task<IActionResult> DeleteStudents([FromQuery]string checkedStudent, int selectedBatch)
        {

            var listOfId = checkedStudent.Split(',').ToList();
            //   var project = "tr";
            //var project = await _context.Projects.SingleOrDefaultAsync(m => m.ProjectId == id);
            string alertType = "success";
            List<String> messageList = new List<String>();
            try
            {
                List<object> listofUserId = new List<object>();
                // var foundProjects = _context.Projects.Include(ir => ir.InternshipRecords).ToList();

                foreach (var oneId in listOfId)
                {
                    var userId = (await _userManager.FindByEmailAsync(oneId)).Id;
                    listofUserId.Add(userId);
                }

                var foundUserBatch = _context.UserBatches.Include(ir => ir.InternshipRecord).ToList();
                List<ApplicationUser> removedUserBatch = new List<ApplicationUser>();

                foreach (var oneUserBatch in foundUserBatch)
                {

                    if (listofUserId.Contains(oneUserBatch.UserId) && oneUserBatch.InternshipRecord == null && oneUserBatch.BatchId == selectedBatch)
                    {
                        removedUserBatch.Add(oneUserBatch.User);
                        _context.UserBatches.Remove(oneUserBatch);
                        // _context.Users.Remove(oneUserBatch.User);
                        await _context.SaveChangesAsync();


                    }
                    else if (listofUserId.Contains(oneUserBatch.UserId) && oneUserBatch.InternshipRecord != null && oneUserBatch.BatchId == selectedBatch)
                    {
                        alertType = "warning";
                        messageList.Add(oneUserBatch.User.FullName + " not removed");
                    }
                    //else if (listofUserId.Contains(oneUserBatch.UserId) && oneUserBatch.InternshipRecord == null && oneUserBatch.BatchId != selectedBatch)
                    //{
                    //    alertType = "warning";
                    //    messageList.Add(oneUserBatch.User.FullName + " account not removed.");
                    //}
                }
                var foundUserId = _context.UserBatches.Select(r => r.UserId).ToList();
                foreach (var oneUser in removedUserBatch)
                {
                    if (foundUserId.Contains(oneUser.Id))
                    {
                        alertType = "warning";
                        messageList.Add(oneUser.FullName + " account not removed.");
                    }
                    else
                    {
                        var registrationPin = _context.RegistrationPins.Where(r => r.UserId.Equals(oneUser.Id)).SingleOrDefault();
                        //var registrationPin = oneUser.RegistrationPins.SingleOrDefault();

                        if (registrationPin != null)
                        {
                            _context.RegistrationPins.Remove(registrationPin);
                        }


                        _context.Users.Remove(oneUser);
                        await _context.SaveChangesAsync();
                    }
                }

                //messageList.Add("Selected Records removed successfully!");
                var responseObject = new
                {
                    AlertType = alertType,
                    Messages = messageList
                };

                return new OkObjectResult(responseObject);

            }
            catch (Exception exceptionObject)
            {
                object httpFailRequestResultMessage = new { Message = "Unable to Process" };
                //Return a bad http response message to the client
                return BadRequest(httpFailRequestResultMessage);
            }


        }
        [HttpPut("SaveNewStudentRecord/{id}")]
        [Authorize(Roles = "SLO")]
        public async Task<IActionResult> SaveNewStudentRecord([FromBody] string value, int id)
        {

            string messageList = "";
            string alertType = "success";
            var userNewInput = JsonConvert.DeserializeObject<dynamic>(value);


            try
            {
                // var checkSupervisorExist = (ApplicationUser)await _userManager.FindByEmailAsync(projectNewInput.SupervisorEmail.Value);
                string userEmail = userNewInput.StudentEmail.Value;
                var user = _context.ApplicationUsers.SingleOrDefault(appuser => appuser.UserName.Equals(userEmail, StringComparison.OrdinalIgnoreCase));
                if (user != null)
                {
                    messageList = user.FullName + " is already enrolled in this batch.";
                    alertType = "warning";
                }
                else
                {
                    //Create user
                    var userStore = new UserStore<ApplicationUser>(_context);
                    var userManager = new UserManager<ApplicationUser>(userStore, null, null, null, null, null, null, null, null);

                    var newStudentUser = new ApplicationUser
                    {
                        UserName = userEmail,
                        Email = userEmail,
                        FullName = userNewInput.StudentName.Value,
                        PhoneNumber = userNewInput.StudentNumber.Value,
                        StudentId = userNewInput.StudentId.Value,
                        //Course = thisBatch.Course
                    };
                    PasswordHasher<ApplicationUser> ph = new PasswordHasher<ApplicationUser>();
                    newStudentUser.PasswordHash = ph.HashPassword(newStudentUser, generateRandomString(11));

                    await userManager.CreateAsync(newStudentUser);
                    await userManager.AddToRoleAsync(newStudentUser, "STUDENT");

                    //Create UserBatch objects
                    var newUserBatch = new UserBatch
                    {
                        BatchId = id,
                        User = newStudentUser,
                        Allowance = userNewInput.Allowance.Value,
                        Designation = userNewInput.Designation.Value
                    };
                    _context.UserBatches.Add(newUserBatch);
                    var repeatPinGeneration = true;
                    do
                    {
                        var registationPin = generateRandomString(50);
                        if (!_context.RegistrationPins.Any(rp => rp.RegistrationPinId.Equals(registationPin)))
                        {
                            //Create new registration pin
                            var newRegistrationPin = new RegistrationPin
                            {
                                User = newStudentUser,
                                RegistrationPinId = generateRandomString(50)
                            };
                            _context.RegistrationPins.Add(newRegistrationPin);
                            repeatPinGeneration = false;
                        }
                    } while (repeatPinGeneration);
                    await _context.SaveChangesAsync();

                    messageList = "Saved Project & Created Supervisor Account";
                    alertType = "success";

                }//End of else statement

                var responseObject = new
                {
                    AlertType = alertType,
                    Messages = messageList
                };

                return new OkObjectResult(responseObject);
            }
            catch (Exception exceptionObject)
            {
                return BadRequest(new
                {
                    exceptionObject.Message
                });
            }

        }//End of Http Put
        private bool UserBatchExists(int id)
        {
            return _context.UserBatches.Any(e => e.UserBatchId == id);
        }
        public string generateRandomString(int size)
        {
            //ACKNOWLEDGEMENT: http://azuliadesigns.com/generate-random-strings-characters/
            StringBuilder builder = new StringBuilder();
            Random random = new Random();
            char ch;
            for (int i = 1; i < size + 1; i++)
            {
                ch = Convert.ToChar(Convert.ToInt32(Math.Floor(26 * random.NextDouble() + 65)));
                builder.Append(ch);
            }

            return builder.ToString();
        }
    }
}