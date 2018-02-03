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
                    eachBatch.Course.CourseName,
                    UserList
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
                        .ThenInclude(ub => ub.User)
                        .AsNoTracking();

                    foreach (var eachBatch in batches)
                    {

                        List<string> UserList = new List<string>();

                        foreach (var eachUserBatch in eachBatch.UserBatches)
                        {
                            if (await _userManager.IsInRoleAsync(eachUserBatch.User, "SLO"))
                            {
                                UserList.Add(eachUserBatch.User.Id);
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
                            eachBatch.Course.CourseName,
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
                    var batch = _context.Batches
                        .Where(b => b.BatchId == id)
                        .Include(userBatch => userBatch.UserBatches)
                        .ThenInclude(ub => ub.User)
                        .SingleOrDefault();

                    List<UserBatch> SloUbs = new List<UserBatch>();
                    List<string> SloIds = new List<string>();

                    foreach (var sloid in batchNewInput.SLOAssigned)
                    {
                        SloIds.Add(sloid.ToString());
                    }

                    foreach (var eachUserBatch in batch.UserBatches)
                    {
                        if (await _userManager.IsInRoleAsync(eachUserBatch.User, "SLO"))
                        {
                            SloUbs.Add(eachUserBatch);
                        }
                    }

                    //Create userbatch for those that do not exist
                    foreach (var sloid in SloIds)
                    {
                        if (!SloUbs.Any(ub => ub.User.Id.Equals(sloid)))
                        {
                            _context.UserBatches.Add(new UserBatch
                            {
                                Batch = batch,
                                User = await _userManager.FindByIdAsync(sloid.ToString())
                            });
                        }
                    }

                    //Delete userbatch fo those that exist
                    foreach (var ub in SloUbs)
                    {
                        if (!SloIds.Contains(ub.User.Id))
                        {
                            _context.UserBatches.Remove(ub);
                        }
                    }

                    await _context.SaveChangesAsync();

                    batch.BatchName = batchNewInput.BatchName.Value;
                    batch.StartDate = DateTime.ParseExact(batchNewInput.StartDate.Value, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                    batch.EndDate = DateTime.ParseExact(batchNewInput.EndDate.Value, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                    batch.CourseId = Convert.ToInt32(batchNewInput.CourseAssigned.Value.ToString());

                    response = new { Status = "success", Message = "Saved batch record." };
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

        // POST: api/Courses
        [HttpPost("BulkAddBatch")]
        public async Task<IActionResult> BulkAddUsers()
        {
            List<string> messageList = new List<string>();
            var alertType = "success";
            string title = "Action Completed";
            var files = Request.Form.Files;

            var csvFile = files[0];

            using (var memoryStream = new MemoryStream())
            {
                List<string> csvLine = new List<string>();
                await csvFile.CopyToAsync(memoryStream);
                memoryStream.Position = 0;

                using (var streamReader = new StreamReader(memoryStream))
                {
                    var heading = streamReader.ReadLine();
                    string[] headingArray = heading.Split(',');
                    //Check if CSV file is in correct order
                    if (!headingArray[0].Equals("Batch Name", StringComparison.OrdinalIgnoreCase) || !headingArray[1].Equals("Course Code", StringComparison.OrdinalIgnoreCase) || !headingArray[2].Equals("SLO Email", StringComparison.OrdinalIgnoreCase) || !headingArray[3].Equals("Start Date", StringComparison.OrdinalIgnoreCase) || !headingArray[4].Equals("End Date", StringComparison.OrdinalIgnoreCase))
                    {
                        return new BadRequestObjectResult(new { Message = "CSV file does not follow correct format" });
                    }

                    //Read the file
                    string fileLine = "";

                    while ((fileLine = streamReader.ReadLine()) != null)
                    {
                        csvLine.Add(fileLine);
                    }
                }

                var userStore = new UserStore<ApplicationUser>(_context);
                var userManager = new UserManager<ApplicationUser>(userStore, null, null, null, null, null, null, null, null);

                foreach (var line in csvLine)
                {
                    if (!(line.Replace(",", "").Trim().Equals("")))
                    {
                        string batchName = "";
                        try
                        {
                            //Get individual data
                            string[] batchData = line.Split(',');

                            Batch newBatch = new Batch();

                            newBatch.BatchName = batchData[0];
                            batchName = batchData[0];

                            Course course = _context.Courses.Where(c => c.CourseCode.Equals(batchData[1], StringComparison.OrdinalIgnoreCase)).SingleOrDefault();
                            newBatch.Course = course;

                            if (course == null)
                            {
                                messageList.Add("Could not find course for " + batchData[0] + "(" + batchData[1] + ")");
                            }

                            //Start and end date
                            newBatch.StartDate = DateTime.ParseExact(batchData[3], "dd/MM/yyyy", CultureInfo.InvariantCulture);
                            newBatch.EndDate = DateTime.ParseExact(batchData[4], "dd/MM/yyyy", CultureInfo.InvariantCulture);
                            _context.Batches.Add(newBatch);
                            _context.SaveChanges();

                            //SLO
                            ApplicationUser slo = _userManager.Users.Where(u => u.Email.Equals(batchData[2].Trim(), StringComparison.OrdinalIgnoreCase)).SingleOrDefault();
                            if (slo != null && await _userManager.IsInRoleAsync(slo, "SLO"))
                            {
                                UserBatch newUb = new UserBatch
                                {
                                    Batch = newBatch,
                                    User = slo
                                };
                                _context.UserBatches.Add(newUb);
                                _context.SaveChanges();
                            }
                            else
                            {
                                messageList.Add("Could not find SLO for " + batchData[0] + "(" + batchData[2] + ")");
                            }

                        }
                        catch (Exception ex)
                        {
                            messageList.Add("Could not create batch due identical batch or missing course (" + batchName + ")");
                        }
                    }
                }

                if (messageList.Count < 1)
                {
                    messageList.Add("Batches created successfully");
                }
                else
                {
                    title = "Action completed with errors";
                    alertType = "warning";
                }


                return new OkObjectResult(new { Title = title, Messages = messageList, AlertType = alertType });
            }
        }

        [HttpPost("SaveNewBatchInformation")]
        [Authorize(Roles = "ADMIN")]
        public async Task<IActionResult> SaveNewBatchInformation([FromBody] string value)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var batchNewInput = JsonConvert.DeserializeObject<dynamic>(value);
            Batch newBatch = new Batch();

            try
            {
                newBatch.BatchName = batchNewInput.BatchName.Value.ToString();
                newBatch.StartDate = DateTime.ParseExact(batchNewInput.StartDate.Value.ToString(), "dd/MM/yyyy", CultureInfo.InvariantCulture);
                newBatch.EndDate = DateTime.ParseExact(batchNewInput.EndDate.Value.ToString(), "dd/MM/yyyy", CultureInfo.InvariantCulture);
                newBatch.CourseId = Convert.ToInt32(batchNewInput.CourseAssigned.Value.ToString());

                _context.Batches.Add(newBatch);
                _context.SaveChanges();

                foreach (var id in batchNewInput.SLOAssigned)
                {
                    string SLOId = id.ToString();
                    ApplicationUser SLOUser = await _userManager.Users.Where(u => u.Id.Equals(SLOId, StringComparison.OrdinalIgnoreCase)).SingleOrDefaultAsync();

                    if (SLOUser != null && await _userManager.IsInRoleAsync(SLOUser, "SLO"))
                    {
                        UserBatch newUserBatch = new UserBatch
                        {
                            Batch = newBatch,
                            User = SLOUser
                        };

                        _context.UserBatches.Add(newUserBatch);
                    }
                }

                _context.SaveChanges();
            }

            catch (Exception exceptionObject)
            {
                return new BadRequestObjectResult(new { Message = exceptionObject.Message.ToString() });
            }
            var successRequestResultMessage = new
            {
                Message = "Saved Batch",
                Status = "success"
            };

            OkObjectResult httpOkResult =
            new OkObjectResult(successRequestResultMessage);
            return httpOkResult;
        }

        // DELETE: api/Batches/5
        [HttpDelete("{id}")]
        public IActionResult DeleteBatch(int id) {
            var batch = _context.Batches.Where(b => b.BatchId == id).Include(b => b.UserBatches).SingleOrDefault();

            if (batch == null)
            {
                return BadRequest(new { Message = "Batch does not exist!"});
            }
            else {
                bool delete = true;
                foreach (var ub in batch.UserBatches) {
                    if (_context.Internship_Records.Any(ir => ir.UserBatchId == ub.UserBatchId)) {
                        delete = false;
                    }
                }

                if (delete)
                {
                    _context.UserBatches.RemoveRange(batch.UserBatches);
                    _context.Batches.Remove(batch);
                    _context.SaveChanges();

                    return new OkObjectResult(new { Message = "Successfully deleted batch" , AlertType = "success", Title = "Successfully Deleted"});
                }
                else {
                    return new OkObjectResult(new { Message = "Could not delete batch as there are internship records attached to this batch!", AlertType = "error", Title = "An error occured" });
                }
            }
        }

        [HttpPut("bulkDelete")]
        [Authorize(Roles ="ADMIN")]
        public IActionResult BulkDelete([FromBody] string value) {
            var idList = JsonConvert.DeserializeObject<dynamic>(value);

            string alertType = "success";
            string title = "Action Completed";
            List<String> messageList = new List<String>();

            try
            {
                foreach (var idstr in idList)
                {
                    int id = Int32.Parse(idstr.ToString());
                    var batch = _context.Batches.Where(b => b.BatchId == id).Include(b => b.UserBatches).SingleOrDefault();

                    if (batch != null)
                    {
                        bool delete = true;
                        foreach (var ub in batch.UserBatches)
                        {
                            if (_context.Internship_Records.Any(ir => ir.UserBatchId == ub.UserBatchId))
                            {
                                delete = false;
                            }
                        }

                        if (delete)
                        {
                            _context.UserBatches.RemoveRange(batch.UserBatches);
                            _context.Batches.Remove(batch);
                            _context.SaveChanges();
                        }
                        else
                        {
                            messageList.Add(batch.BatchName + " could not be deleted as there are internship records tied to it");
                            alertType = "warning";
                            title = "Completed with errors";
                        }
                    }
                    if (messageList.Count < 1)
                        messageList.Add("Deleted all batches successfully");

                    _context.SaveChanges();
                }
                return new OkObjectResult(new { Messages = messageList, AlertType = alertType, Title = title });
            }
            catch (Exception exceptionObject)
            {
                object httpFailRequestResultMessage = new { Message = "Unable to Process" };
                //Return a bad http response message to the client
                return BadRequest(httpFailRequestResultMessage);
            }
        }


        private bool BatchExists(int id)
        {
            return _context.Batches.Any(e => e.BatchId == id);
        }
    }
}