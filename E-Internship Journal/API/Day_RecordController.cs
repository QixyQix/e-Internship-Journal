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
using Microsoft.AspNetCore.Identity;

namespace E_Internship_Journal.API
{
    [Produces("application/json")]
    [Route("api/Day_Record")]
    public class Day_RecordController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public Day_RecordController(UserManager<ApplicationUser> userManager, ApplicationDbContext context)
        {
            _userManager = userManager;
            _context = context;
        }

        //// GET: api/Day_Record
        //[HttpGet]
        //public IActionResult GetDay_Records()
        //{
        //    List<object> Day_Records_List = new List<object>();
        //    var Day_Records = _context.Day_Records
        //        .Include(eachDay => eachDay.Month)
        //        .Include(eachDay => eachDay.Tasks)
        //    .AsNoTracking();

        //    foreach (var oneDayRecord in Day_Records)
        //    {
        //        //   List<int> categoryIdList = new List<int>();
        //        Day_Records_List.Add(new
        //        {
        //            DayId = oneDayRecord.DayId,
        //            Date = oneDayRecord.Date,
        //            ArrivalTime = oneDayRecord.ArrivalTime,
        //            DepartureTime = oneDayRecord.DepartureTime,
        //            WeekNo = oneDayRecord.WeekNo,
        //            Remarks = oneDayRecord.Remarks,
        //            MonthRecordId = oneDayRecord.MonthRecordId,

        //            TaskDescription = oneDayRecord.Tasks.Select(eachItem => eachItem.Description).ToList<string>()

        //        });
        //    }

        //    return new JsonResult(Day_Records_List);
        //}

        //// GET: api/Day_Record/5
        //[HttpGet("{date}")]
        //public IActionResult GetDay_Record(string date)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return BadRequest(ModelState);
        //    }

        //    Day_Record day = getUserDayRecord(date);
        //    if (day != null)
        //    {
        //        var dayId = day.DayId;
        //        var dayDate = day.Date;
        //        var arrivalTime = day.ArrivalTime;
        //        var departureTime = day.DepartureTime;
        //        var remarks = day.Remarks;
        //        var tasks = new List<object>();

        //        foreach (var task in day.Tasks) {
        //            tasks.Add(new
        //            {
        //                Id = task.TaskRecordId,
        //                Description = task.Description
        //            });
        //        }

        //        var response = new
        //        {
        //            DayId = dayId,
        //            Date = dayDate,
        //            ArrivalTime = arrivalTime,
        //            DepartureTime = departureTime,
        //            Remarks = remarks,
        //            Tasks = tasks,
        //        };
        //        return new JsonResult(response); 
        //    }
        //    else {
        //        return BadRequest();
        //    }
        //}

        //// PUT: api/Day_Record/5
        //[HttpPut("{id}")]
        //public async Task<IActionResult> PutDay_Record(int id, [FromBody] string value)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return BadRequest(ModelState);
        //    }

        //    if (Day_RecordExists(id))
        //    {
        //        var day_RecordNewInput = JsonConvert.DeserializeObject<dynamic>(value);
        //        try
        //        {
        //            var foundOneDay = _context.Day_Records
        //                .Where(eachDayEntity => eachDayEntity.DayId == id)
        //                .Single();

        //            Day_Record newDay_Record = new Day_Record
        //            {
        //                ArrivalTime = day_RecordNewInput.Description.Value,
        //                DepartureTime = day_RecordNewInput.DayRecordId.Value,
        //                Date = day_RecordNewInput.Date.Value,
        //                Remarks = day_RecordNewInput.Remarks.Value,
        //                WeekNo = day_RecordNewInput.WeekNo.Value,

        //            };
        //            _context.Day_Records.Update(newDay_Record);
        //            await _context.SaveChangesAsync();

        //            var successRequestResultMessage = new
        //            {
        //                Message = "Updated Day Record into database"
        //            };

        //            OkObjectResult httpOkResult =
        //            new OkObjectResult(successRequestResultMessage);
        //            return httpOkResult;

        //        }
        //        catch (Exception exceptionObject)
        //        {
        //            //Create a fail message anonymous object
        //            //This anonymous object only has one Message property 
        //            //which contains a simple string message
        //            object httpFailRequestResultMessage =
        //            new { Message = exceptionObject };
        //            //Return a bad http response message to the client
        //            return BadRequest(httpFailRequestResultMessage);
        //        }
        //    }
        //    else
        //    {
        //        object httpFailRequestResultMessage =
        //        new { Message = "Day Record ID not found" };
        //        //Return a bad http response message to the client
        //        return BadRequest(httpFailRequestResultMessage);


        //    }//End of Get(id) Web API method
        //}

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

