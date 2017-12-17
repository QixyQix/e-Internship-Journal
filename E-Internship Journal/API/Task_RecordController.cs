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
using System.Globalization;
using Microsoft.AspNetCore.Identity;

namespace E_Internship_Journal.API
{
    [Produces("application/json")]
    [Route("api/Task_Record")]
    public class Task_RecordController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public Task_RecordController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        //// GET: api/Task_Record
        //[HttpGet]
        //public IActionResult GetTasks()
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return BadRequest(ModelState);
        //    }
        //    List<object> task_Record_List = new List<object>();
        //    var task_Records = _context.Tasks
        //        .Include(eachTaskEntity =>eachTaskEntity.DayRecord)

        //    .AsNoTracking();
        //    foreach (var oneTaskRecord in task_Records)
        //    {
        //        //   List<int> categoryIdList = new List<int>();
        //        task_Record_List.Add(new
        //        {
        //            oneTaskRecord.TaskRecordId,
        //            oneTaskRecord.DayRecordId,
        //            oneTaskRecord.Description,

        //           // oneTaskRecord.DayRecord.Date
        //        });
        //    }

        //    return new JsonResult(task_Record_List);
        //}

        //GET: api/Task_Record/getTaskRecordsUnderInternRecord
        [HttpGet("getTaskRecordsUnderInternRecord/{id}")]
        public async Task<IActionResult> getTaskRecordsUnderInternRecord(int id)
        {
            List<object> taskRecordObjs = new List<object>();

            var taskRecords = _context.Tasks.Include(tr => tr.MonthRecord)
                .ThenInclude(tr => tr.InternshipRecord)
                .Where(tr => tr.MonthRecord.InternshipRecord.InternshipRecordId == id)
                .ToList();

            foreach (var task in taskRecords)
            {
                taskRecordObjs.Add(new
                {
                    Date = task.Date,
                    Description = task.Description,
                    TaskRecordId = task.TaskRecordId,
                    TaskRemarks = task.Remarks,
                    WeekNo = task.WeekNo

                });
            }

            return new JsonResult(taskRecordObjs);
        }

        [HttpGet("getTasksForDay/{internrecordId}/{date}")]
        public async Task<IActionResult> getTasksForDay(int internrecordId, string date)
        {
            DateTime dateDT = DateTime.ParseExact(date, "dd-MM-yyyy", CultureInfo.InvariantCulture);

            var tasks = _context.Tasks.Include(tr => tr.MonthRecord)
                .ThenInclude(mr => mr.InternshipRecord)
                .ThenInclude(ir => ir.UserBatch)
                .Where(tr => tr.Date == dateDT && tr.MonthRecord.InternshipRecord.UserBatch.UserId.Equals(_userManager.GetUserId(User)))
                .ToList();

            List<object> tasksList = new List<object>();

            foreach (var task in tasks)
            {
                tasksList.Add(new
                {
                    Id = task.TaskRecordId,
                    Description = task.Description
                });
            }

            return new JsonResult(tasksList);
        }

        //PUT: Update Tasks for day
        [HttpPut("updateTasksForDay")]
        public IActionResult updateTasksForDay([FromBody] string value)
        {

            var taskInput = JsonConvert.DeserializeObject<dynamic>(value);

            int internshipRecordId = Int32.Parse(taskInput.InternshipRecordId.Value);

            Internship_Record internshipRecord = _context.Internship_Records.Where(ir => ir.InternshipRecordId == internshipRecordId)
                .Include(ir => ir.UserBatch)
                .ThenInclude(ub => ub.Batch)
                .Include(ir => ir.MonthRecords)
                .ThenInclude(mr => mr.TaskRecords)
                .Where(ir => ir.UserBatch.UserId.Equals(_userManager.GetUserId(User)))
                .Single();

            if (internshipRecord == null)
            {
                return BadRequest(new { Message = "This internship record does not belong to you." });
            }
            DateTime date = DateTime.ParseExact(taskInput.Date.Value, "dd/MM/yyyy", CultureInfo.InvariantCulture);

            //Calculate number of weeks
            var weekNo = Int32.Parse((date.Subtract(internshipRecord.UserBatch.Batch.StartDate).Days / 7).ToString()) + 1;
            //Calculate month
            var monthNo = Int32.Parse((weekNo / 4).ToString()) + 1;

            Month_Record monthRecord;

            //No month record yet
            if (monthNo > internshipRecord.MonthRecords.Count)
            {
                monthRecord = new Month_Record { InternshipRecord = internshipRecord };
                _context.Add(monthRecord);
            }
            else //Month record already exists
            {
                monthRecord = internshipRecord.MonthRecords[monthNo - 1];
            }

            var taskObjs = taskInput.Tasks;
            foreach (var task in taskObjs)
            {
                string taskId = "" + task.Id.Value;
                if (string.IsNullOrWhiteSpace(taskId))
                { //No Id, task doesnt exist yet
                    Task_Record newTask = new Task_Record
                    {
                        Date = date,
                        Description = task.Description.Value,
                        MonthRecord = monthRecord
                    };

                    _context.Tasks.Add(newTask);
                }
                else //Update the task
                {
                    int taskIdInt = Int32.Parse(taskId);
                    Task_Record taskToUpdate = _context.Tasks.Find(taskIdInt);
                    taskToUpdate.Date = date;
                    taskToUpdate.Description = task.Description.Value;
                    taskToUpdate.MonthRecord = monthRecord;
                    _context.Update(taskToUpdate);
                }
            }
            _context.SaveChanges();

            return new OkObjectResult(new { Message = "Records saved successfully!" });
        }

