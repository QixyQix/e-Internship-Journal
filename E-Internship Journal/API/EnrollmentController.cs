using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using E_Internship_Journal.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.IO;
using E_Internship_Journal.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Newtonsoft.Json;
using E_Internship_Journal.Services;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace E_Internship_Journal.Controllers
{
    [Route("api/enroll")]
    public class EnrollmentController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IEmailSender _emailSender;
        public EnrollmentController(ApplicationDbContext context, IEmailSender emailSender)
        {
            _emailSender = emailSender;
            _context = context;
        }

        //POST: Enroll student into batch
        [HttpPost("EnrollStudent/{id}")]
        //[Authorize(Roles = "SLO")]
        public async Task<IActionResult> EnrollStudent(int id)
        {

            List<string> messageList = new List<string>();
            string alertType = "success";
            var files = Request.Form.Files;
            //Get the batch & course
            var thisBatch = _context.Batches
                .Where(batch => batch.BatchId == id)
                .Include(batch => batch.Course)
                .Single();

            var csvFile = files[0];
            List<string> csvLine = new List<string>();
            using (var memoryStream = new MemoryStream())
            {

                await csvFile.CopyToAsync(memoryStream);
                memoryStream.Position = 0;
                using (var streamReader = new StreamReader(memoryStream))
                {
                    var heading = streamReader.ReadLine();
                    //Check if CSV file is in correct order
                    if (!heading.Equals("Student Id,Student Name,Email,Contact No"))
                    {
                        return BadRequest(new { Message = "CSV file does not follow correct format" });
                    }

                    //Read the file
                    string fileLine = "";

                    while ((fileLine = streamReader.ReadLine()) != null)
                    {
                        csvLine.Add(fileLine);
                    }
                }
            }
            var studentAdded = new List<UserBatch>();
            var studentCreated = new List<RegistrationPin>();
            var studentCreatedBatch = new List<UserBatch>();
            foreach (var line in csvLine)
            {

                if (!(line.Replace(",", "").Trim().Equals("")))
                {
                    try
                    {
                        //Get individual data
                        string[] oneStudentData = line.Split(',');

                        var user = _context.ApplicationUsers.SingleOrDefault(appuser => appuser.UserName.Equals(oneStudentData[2], StringComparison.OrdinalIgnoreCase));

                        if (user != null)
                        {
                            //Check if the user is already enrolled
                            if (!(_context.UserBatches.Any(ub => ub.UserId.Equals(user.Id) && ub.BatchId == thisBatch.BatchId)))
                            {

                                var www = (_context.UserBatches.Any(ub => ub.UserId.Equals(user.Id) && ub.BatchId == thisBatch.BatchId));
                                //Create UserBatch objects
                                var newUserBatch = new UserBatch
                                {
                                    Batch = thisBatch,
                                    User = user
                                };
                                studentAdded.Add(newUserBatch);
                                _context.UserBatches.Add(newUserBatch);
                                await _context.SaveChangesAsync();
                            }
                            else
                            {
                                messageList.Add(user.FullName + " is already enrolled in this batch.");
                                alertType = "warning";
                            }
                        }
                        else
                        {
                            //Create user
                            var userStore = new UserStore<ApplicationUser>(_context);
                            var userManager = new UserManager<ApplicationUser>(userStore, null, null, null, null, null, null, null, null);

                            var newStudentUser = new ApplicationUser
                            {
                                StudentId = oneStudentData[0],
                                UserName = oneStudentData[2],
                                Email = oneStudentData[2],
                                FullName = oneStudentData[1],
                                PhoneNumber = oneStudentData[3]
                            };
                            PasswordHasher<ApplicationUser> ph = new PasswordHasher<ApplicationUser>();
                            newStudentUser.PasswordHash = ph.HashPassword(newStudentUser, generateRandomString(11));

                            await userManager.CreateAsync(newStudentUser);
                            await userManager.AddToRoleAsync(newStudentUser, "STUDENT");

                            //Create UserBatch objects
                            var newUserBatch = new UserBatch
                            {
                                Batch = thisBatch,
                                User = newStudentUser
                            };
                            _context.UserBatches.Add(newUserBatch);

                            studentCreatedBatch.Add(newUserBatch);
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
                                    studentCreated.Add(newRegistrationPin);
                                }
                            } while (repeatPinGeneration);
                            await _context.SaveChangesAsync();
                        }//End of Else

                    }
                    catch (Exception ex)
                    {
                        return BadRequest();

                    }
                }
            }
            var absoluteUri = string.Concat(
                  Request.Scheme,
                  "://",
                  Request.Host.ToUriComponent(),
                  Request.PathBase.ToUriComponent());
            for (int i = 0; i < studentCreated.Count; i++)
            {
                var studentEmail = studentCreated[i].User.Email;
                var studentName = studentCreated[i].User.FullName;
                var batchName = studentCreatedBatch[i].Batch.BatchName;
                var startDate = studentCreatedBatch[i].Batch.StartDate.ToString("dd MMMM yyyy");
                var endDate = studentCreatedBatch[i].Batch.EndDate.ToString("dd MMMM yyyy");
                await _emailSender.SendChangeEmailAsync(true, studentEmail, "Your account has been created and enrolled!",
                    "Hi, " + studentName, "Your student account has been created and enrolled into Batch " + batchName + "." +
                    " The Semester will start from " + startDate + " and end on " + endDate + ". Kindly proceed to activate your account before your internship starts.",
                     absoluteUri + "/Account/SetPassword?registrationPin=" + studentCreated[i].RegistrationPinId, "Activate Account");
            }
            for (int k = 0; k < studentAdded.Count; k++)
            {
                var studentEmail = studentAdded[k].User.Email;
                var studentName = studentAdded[k].User.FullName;
                var batchName = studentAdded[k].Batch.BatchName;
                var startDate = studentAdded[k].Batch.StartDate.ToString("dd MMMM yyyy");
                var endDate = studentAdded[k].Batch.EndDate.ToString("dd MMMM yyyy");
                await _emailSender.SendChangeEmailAsync(false, studentEmail, "You have been enrolled into " + batchName, "Hi, " + studentName,
                    "You have been enrolled into Batch " + batchName + "." +
                    " The Semester will start from " + startDate + " and end on " + endDate + ".Kindly login to confirm your batch.", "", "");

            }


            messageList.Add("All Records saved successfully!");
            var responseObject = new
            {
                AlertType = alertType,
                Messages = messageList
            };

            return new OkObjectResult(responseObject);
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

        [HttpPut("SetPassword/{pin}")]
        [AllowAnonymous]
        public IActionResult SetPassword(string pin, [FromBody] string value)
        {
            RegistrationPin rp = _context.RegistrationPins.Where(p => p.RegistrationPinId.Equals(pin)).Include(p => p.User).SingleOrDefault();

            if (rp != null)
            {
                var passwordInput = JsonConvert.DeserializeObject<dynamic>(value);

                var userStore = new UserStore<ApplicationUser>(_context);
                var userManager = new UserManager<ApplicationUser>(userStore, null, null, null, null, null, null, null, null);

                PasswordHasher<ApplicationUser> ph = new PasswordHasher<ApplicationUser>();
                rp.User.PasswordHash = ph.HashPassword(rp.User, passwordInput.Password.Value.ToString());

                _context.SaveChanges();
                _context.RegistrationPins.Remove(rp);
                _context.SaveChanges();
                return new OkObjectResult(new { Message = "Set password successfully! You may now log in." });
            }
            else
            {
                return BadRequest();
            }
        }

        [HttpPut("MassAssignStudent/{id}")]
        [Authorize(Roles = "SLO")]
        public async Task<IActionResult> MassAssignStudent(int id, [FromBody] string value)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var assignmentInput = JsonConvert.DeserializeObject<dynamic>(value);

            var studentProjects = assignmentInput.studentProjects;

            foreach (var pairing in studentProjects)
            {

                string studentUserId = pairing.studentId.Value.ToString().Trim();
                var projectIdInput = pairing.projectId.Value;
                int projectId = 0;

                if (string.IsNullOrEmpty(studentUserId) || !Int32.TryParse(projectIdInput, out projectId))
                {
                    return BadRequest();
                }

                //Find the userbatch for the user
                var userBatch = _context.UserBatches.SingleOrDefault(ub => ub.BatchId == id && ub.UserId.Equals(studentUserId));
                //Find the project
                var project = _context.Projects.Find(projectId);

                if (userBatch == null || project == null)
                {
                    return BadRequest();
                }

                var internshipRecord = new Internship_Record
                {
                    UserBatch = userBatch,
                    Project = project
                };

                _context.Internship_Records.Add(internshipRecord);
            }
            //Save changes in db
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
