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

        public BatchesController(ApplicationDbContext context)
        {
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

        // GET api/Brands/5
        [HttpGet("GetOneBatches/{id}")]
        public IActionResult GetOneBatches(int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            if (BatchExists(id))
            {
                try
                {
                    var foundCourses = _context.Batches
                    .Where(eachCourse => eachCourse.CourseId == id)
                    .Include(eachCourse => eachCourse.Course).Single();
                    var response = new
                    {
                        CourseId = foundCourses.CourseId,
                        Description = foundCourses.Description,
                        StartDate = foundCourses.StartDate,
                        EndDate = foundCourses.EndDate,
                        CourseName = foundCourses.Course.CourseName,
                        CourseCode = foundCourses.Course.CourseCode
                    };//end of creation of the response object
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
                object httpFailRequestResultMessage =
                new { Message = "Invalid Batch ID" };
                //Return a bad http response message to the client
                return BadRequest(httpFailRequestResultMessage);

            }
            return NoContent();
        }

        //PUT: Enroll student into batch
        [HttpPost("EnrollStudent/{id}")]
        //[Authorize(Roles = "SLO")]
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
                    //try
                    //{
                        while ((fileLine = streamReader.ReadLine()) != null)
                        {
                            //Get individual data
                            string[] oneStudentData = fileLine.Split(',');

                            var name = oneStudentData[0];
                            var email = oneStudentData[1];
                            var mobileNo = oneStudentData[2];

                            //Create user
                            var userStore = new UserStore<ApplicationUser>(_context);
                            var userManager = new UserManager<ApplicationUser>(userStore, null, null, null, null, null, null, null, null);

                            var newStudentUser = new ApplicationUser
                            {
                                UserName = email,
                                Email = email,
                                FullName = name,
                                PhoneNumber = mobileNo,
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
                            _context.SaveChanges();

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
                                _context.SaveChanges();
                            }
                            catch (Exception ex) {
                                repeatPinGeneration = true;
                            }
                        } while (repeatPinGeneration);
                        }
                    //}
                    //catch (Exception ex)
                    //{
                    //    return BadRequest();
                    //}
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