        // GET: api/Task_Record/5
        //[HttpGet("{id}")]
        //public async Task<IActionResult> GetTask_Record(int id)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return BadRequest(ModelState);
        //    }
        //    if (Task_RecordExists(id))
        //    {
        //        try
        //        {
        //            List<object> TaskList = new List<object>();
        //            var foundTasks = _context.Tasks
        //                .Where(eachTaskEntity => eachTaskEntity.DayRecordId == id)
        //                .Include(eachTaskENtity => eachTaskENtity.DayRecord)
        //                .AsNoTracking();
        //            foreach (var oneTask in foundTasks)
        //            {
        //                TaskList.Add(new
        //                {
        //                    DayRecordId = oneTask.DayRecordId,
        //                    TaskRecordId = oneTask.TaskRecordId,
        //                    Description = oneTask.Description,
        //                    Date = oneTask.DayRecord.Date,
        //                    ArrivalTime = oneTask.DayRecord.ArrivalTime,
        //                    DepartureTime = oneTask.DayRecord.DepartureTime
        //                    //WeekNo = oneTask.DayRecord.WeekNo

        //                });

        //            };//end of creation of the response object
        //            return new JsonResult(TaskList);
        //        }
        //        catch (Exception exceptionObject)
        //        {
        //            //Create a fail message anonymous object
        //            //This anonymous object only has one Message property 
        //            //which contains a simple string message
        //            object httpFailRequestResultMessage =
        //            new { Message = "Unable to obtain brand information." };
        //            //Return a bad http response message to the client
        //            return BadRequest(httpFailRequestResultMessage);
        //        }
        //    }
        //    else
        //    {
        //        object httpFailRequestResultMessage =
        //        new { Message = "Unable to obtain brand information." };
        //        //Return a bad http response message to the client
        //        return BadRequest(httpFailRequestResultMessage);


        //    }//End of Get(id) Web API method
        //}

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
                        .Where(eachTaskEntity => eachTaskEntity.MonthRecordId == id)
                        .Single();

                    Task_Record newTask_Record = new Task_Record
                    {
                        Description = task_RecordNewInput.Description.Value,
                        MonthRecordId = task_RecordNewInput.MonthRecord.Value,

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

        //POST: api/Task_Record
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
                    MonthRecordId = task_RecordNewInput.MonthRecord.Value,

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

            var taskRecord = await _context.Tasks
                .Include(task => task.MonthRecord)
                .ThenInclude(mr => mr.InternshipRecord)
                .ThenInclude(ir => ir.UserBatch)
                .SingleOrDefaultAsync(m => m.TaskRecordId == id);

            if (taskRecord == null)
            {
                return NotFound(new { Message = "Task record does not exist!" });
            }
            else if (taskRecord.MonthRecord.InternshipRecord.UserBatch.UserId.Equals(_userManager.GetUserId(User)))
            {
                _context.Tasks.Remove(taskRecord);
            }
            else
            {
                return BadRequest(new { Message = "This task record does not belong to you!" });
            }


            await _context.SaveChangesAsync();

            return new OkObjectResult(new { Message = "Task record is deleted!" });
        }

        private bool Task_RecordExists(int id)
        {
            return _context.Tasks.Any(e => e.TaskRecordId == id);
        }
    }
}