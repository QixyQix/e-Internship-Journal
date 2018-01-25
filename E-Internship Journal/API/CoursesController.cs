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
using System.IO;
using System.Net.Http;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

namespace E_Internship_Journal.API
{
    [Produces("application/json")]
    [Route("api/Courses")]
    public class CoursesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        public CoursesController(UserManager<ApplicationUser> userManager, ApplicationDbContext context)
        {
            _userManager = userManager;
            _context = context;
        }

        [HttpGet]
        public IActionResult GetCourses()
        {
            string customMessage = "";
            List<object> courseList = new List<object>();
            var courses = _context.Courses;

            foreach (var oneCourse in courses)
            {
                courseList.Add(new
                {
                    CourseId = oneCourse.CourseId,
                    CourseName = oneCourse.CourseName,
                    CourseCode = oneCourse.CourseCode

                });

            }

            if (courseList.Count == 0)
            {
                customMessage = "No Record Found";
                object httpFailRequestResultMessage = new { Message = customMessage };
                return BadRequest(httpFailRequestResultMessage);
            }
            return new JsonResult(courseList);
        }

        // GET: api/Courses/5
        [HttpGet("{id}")]
        public IActionResult GetCourse(int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var oneCourse = _context.Courses.Where(item => item.CourseId == id).SingleOrDefault();

            var response = new
            {
                oneCourse.CourseId,
                oneCourse.CourseName,
                oneCourse.CourseCode
            };

            if (oneCourse == null)
            {
                return NotFound();
            }

            return new JsonResult(response);
        }
        
        [HttpGet("getAssignedCourse")]
        [Authorize(Roles = "SLO")]
        public IActionResult getAssignedCourse()
        {
            List<object> courses_List = new List<object>();

            var courses = _context.UserBatches
                .Include(b => b.Batch)
                .ThenInclude(c => c.Course)
                .Where(ub => ub.UserId.Equals(_userManager.GetUserId(User)))
                .ToList();

            foreach (var oneCourse in courses)
            {
                //   List<int> categoryIdList = new List<int>();
                courses_List.Add(new
                {
                    oneCourse.Batch.Course.CourseId,
                    oneCourse.Batch.Course.CourseName
                });
            }

            return new JsonResult(courses_List);
        }

        [HttpPut("UpdateOneCourse/{id}")]
        public async Task<IActionResult> UpdateOneCourse(int id, [FromBody] string value)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            string customMessage = "";
            if (CourseExists(id))
            {
                try
                {
                    var courseNewInput = JsonConvert.DeserializeObject<dynamic>(value);
                    var foundOneCourse = _context.Courses.Find(id);

                    foundOneCourse.CourseCode = courseNewInput.CourseCode.Value;
                    foundOneCourse.CourseName = courseNewInput.CourseName.Value;
                    _context.Courses.Update(foundOneCourse);
                    await _context.SaveChangesAsync();
                }
                catch (Exception e)
                {
                    return new BadRequestObjectResult(new { Message = e.Message.ToString() });
                }
            }
            else
            {
                return new BadRequestObjectResult(new { Message = "Course does not exist" });
            }

