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
using Microsoft.VisualBasic;
using System.IO;

namespace E_Internship_Journal.API
{
    [Produces("application/json")]
    [Route("api/Courses")]
    public class CoursesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CoursesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/Courses
        /* [HttpGet]
         public IEnumerable<Course> GetCourses()
         {
             return _context.Courses;
         }*/
        // GET: api/Courses
        [HttpGet]
        public IActionResult GetCourses()
        {
            string customMessage = "";
            List<object> courseList = new List<object>();
            var courses = _context.Courses;
            //  .Where(eachCategory => eachCategory.DeletedAt == null)

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
                //HttpResponseMessage response =  Request.CreateResponse("Error message");
                //Return a bad http request message to the client
                return BadRequest(httpFailRequestResultMessage);
                // return BadRequest(httpFailRequestResultMessage);
            }
            return new JsonResult(courseList);
        }

        // GET: api/Courses/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetCourse([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var oneCourse = _context.Courses
          .SingleOrDefaultAsync(item => item.CourseId == id);
            // var course = await _context.Courses.SingleOrDefaultAsync(m => m.CourseId == id);

            if (oneCourse == null)
            {
                return NotFound();
            }

            return Ok(oneCourse);
        }

        // PUT: api/Courses/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCourse([FromRoute] int id, [FromBody] Course course)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != course.CourseId)
            {
                return BadRequest();
            }

            _context.Entry(course).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CourseExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Courses
        [HttpPost]
        //public async Task<IActionResult> SaveNewCourseInformation([FromBody] Course course)
        public async Task<IActionResult> SaveNewCourseInformation([FromBody] string value)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            //  _context.Courses.Add(course);
            string customMessage = "";
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
                customMessage = "Unable to save to database";
            }
            var successRequestResultMessage = new
            {
                Message = "Saved Course into database"
            };

            OkObjectResult httpOkResult =
new OkObjectResult(successRequestResultMessage);
            return httpOkResult;
            //return CreatedAtAction("GetCourse", new { id = course.CourseId }, course);
        }

        // POST: api/Courses
        [HttpPost("SaveNewCourseViaFile")]
        //public async Task<IActionResult> SaveNewCourseInformation([FromBody] Course course)
        public async Task<IActionResult> SaveNewCourseViaFile([FromBody] string value)
        {
          
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            using (var reader = new StreamReader(@"C:\test.csv"))
            {
                List<string> listA = new List<string>();
                List<string> listB = new List<string>();
                reader.ReadLine();
                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();
                    var values = line.Split(';');

                    listA.Add(values[0]);
                    listB.Add(values[1]);
                }
            }

            //  _context.Courses.Add(course);
            string customMessage = "";
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
                customMessage = "Unable to save to database";
            }
            var successRequestResultMessage = new
            {
                Message = "Saved Course into database"
            };

            OkObjectResult httpOkResult =
new OkObjectResult(successRequestResultMessage);
            return httpOkResult;
            //return CreatedAtAction("GetCourse", new { id = course.CourseId }, course);
        }

        // DELETE: api/Courses/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCourse([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var course = await _context.Courses.SingleOrDefaultAsync(m => m.CourseId == id);
            if (course == null)
            {
                return NotFound();
            }

            _context.Courses.Remove(course);
            await _context.SaveChangesAsync();

            return Ok(course);
        }

        private bool CourseExists(int id)
        {
            return _context.Courses.Any(e => e.CourseId == id);
        }
    }
}