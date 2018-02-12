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
using Hangfire;
using E_Internship_Journal.Services;

namespace E_Internship_Journal.API
{
    [Produces("application/json")]
    [Route("api/UserBatches")]
    public class UserBatchesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IEmailSender _emailSender;
        public UserBatchesController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, IEmailSender emailSender,
            SignInManager<ApplicationUser> signInManager)
        {
            _userManager = userManager;
            _emailSender = emailSender;
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
        public IActionResult GetStudentBatches(int UserBatchId)
        {
            List<object> userBatchList = new List<object>();
            //var studentId = (await _userManager.FindByEmailAsync(StudentId)).Id;
            var userBatches = _context.UserBatches
            .Include(eachUserBatchEntity => eachUserBatchEntity.User)
            .Include(eacbProjectEntity => eacbProjectEntity.Batch)
            .Include(ir => ir.InternshipRecord)
            .ThenInclude(p => p.Project)
            .Where(ub => ub.UserBatchId == UserBatchId)
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
                    LiaisonOfficerName = _context.Users.Where(input => input.Id.Equals(userBatches.InternshipRecord.LiaisonOfficerId)).Select(u => u.FullName).SingleOrDefault(),
                    LiaisonOfficerEmail = _context.Users.Where(input => input.Id.Equals(userBatches.InternshipRecord.LiaisonOfficerId)).Select(u => u.Email).SingleOrDefault(),

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
                    LiaisonOfficerName = "",
                    LiaisonOfficerEmail = "",

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
        [HttpPut("SaveNewStudentRecord/")]
        [Authorize(Roles = "SLO")]
        public async Task<IActionResult> SaveNewStudentRecord([FromBody] string value)
        {

            string messageList = "";
            string alertType = "success";
            var userNewInput = JsonConvert.DeserializeObject<dynamic>(value);


            try
            {
                // var checkSupervisorExist = (ApplicationUser)await _userManager.FindByEmailAsync(projectNewInput.SupervisorEmail.Value);
                //if (!(_context.UserBatches.Any(ub => ub.UserId.Equals(user.Id) && ub.BatchId == thisBatch.BatchId)))
                //{

                //}

                //if ((_context.UserBatches.Any(ub => ub.UserBatchId == id)))
                if (userNewInput.UserBatchId != null)
                {
                    string userEmail = userNewInput.StudentEmail.Value;
                    int id = Int32.Parse(userNewInput.UserBatchId.Value);
                    var user = _context.ApplicationUsers.SingleOrDefault(appuser => appuser.UserName.Equals(userEmail, StringComparison.OrdinalIgnoreCase));
                    var userBatch = _context.UserBatches.Include(b => b.Batch).Include(ir => ir.InternshipRecord).Single(ub => ub.UserBatchId == id);

                    user.FullName = userNewInput.StudentName.Value;
                    user.PhoneNumber = userNewInput.StudentNumber.Value;
                    user.StudentId = userNewInput.StudentId.Value;
                    userBatch.Designation = userNewInput.Designation.Value;
                    userBatch.Allowance = userNewInput.Allowance.Value;


                    if (!user.Email.Equals(userNewInput.StudentEmail.Value))
                    {
                        var code = await _userManager.GenerateChangeEmailTokenAsync(user, userNewInput.StudentEmail.Value.ToString());
                        string codeHtmlVersion = System.Net.WebUtility.UrlEncode(code);
                    }
                    if (_context.ScheduleInternshipRecords.Any(ub => ub.UserBatchId == id) && userBatch.Batch.StartDate <= DateTime.Now && userBatch.InternshipRecord == null)
                    {
                        var ScheduleInternshipRecord = _context.ScheduleInternshipRecords.Where(ub => ub.UserBatchId == id).Single();
                        ScheduleInternshipRecord.ProjectId = Int32.Parse(userNewInput.ProjectId.Value);
                        var LOEmail = (string)userNewInput.LO.Value;
                        ScheduleInternshipRecord.LiaisonOfficerId = (await _userManager.FindByEmailAsync(LOEmail)).Id;
                    }
                    if (userBatch.InternshipRecord != null)
                    {
                        userBatch.InternshipRecord.ProjectId = Int32.Parse(userNewInput.ProjectId.Value);
                        var LOEmail = (string)userNewInput.LO.Value;
                        userBatch.InternshipRecord.LiaisonOfficerId = (await _userManager.FindByEmailAsync(LOEmail)).Id;

                    }
                    await _context.SaveChangesAsync();
                    messageList = "Updated Student Account";
                    alertType = "success";

                }
                else
                {
                    var studentCreatedBatch = new UserBatch();
                    var studentCreated = new RegistrationPin();

                    string userEmail = userNewInput.StudentEmail.Value;
                    var user = _context.ApplicationUsers.SingleOrDefault(appuser => appuser.UserName.Equals(userEmail, StringComparison.OrdinalIgnoreCase));
                    if (user != null)
                    {
                        messageList = user.FullName + " is already enrolled in this batch.";
                        alertType = "warning";
                    }
                    else
                    {
                        int id = Int32.Parse(userNewInput.BatchId.Value);
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
                        // newStudentUser.PasswordHash = ph.HashPassword(newStudentUser,"P@ssw0rd");
                        await userManager.CreateAsync(newStudentUser);
                        await userManager.AddToRoleAsync(newStudentUser, "STUDENT");

                        //Create UserBatch objects
                        var newUserBatch = new UserBatch
                        {
                            Batch = _context.Batches.Where(batch => batch.BatchId == id).Single(),
                            User = newStudentUser,
                            Allowance = userNewInput.Allowance.Value,
                            Designation = userNewInput.Designation.Value
                        };
                        studentCreatedBatch = newUserBatch;

                        _context.UserBatches.Add(newUserBatch);
                        string LoEmail = userNewInput.LO.Value;
                        var LoId = (await _userManager.FindByEmailAsync(LoEmail)).Id;
                        var newInternshipRecord = new Internship_Record
                        {
                            ProjectId = Int32.Parse(userNewInput.ProjectId.Value),
                            LiaisonOfficerId = LoId,
                            UserBatch = newUserBatch
                        };
                        _context.Internship_Records.Add(newInternshipRecord);
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
                                studentCreated = newRegistrationPin;
                            }
                        } while (repeatPinGeneration);
                        await _context.SaveChangesAsync();


                        //var LOEmail = (string)userNewInput.LO.Value;
                        //var startDate = _context.Batches.Where(b => b.BatchId == id).Select(b => b.StartDate).Single();

                        //var scheduleCreationRecord = new ScheduleInternshipRecord
                        //{
                        //    ProjectId = Int32.Parse(userNewInput.ProjectId.Value),
                        //    LiaisonOfficerId = (await _userManager.FindByEmailAsync(LOEmail)).Id,
                        //    UserBatchId = newUserBatch.UserBatchId

                        //};

                        //_context.ScheduleInternshipRecords.Add(scheduleCreationRecord);
                        //await _context.SaveChangesAsync();
                        //BackgroundJob.Schedule(() => CreateInternshipJournalAsync(scheduleCreationRecord.ScheduleIntershipRecordId), startDate);

                        //var studentEmail = studentCreated.User.Email;
                        //var studentName = studentCreated.User.FullName;
                        var studentEmail = studentCreatedBatch.User.Email;
                        var studentName = studentCreatedBatch.User.FullName;
                        var batchName = studentCreatedBatch.Batch.BatchName;
                        var startDate = studentCreatedBatch.Batch.StartDate.ToString("dd MMMM yyyy");
                        var endDate = studentCreatedBatch.Batch.EndDate.ToString("dd MMMM yyyy");

                        //var code = await _userManager.GenerateEmailConfirmationTokenAsync(studentCreated.User);
                        //string codeHtmlVersion = System.Net.WebUtility.UrlEncode(code);
                        var absoluteUri = string.Concat(
      Request.Scheme,
      "://",
      Request.Host.ToUriComponent(),
      Request.PathBase.ToUriComponent());
                        await _emailSender.SendChangeEmailAsync(true, studentEmail, "Your account has been created and enrolled!",
                            "Hi, " + studentName, "Your student account has been created and enrolled into Batch " + batchName + "." +
                            " The Semester will start from " + startDate + " and end on " + endDate + ". Kindly proceed to activate your account before your internship starts.",
                            absoluteUri + "/Account/SetPassword?registrationPin=" + studentCreated.RegistrationPinId, "Activate Account");

                        messageList = "Created Student Account";
                        alertType = "success";

                    }//End of else statement

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
        public async Task CreateInternshipJournalAsync(int ScheduledRecordId)
        {

            try
            {
                var foundRecord = _context.ScheduleInternshipRecords.Where(sir => sir.ScheduleIntershipRecordId == ScheduledRecordId).Single();
                var newInternshipJournal = new Internship_Record
                {
                    LiaisonOfficerId = foundRecord.LiaisonOfficerId,
                    ProjectId = foundRecord.ProjectId,
                    UserBatchId = foundRecord.UserBatchId
                };
                _context.Internship_Records.Add(newInternshipJournal);
                _context.ScheduleInternshipRecords.Remove(foundRecord);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }


        }
    }
}