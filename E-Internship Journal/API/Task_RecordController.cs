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

namespace E_Internship_Journal.API
{
    [Produces("application/json")]
    [Route("api/Task_Record")]
    public class Task_RecordController : Controller
    {
        private readonly ApplicationDbContext _context;

        public Task_RecordController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/Task_Record
        [HttpGet]
        public IActionResult GetTasks()
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            List<object> task_Record_List = new List<object>();
            var task_Records = _context.Tasks
                .Include(eachTaskEntity =>eachTaskEntity.DayRecord)

            .AsNoTracking();
            foreach (var oneTaskRecord in task_Records)
            {
                //   List<int> categoryIdList = new List<int>();
                task_Record_List.Add(new
                {
                    oneTaskRecord.TaskRecordId,
                    oneTaskRecord.DayRecordId,
                    oneTaskRecord.Description,

                   // oneTaskRecord.DayRecord.Date
                });
            }

            return new JsonResult(task_Record_List);
        }

        // GET: api/Task_Record/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetTask_Record(int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            if (Task_RecordExists(id))
            {
                try
                {
                    List<object> TaskList = new List<object>();
                    var foundTasks = _context.Tasks
                        .Where(eachTaskEntity => eachTaskEntity.DayRecordId == id)
                        .Include(eachTaskENtity => eachTaskENtity.DayRecord)
                        .AsNoTracking();
                    foreach (var oneTask in foundTasks)
                    {
                        TaskList.Add(new
                        {
                            DayRecordId = oneTask.DayRecordId,
                            TaskRecordId = oneTask.TaskRecordId,
                            Description = oneTask.Description,
                            Date = oneTask.DayRecord.Date,
                            ArrivalTime = oneTask.DayRecord.ArrivalTime,
                            DepartureTime = oneTask.DayRecord.DepartureTime
                            //WeekNo = oneTask.DayRecord.WeekNo

                        });

                    };//end of creation of the response object
                    return new JsonResult(TaskList);
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


            }//End of Get(id) Web API method
        }

        // PUT: api/Task_Record/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutTask_RecordAsync(int id, [FromBody] string value)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            if (Task_RecordExists(id))
            {
                var task_RecordNewInput = JsonConvert.DeserializeObject<dynamic>(value);
                try
                {
                    var foundOneTask = _context.Tasks
                        .Where(eachTaskEntity => eachTaskEntity.DayRecordId == id)
                        .Single();

                    Task_Record newTask_Record = new Task_Record
                    {
                        Description = task_RecordNewInput.Description.Value,
                        DayRecordId = task_RecordNewInput.DayRecordId.Value,

                    };
                    _context.Tasks.Update(newTask_Record);
                    await _context.SaveChangesAsync();

                    var successRequestResultMessage = new
                    {
                        Message = "Updated Task into database"
                    };

                    OkObjectResult httpOkResult =
                    new OkObjectResult(successRequestResultMessage);
                    return httpOkResult;

                }
                catch (Exception exceptionObject)
                {
                    //Create a fail message anonymous object
                    //This anonymous object only has one Message property 
                    //which contains a simple string message
                    object httpFailRequestResultMessage =
                    new { Message = exceptionObject };
                    //Return a bad http response message to the client
                    return BadRequest(httpFailRequestResultMessage);
                }
            }
            else
            {
                object httpFailRequestResultMessage =
                new { Message = "Task Record ID not found" };
                //Return a bad http response message to the client
                return BadRequest(httpFailRequestResultMessage);


            }//End of Get(id) Web API method
        }

        // POST: api/Task_Record
        [HttpPost("SaveNewTaskRecordInformation")]
        public async Task<IActionResult> SaveNewTaskRecordInformation([FromBody] string value)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            string customMessage = "";
            try
            {

                var task_RecordNewInput = JsonConvert.DeserializeObject<dynamic>(value);
                Task_Record newTask_Record = new Task_Record
                {
                    Description = task_RecordNewInput.Description.Value,
                    DayRecordId = task_RecordNewInput.DayRecordId.Value,

                };

                _context.Tasks.Add(newTask_Record);
                await _context.SaveChangesAsync();

            }
            catch (Exception exceptionObject)
            {
                customMessage = "Unable to save to database";
                //return 
            }
            var successRequestResultMessage = new
            {
                Message = "Saved Task into database"
            };

            OkObjectResult httpOkResult =
            new OkObjectResult(successRequestResultMessage);
            return httpOkResult;
        }

        // DELETE: api/Task_Record/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTask_Record([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var task_Record = await _context.Tasks.SingleOrDefaultAsync(m => m.TaskRecordId == id);
            if (task_Record == null)
            {
                return NotFound();
            }

            _context.Tasks.Remove(task_Record);
            await _context.SaveChangesAsync();

            return Ok(task_Record);
        }

        private bool Task_RecordExists(int id)
        {
            return _context.Tasks.Any(e => e.TaskRecordId == id);
        }
    }
}