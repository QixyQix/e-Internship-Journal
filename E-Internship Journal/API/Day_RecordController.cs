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
using Microsoft.AspNetCore.Authorization;

namespace E_Internship_Journal.API
{
    [Produces("application/json")]
    [Route("api/Day_Record")]
    public class Day_RecordController : Controller
    {
        private readonly ApplicationDbContext _context;

        public Day_RecordController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/Day_Record
        [HttpGet]
        public IActionResult GetDay_Records()
        {
            List<object> Day_Records_List = new List<object>();
            var Day_Records = _context.Day_Records
                .Include(eachDay => eachDay.Month)
                .Include(eachDay => eachDay.Tasks)
            .AsNoTracking();

            foreach (var oneDayRecord in Day_Records)
            {
                //   List<int> categoryIdList = new List<int>();
                Day_Records_List.Add(new
                {
                    DayId = oneDayRecord.DayId,
                    Date = oneDayRecord.Date,
                    ArrivalTime = oneDayRecord.ArrivalTime,
                    DepartureTime = oneDayRecord.DepartureTime,
                    WeekNo = oneDayRecord.WeekNo,
                    MC = oneDayRecord.MC,
                    MonthRecordId = oneDayRecord.MonthRecordId,

                    TaskDescription = oneDayRecord.Tasks.Select(eachItem => eachItem.Description).ToList<string>()

                });
            }

            return new JsonResult(Day_Records_List);
        }

        // GET: api/Day_Record/5
        [HttpGet("{id}")]
        public IActionResult GetDay_Record(int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            if (Day_RecordExists(id))
            {
                try
                {
                    var foundOneDay = _context.Day_Records
                        .Where(eachDayEntity => eachDayEntity.DayId == id)
                        .Include(eachDayEntity => eachDayEntity)
                        .Single();
                    var response = new
                    {
                        DayId = foundOneDay.DayId,
                        Date = foundOneDay.Date,
                        ArrivalTime = foundOneDay.ArrivalTime,
                        DepartureTime = foundOneDay.DepartureTime,
                        WeekNo = foundOneDay.WeekNo,
                        MC = foundOneDay.MC
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


            }//End of Get(id) Web API method
        }

        // PUT: api/Day_Record/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutDay_Record(int id, [FromBody] string value)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (Day_RecordExists(id))
            {
                var day_RecordNewInput = JsonConvert.DeserializeObject<dynamic>(value);
                try
                {
                    var foundOneDay = _context.Day_Records
                        .Where(eachDayEntity => eachDayEntity.DayId == id)
                        .Single();

                    Day_Record newDay_Record = new Day_Record
                    {
                        ArrivalTime = day_RecordNewInput.Description.Value,
                        DepartureTime = day_RecordNewInput.DayRecordId.Value,
                        CompanyOff = day_RecordNewInput.CompanyOff.Value,
                        Date = day_RecordNewInput.Date.Value,
                        MC = day_RecordNewInput.MC.Value,
                        WeekNo = day_RecordNewInput.WeekNo.Value,

                    };
                    _context.Day_Records.Update(newDay_Record);
                    await _context.SaveChangesAsync();

                    var successRequestResultMessage = new
                    {
                        Message = "Updated Day Record into database"
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
                new { Message = "Day Record ID not found" };
                //Return a bad http response message to the client
                return BadRequest(httpFailRequestResultMessage);


            }//End of Get(id) Web API method
        }

        // POST: api/Day_Record
        [HttpPost("SaveNewDayRecordInformation")]
        [AllowAnonymous]
        public async Task<IActionResult> SaveNewDayRecordInformation([FromBody] string value)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            string customMessage = "";
            try
            {
                var day_RecordNewInput = JsonConvert.DeserializeObject<dynamic>(value);

                int internshipRecordId = day_RecordNewInput.InternshipRecordId.Value;

                //Get the student's internship record
                var internshipRecord = _context.Internship_Records.Include(ir => ir.UserBatch.Batch)
                    .Include(ir => ir.MonthRecords.Select(mn => mn.DayRecords))
                    .SingleOrDefault(ir => ir.InternshipRecordId == internshipRecordId);

                DateTime date = DateTime.ParseExact(day_RecordNewInput.Date.Value, "d/M/yyyy", CultureInfo.InvariantCulture);

                Month_Record monthRecord = null;

                if (!(internshipRecord.MonthRecords.Count < 1))
                {
                    //Check if the date already exists
                    foreach (var month in internshipRecord.MonthRecords)
                    {
                        if (month.DayRecords.Count < 36)
                        {
                            monthRecord = month;
                            foreach (var dayRecord in month.DayRecords)
                            {
                                if (dayRecord.Date.Equals(date))
                                {
                                    return BadRequest();
                                }
                            }
                        }
                    }
                }
                else if(monthRecord == null)
                {
                    //Create new month record
                    monthRecord = new Month_Record
                    {
                        InternshipRecord = internshipRecord
                    };
                    _context.Month_Records.Add(monthRecord);
                }

                //TODO VALIDATION FOR ATTENDANCE
                string[] timeValArray = day_RecordNewInput.ArrivalTime.Value.ToString().Replace(" ","").Split(':');
                DateTime timeIn = date.AddHours(Int32.Parse(timeValArray[0])).AddMinutes(Int32.Parse(timeValArray[1]));
                timeValArray = day_RecordNewInput.DepartureTime.Value.ToString().Replace(" ", "").Split(':');
                DateTime timeOut = date.AddHours(Int32.Parse(timeValArray[0])).AddMinutes(Int32.Parse(timeValArray[1]));
                var mc = bool.Parse(day_RecordNewInput.MC.Value);
                var companyOff = bool.Parse(day_RecordNewInput.CompanyOff.Value);
                var taskRecords = day_RecordNewInput.Tasks;

                //Calculate the weekno

                DateTime startDate = internshipRecord.UserBatch.Batch.StartDate;

                int weekNo = (Int32.Parse((date - startDate).TotalDays.ToString()) /7 ) +1;

                var newDayRecord = new Day_Record
                {
                    Date = date,
                    ArrivalTime = timeIn,
                    DepartureTime = timeOut,
                    CompanyOff = companyOff,
                    MC = mc,
                    WeekNo = weekNo,
                    Month = monthRecord
                };

                List<Task_Record> tasks = new List<Task_Record>();
                for (int i = 0; i < taskRecords.Count; i++) {
                    var taskRecord = new Task_Record
                    {
                        Description = taskRecords[i].Value
                    };

                    tasks.Add(taskRecord);
                }

                newDayRecord.Tasks = tasks;

                _context.Day_Records.Add(newDayRecord);
                await _context.SaveChangesAsync();

            }
            catch (Exception exceptionObject)
            {
                customMessage = "Unable to save to database"; 
            }
            var successRequestResultMessage = new
            {
                Message = "Saved Day Record into database"
            };

            OkObjectResult httpOkResult =
            new OkObjectResult(successRequestResultMessage);
            return httpOkResult;
        }

        // DELETE: api/Day_Record/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteDay_Record([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var day_Record = await _context.Day_Records.SingleOrDefaultAsync(m => m.DayId == id);
            if (day_Record == null)
            {
                return NotFound();
            }

            _context.Day_Records.Remove(day_Record);
            await _context.SaveChangesAsync();

            return Ok(day_Record);
        }

        private bool Day_RecordExists(int id)
        {
            return _context.Day_Records.Any(e => e.DayId == id);
        }
    }
}