                //int internshipRecordId = day_RecordNewInput.InternshipRecordId.Value;

                //Get the student's internship record
                var internshipRecord = _context.Internship_Records
                    .Include(ir => ir.UserBatch.Batch)
                    .Where(ir => DateTime.Now >= ir.UserBatch.Batch.StartDate && DateTime.Now <= ir.UserBatch.Batch.EndDate)
                    .Where(ir => ir.UserBatch.User.Id.Equals(_userManager.GetUserId(User)))
                    .Include(ir => ir.MonthRecords)
                    .ThenInclude(mn => mn.DayRecords)
                    .Single();

                ////Check if the internship record belongs to the user
                //if (!(internshipRecord.UserBatch.UserId.Equals(_userManager.GetUserId(User))))
                //{
                //    return BadRequest();
                //}

                if (internshipRecord == null)
                {
                    return NotFound();
                }

                DateTime date = DateTime.ParseExact(day_RecordNewInput.Date.Value, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                DateTime startDate = internshipRecord.UserBatch.Batch.StartDate;

                var noDaysFromStartDate = (date - startDate).Days;

                Month_Record monthRecord = null;

                //If the internship record has month records
                if (!(internshipRecord.MonthRecords.Count < 1))
                {
                    //Check if the date already exists
                    foreach (var month in internshipRecord.MonthRecords)
                    {
                        foreach (var dayRecord in month.DayRecords)
                        {
                            if (dayRecord.Date.Equals(date))
                            {
                                return BadRequest();
                            }
                        }

                    }
                }
                //Determine which month object
                var monthNo = Int32.Parse((noDaysFromStartDate / 28).ToString());
                if (monthNo < internshipRecord.MonthRecords.Count)//If month number is less than the count (meaning it exists) 
                {
                    monthRecord = internshipRecord.MonthRecords[monthNo];
                }
                else//Else the month does not exist and create a new month record.
                {
                    monthRecord = new Month_Record
                    {
                        InternshipRecord = internshipRecord
                    };
                    _context.Month_Records.Add(monthRecord);
                }

                string arrivalTimeStr = day_RecordNewInput.ArrivalTime.Value.ToString();
                string departTimeStr = day_RecordNewInput.DepartureTime.Value.ToString();
                var timeIn = new DateTime();
                var timeOut = new DateTime();
                bool isPresent = false;
                string remarks = day_RecordNewInput.Remarks.Value.ToString().Trim();
                if (!string.IsNullOrEmpty(arrivalTimeStr) && string.IsNullOrWhiteSpace(remarks))
                {
                    timeIn = DateTime.Parse(arrivalTimeStr, CultureInfo.InvariantCulture);
                    timeOut = DateTime.Parse(departTimeStr, CultureInfo.InvariantCulture);
                    isPresent = true;
                }
                else
                {
                    return BadRequest("Time In and Time Out must not have any values if absent");
                }

                //var taskRecords = day_RecordNewInput.Tasks;


                //Calculate the weekno

                int weekNo = (Int32.Parse(date.Subtract(startDate).Days.ToString()) / 7) + 1;

                var newDayRecord = new Day_Record
                {
                    Date = date,
                    IsPresent = isPresent,
                    Remarks = remarks,
                    WeekNo = weekNo,
                    Month = monthRecord
                };

                if (isPresent)
                {
                    newDayRecord.ArrivalTime = timeIn;
                    newDayRecord.DepartureTime = timeOut;
                    newDayRecord.Remarks = "";

                    //List<Task_Record> tasks = new List<Task_Record>();
                    //for (int i = 0; i < taskRecords.Count; i++)
                    //{
                    //    var taskRecord = new Task_Record
                    //    {
                    //        Description = taskRecords[i].Value
                    //    };

                    //    tasks.Add(taskRecord);
                    //}

                    //newDayRecord.Tasks = tasks;
                }

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

        //DELETE: api/Day_Record/5
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

        private Day_Record getUserDayRecord(string dateStr)
        {

            DateTime date = DateTime.ParseExact(dateStr, "dd-MM-yyyy", CultureInfo.InvariantCulture);
            Day_Record dayRecord = new Day_Record();

            dayRecord = _context.Day_Records.Where(dr => dr.Date.Equals(date))
                .Include(dr => dr.Month)
                .ThenInclude(mn => mn.InternshipRecord)
                .ThenInclude(ir => ir.UserBatch)
                .ThenInclude(ub => ub.User)
                .Where(dr => dr.Month.InternshipRecord.UserBatch.User.Id.Equals(_userManager.GetUserId(User)))
                .AsNoTracking()
                .Single();

            return dayRecord;
        }
    }
}