            return new OkObjectResult(new { Message = "Course Updated Successfully!" });
        }


        // POST: api/Courses
        [HttpPost("SaveNewCourseInformation")]
        public async Task<IActionResult> SaveNewCourseInformation([FromBody] string value)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var courseNewInput = JsonConvert.DeserializeObject<dynamic>(value);
            Course newCourse = new Course();

            try
            {
                newCourse.CourseCode = courseNewInput.CourseCode.Value;
                newCourse.CourseName = courseNewInput.CourseName.Value;
                _context.Courses.Add(newCourse);
                await _context.SaveChangesAsync();

            }
            catch (Exception exceptionObject)
            {
                object httpFailRequestResultMessage = new { Message = exceptionObject };
                //Return a bad http response message to the client
                return BadRequest(httpFailRequestResultMessage);
            }
            var successRequestResultMessage = new
            {
                Message = "Saved Course into database"
            };

            OkObjectResult httpOkResult = new OkObjectResult(successRequestResultMessage);
            return httpOkResult;
            //return CreatedAtAction("GetCourse", new { id = course.CourseId }, course);
        }

        // POST: api/Courses
        [HttpPost("BulkAddCourse")]
        public async Task<IActionResult> SaveNewCourseViaFile()
        {
            List<string> messageList = new List<string>();
            var alertType = "success";
            string title = "Success";
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
                    if (!headingArray[0].Equals("Course Code", StringComparison.OrdinalIgnoreCase) || !headingArray[1].Equals("Course Name", StringComparison.OrdinalIgnoreCase))
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

                foreach (var line in csvLine)
                {
                    if (!(line.Replace(",", "").Trim().Equals("")))
                    {
                        try
                        {
                            //Get individual data
                            string[] oneCourseData = line.Split(',');

                            if (!_context.Courses.Any(c => c.CourseCode.Equals(oneCourseData[0], StringComparison.OrdinalIgnoreCase) && c.CourseName.Equals(oneCourseData[1], StringComparison.OrdinalIgnoreCase)))
                            {
                                Course newCourse = new Course { CourseCode = oneCourseData[0], CourseName = oneCourseData[1] };
                                _context.Courses.Add(newCourse);
                            }
                            else
                            {
                                if (messageList.Count < 1)
                                {
                                    messageList.Add("Completed with the following errors:");
                                    alertType = "warning";
                                    title = "Completed with errors";
                                }

                                messageList.Add(oneCourseData[1] + "(" + oneCourseData[0] + ") Another identical course already exists and was therefore not created");
                            }
                        }
                        catch (Exception ex)
                        {
                            return BadRequest();
                        }
                    }
                }
                await _context.SaveChangesAsync();

                if (messageList.Count < 1)
                    messageList.Add("Courses in csv file created successfully!");

                return new OkObjectResult(new { Title = title, Messages = messageList, AlertType = alertType });
            }
        }

        // DELETE: api/Courses/5
        [HttpDelete("{id}")]
        public IActionResult DeleteCourse([FromRoute] int id)
        {
            var course = _context.Courses.Include(c => c.CompetencyTitle).Include(c => c.Batches).SingleOrDefault(c => c.CourseId == id);
            if (course == null)
            {
                return NotFound();
            }
            else if (course.CompetencyTitle.Count() > 0 || course.Batches.Count() > 0)
            {
                return new OkObjectResult(new { Message = "Unable to delete course - This course is attached to competencies and or batches", Title = "Error", AlertType = "error" });
            }

            _context.Courses.Remove(course);
            _context.SaveChanges();

            return new OkObjectResult(new { Message = "Course successfully deleted!", Title = "Course Deleted", AlertType = "success" });
        }

        [HttpPut("bulkDelete")]
        public IActionResult BulkDeleteCourse([FromBody] string value)
        {

            var alertType = "success";
            var courseIds = JsonConvert.DeserializeObject<dynamic>(value);
            List<string> messageList = new List<string>();
            try
            {
                foreach (var idstr in courseIds.CourseIds)
                {

                    int id = Int32.Parse(idstr.ToString());

                    var course = _context.Courses.Include(c => c.CompetencyTitle).Include(c => c.Batches).SingleOrDefault(c => c.CourseId == id);
                    if (course == null)
                    {
                        return NotFound();
                    }
                    else if (course.CompetencyTitle.Count() > 0 || course.Batches.Count() > 0)
                    {
                        if (messageList.Count < 1)
                            messageList.Add("Action completed with the following errors:");
                        messageList.Add(course.CourseName + "(" + course.CourseCode + ")" + " cannot be deleted (competencies and or batches) ");
                        alertType = "warning";
                    }
                    else
                    {
                        _context.Courses.Remove(course);
                    }
                }
                _context.SaveChanges();

                return new OkObjectResult(new { Messages = messageList, AlertType = alertType });
            }
            catch (Exception e)
            {
                return new BadRequestObjectResult(new { Message = e.Message.ToString() });
            }


        }

        private bool CourseExists(int id)
        {
            return _context.Courses.Any(e => e.CourseId == id);
        }
    }
}