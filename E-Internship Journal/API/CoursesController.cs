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
        //public async Task<IActionResult> SaveNewCourseViaFile([FromBody] IList<IFormFile> fileInput)
        public async Task<IActionResult> SaveNewCourseViaFile(List<IFormFile> files)
        {
            var test = files[0];


            var filePath = Path.GetTempFileName();
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                //  await test.CopyToAsync(stream);
                //  System.IO.File.Delete();

            }

            var form = Request.Form;

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            //string ff = "@" + filePath;
            string path = @"c:\temp\MyTest.txt";
            // using (var reader = new StreamReader(path))
            //using (StreamReader reader = System.IO.File.OpenText(filePath))
            //{
            //    List<string> listA = new List<string>();
            //    List<string> listB = new List<string>();
            //    reader.ReadLine();
            //    while (!reader.EndOfStream)
            //    {
            //        var line = reader.ReadLine();
            //        var values = line.Split(';');

            //       /* listA.Add(values[0]);
            //        listB.Add(values[1]);*/
            //    }
            //  //  File.del
            //}

            /*  string myFileName = test.Name;
              byte[] myFileContent;

              using (var memoryStream = new MemoryStream())
              {
                  await test.CopyToAsync(memoryStream);
                  memoryStream.Seek(0, SeekOrigin.Begin);
                  myFileContent = new byte[memoryStream.Length];
                  var teeee = await memoryStream.ReadAsync(myFileContent, 0, myFileContent.Length);
              }*/
            // using (var reader = new StreamReader(@"C:\test.csv"))
            //using (var readStream = test.OpenReadStream())
            //{
            //   /* StreamContent scontent = new StreamContent(readStream);
            //    scontent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/vnd.ms-excel");
            //    using (MultipartFormDataContent ss = new MultipartFormDataContent()) // Creates a multipart form data for the HTTP POST request
            //    {
            //        ss.Add(scontent, @"""file""", @"""image""");
            //        string msg2 = "test";
            //    }*/

            //    //var reader = new StreamReader(readStream);
            //    // readStream.;
            //    string msg = "test";
            //    /*   List<string> listA = new List<string>();
            //       List<string> listB = new List<string>();
            //       reader.ReadLine();
            //       while (!reader.EndOfStream)
            //       {
            //           var line = reader.ReadLine();
            //           var values = line.Split(';');

            //           listA.Add(values[0]);
            //           listB.Add(values[1]);*/
            //    //}
            //    // Stream TargetStream = new Stream;
            //    Stream stream = new MemoryStream();

            //    Stream TargetStream = Stream.Null;
            //    const int BUFFER_SIZE = 4096;
            //    byte[] buffer = new byte[BUFFER_SIZE];
            //    //Reset the source stream in order to process all data.
            //    if (readStream.CanSeek)
            //        readStream.Position = 0;
            //    //Copy data from the source stream to the target stream.
            //    int BytesRead = 0;
            //    while ((BytesRead = readStream.Read(buffer, 0, BUFFER_SIZE)) > 0)
            //        TargetStream.Write(buffer, 0, BytesRead);
            //    stream.Write(buffer, 0, BytesRead);
            //    //Reset the source stream and the target stream to make them ready for any other operation.
            //    if (readStream.CanSeek)
            //        readStream.Position = 0;
            //    if (TargetStream.CanSeek)
            //        TargetStream.Position = 0;

            //    var testtt = TargetStream.Read(buffer, 0, (int)stream.Length);
            //    var ttt = System.Text.Encoding.UTF8.GetString((stream as MemoryStream).ToArray());
            //    using (var streamReader = new StreamReader(stream))
            //    {
            //        var tttt = streamReader.ReadToEnd();
            //        string msg4423 = "test";
            //    }
            //        string msg23 = "test";
            //}


            /*  using (var reader = new StreamReader(@"C:\test.csv"))
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
              }*/
            using (var testt = new MemoryStream())
            {
                await test.CopyToAsync(testt);
                //  await test.CopyToAsync(stream);
                //  System.IO.File.Delete();
                testt.Seek(0, SeekOrigin.Begin); // <-- missing line
                byte[] buf = new byte[testt.Length];
                var qqqq = testt.Read(buf, 0, buf.Length);
                //var last = testt.ReadTo
                testt.Position = 0;
                using (var streamReader = new StreamReader(testt))
                {
                    
                    var tttt = streamReader.ReadToEnd();
                    var testtt = streamReader.ReadLine();
                    string msg4423 = "test";
                }
                string msg23 = "test";
            }
            return Ok();
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