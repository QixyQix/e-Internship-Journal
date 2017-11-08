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

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace E_Internship_Journal.Controllers
{
    [Route("api/[controller]")]
    public class EnrollmentController : Controller
    {
        private readonly ApplicationDbContext _context;

        public EnrollmentController(ApplicationDbContext context)
        {
            _context = context;
        }

        //POST: Enroll student into batch
        [HttpPost("EnrollStudent/{id}")]
        [Authorize(Roles = "SLO")]
        public async Task<IActionResult> EnrollStudent(int id, List<IFormFile> files)
        {

            //Get the batch & course
            var thisBatch = _context.Batches
                .Where(batch => batch.BatchId == id)
                .Include(batch => batch.Course)
                .Single();

            var csvFile = files[0];

            using (var memoryStream = new MemoryStream())
            {
                List<string> csvLine = new List<string>();
                await csvFile.CopyToAsync(memoryStream);
                memoryStream.Position = 0;
                using (var streamReader = new StreamReader(memoryStream))
                {
                    var heading = streamReader.ReadLine();
                    //Check if CSV file is in correct order
                    if (!heading.Equals("Student Name,Email,Contact No,Project,Company"))
                    {
                        return BadRequest("CSV file does not follow correct format");
                    }

                    //Read the file
                    string fileLine = "";

                    while ((fileLine = streamReader.ReadLine()) != null)
                    {
                        csvLine.Add(fileLine);
                    }
                }

                foreach (var line in csvLine)
                {

                    if (!(line.Replace(",", "").Trim().Equals("")))
                    {
                        try
                        {
                            //Get individual data
                            string[] oneStudentData = line.Split(',');

                            var user = _context.ApplicationUsers.SingleOrDefault(appuser => appuser.UserName.Equals(oneStudentData[1], StringComparison.OrdinalIgnoreCase));

                            if (user != null)
                            {
                                //Check if the user is already enrolled
                                if (!(_context.UserBatches.Any(ub => ub.UserId == user.Id && ub.BatchId == thisBatch.BatchId)))
                                {
                                    //Create UserBatch objects
                                    var newUserBatch = new UserBatch
                                    {
                                        Batch = thisBatch,
                                        User = user
                                    };
                                    _context.UserBatches.Add(newUserBatch);
                                }
                            }
                            else
                            {
                                //Create user
                                var userStore = new UserStore<ApplicationUser>(_context);
                                var userManager = new UserManager<ApplicationUser>(userStore, null, null, null, null, null, null, null, null);

                                var newStudentUser = new ApplicationUser
                                {
                                    UserName = oneStudentData[1],
                                    Email = oneStudentData[1],
                                    FullName = oneStudentData[0],
                                    PhoneNumber = oneStudentData[2],
                                    Course = thisBatch.Course
                                };
                                PasswordHasher<ApplicationUser> ph = new PasswordHasher<ApplicationUser>();
                                newStudentUser.PasswordHash = ph.HashPassword(newStudentUser, generateRandomString(11)); //TODO: Random default password generator

                                await userManager.CreateAsync(newStudentUser);
                                await userManager.AddToRoleAsync(newStudentUser, "STUDENT");

                                //Create UserBatch objects
                                var newUserBatch = new UserBatch
                                {
                                    Batch = thisBatch,
                                    User = newStudentUser
                                };
                                _context.UserBatches.Add(newUserBatch);

                                var repeatPinGeneration = false;

                                do
                                {
                                    repeatPinGeneration = false;
                                    try
                                    {
                                        //Create new registration pin
                                        var newRegistrationPin = new RegistrationPin
                                        {
                                            User = newStudentUser,
                                            RegistrationPinId = generateRandomString(50)
                                        };
                                        _context.RegistrationPins.Add(newRegistrationPin);
                                    }
                                    catch (Exception ex)
                                    {
                                        repeatPinGeneration = true;
                                    }
                                } while (repeatPinGeneration);
                            }
                            await _context.SaveChangesAsync();
                        }
                        catch (Exception ex)
                        {
                            return BadRequest();

                        }
                    }
                }
            }
            return Ok();
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

        [HttpPut("MassAssignStudent/{id}")]
        [Authorize(Roles = "SLO")]
        public async Task<IActionResult> MassAssignStudent(int id, [FromBody] string value) {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var assignmentInput = JsonConvert.DeserializeObject<dynamic>(value);

            var studentProjects = assignmentInput.studentProjects;

            foreach (var pairing in studentProjects) {

                string studentUserId = pairing.studentId.Value.ToString().Trim();
                var projectIdInput = pairing.projectId.Value;
                int projectId = 0;

                if (string.IsNullOrEmpty(studentUserId) || !Int32.TryParse(projectIdInput, out projectId)) {
                    return BadRequest();
                }

                //Find the userbatch for the user
                var userBatch = _context.UserBatches.SingleOrDefault(ub => ub.BatchId == id && ub.UserId.Equals(studentUserId));
                //Find the project
                var project = _context.Projects.Find(projectId);

                if (userBatch == null || project == null) {
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
