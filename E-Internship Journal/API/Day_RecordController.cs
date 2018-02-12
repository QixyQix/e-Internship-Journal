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

        // GET: api/Day_Record
        [HttpGet("getInternshipDayRecords/{id}")]
        public IActionResult GetDay_Records(int id)
        {
            List<object> Day_Records_List = new List<object>();

            //var dayRecords = _context.Day_Records.Include(dr => dr.Month)
            //    .ThenInclude(mr => mr.InternshipRecord)
            //    .ThenInclude(ir => ir.UserBatch)
            //    .ToList();

            var internshipRecord = _context.Internship_Records
                .Where(ir => ir.InternshipRecordId == id)
                .Include(ir => ir.MonthRecords)
                .ThenInclude(mr => mr.DayRecords)
                .Include(ir => ir.UserBatch)
                .SingleOrDefault();

            //Latest month record
            Month_Record monthRecord = internshipRecord.MonthRecords[internshipRecord.MonthRecords.Count - 1];

            foreach (var dayRecord in monthRecord.DayRecords)
            {
                Day_Records_List.Add(new
                {
                    DayId = dayRecord.DayId,
                    Date = dayRecord.Date,
                    ArrivalTime = dayRecord.ArrivalTime,
                    DepartureTime = dayRecord.DepartureTime,
                    WeekNo = dayRecord.WeekNo,
                    Remarks = dayRecord.Remarks,
                    MonthRecordId = dayRecord.MonthRecordId
                });
            }
            return new JsonResult(Day_Records_List);
        }

        [HttpGet("getLatestInternshipMonthDayRecords/{id}")]
        public IActionResult GetLatestInternshipMonthDayRecords(int id) {
            List<object> Day_Records_List = new List<object>();

            var internshipRecord = _context.Internship_Records
                .Where(ir => ir.InternshipRecordId == id)
                .Include(ir => ir.MonthRecords)
                .ThenInclude(mr => mr.DayRecords)
                .SingleOrDefault();

            if (internshipRecord == null) {
                return new BadRequestObjectResult(new { Message = "Internship record does not exist" });
            }

            //Latest month record
            Month_Record monthRecord = internshipRecord.MonthRecords[internshipRecord.MonthRecords.Count-1];

            foreach (var dayRecord in monthRecord.DayRecords)
            {
                Day_Records_List.Add(new
                {
                    DayId = dayRecord.DayId,
                    Date = dayRecord.Date,
                    ArrivalTime = dayRecord.ArrivalTime,
                    DepartureTime = dayRecord.DepartureTime,
                    WeekNo = dayRecord.WeekNo,
                    Remarks = dayRecord.Remarks,
                    MonthRecordId = dayRecord.MonthRecordId
                });
            }
            return new JsonResult(Day_Records_List);
        }

        // GET: api/Day_Record
        [Authorize(Roles = "LO,STUDENT")]
        [HttpGet("getStudentDayRecords/{id}")]
        public IActionResult GetStudentDayRecords(int id)
        {
            List<object> Day_Records_List = new List<object>();
            var dayRecords = _context.Day_Records.Include(tr => tr.Month)
                .ThenInclude(tr => tr.InternshipRecord)
                .Where(tr => tr.Month.InternshipRecord.InternshipRecordId == id)
                .ToList();
            var internshipRecords = _context.Internship_Records.Include(ub => ub.UserBatch)
                .ThenInclude(b => b.Batch)
                .Where(tr => tr.InternshipRecordId == id).SingleOrDefault();
            var startBatch = internshipRecords.UserBatch.Batch.StartDate;
            var endBatch = internshipRecords.UserBatch.Batch.EndDate;
            //To be continued
            var totalDays = BusinessDaysUntil(startBatch, endBatch);
            foreach (var dayRecord in dayRecords)
            {
                Day_Records_List.Add(new
                {
                    dayRecord.DayId,
                    dayRecord.Date,
                    dayRecord.ArrivalTime,
                    dayRecord.DepartureTime,
                    dayRecord.WeekNo,
                    dayRecord.Remarks,
                    dayRecord.MonthRecordId
                });
            }

            return new JsonResult(Day_Records_List);
        }

        //GET: api/Day_Record/id
        [HttpGet("{id}")]
        public IActionResult GetDay_Record(int id)
        {
            Day_Record dayRecord = _context.Day_Records.Find(id);

            if (dayRecord != null)
            {
                var dayResultObj = new
                {
                    DayId = dayRecord.DayId,
                    Date = dayRecord.Date,
                    ArrivalTime = dayRecord.ArrivalTime,
                    DepartureTime = dayRecord.DepartureTime,
                    WeekNo = dayRecord.WeekNo,
                    Remarks = dayRecord.Remarks,
                    MonthRecordId = dayRecord.MonthRecordId
                };
                return new JsonResult(dayResultObj);
            }
            else
            {
                return NotFound(new { Message = "Day record not found" });
            }
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "STUDENT")]
        public IActionResult DeleteDay_Record(int id) {
            Day_Record dayRecord = _context.Day_Records.Where(dr => dr.DayId == id)
                .Include(dr => dr.Month)
                .ThenInclude(mr => mr.InternshipRecord)
                .ThenInclude(ir => ir.UserBatch)
                .ThenInclude(ub => ub.User)
                .SingleOrDefault();

            if (dayRecord == null)
            {
                return new BadRequestObjectResult(new { Message = "Day record does not exist!" });
            }
            else if (dayRecord.Month.Approved == true)
            {
                return new BadRequestObjectResult(new { Message = "The month this day record belongs to has already been approved and cannot be edited" });
            }
            else if (!dayRecord.Month.InternshipRecord.UserBatch.User.Id.Equals(_userManager.GetUserId(User))) {
                return new BadRequestObjectResult(new { Message = "This day record does not belong to you!" });
            }

            _context.Day_Records.Remove(dayRecord);
            _context.SaveChanges();

            return new OkObjectResult(new { Message = "Day record deleted successfully!" });
        }

        //// GET: api/Day_Record/09-11-2017
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

        //        var response = new
        //        {
        //            DayId = dayId,
        //            Date = dayDate,
        //            ArrivalTime = arrivalTime,
        //            DepartureTime = departureTime,
        //            Remarks = remarks
        //        };
        //        return new JsonResult(response);
        //    }
        //    else
        //    {
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
        [HttpPut("SaveUpdateDayRecord")]
        [AllowAnonymous]
        public async Task<IActionResult> SaveUpdateDayRecord([FromBody] string value)
        {
            string customMessage = "";
            try
            {
                var day_RecordNewInput = JsonConvert.DeserializeObject<dynamic>(value);

                //Get the student's internship record
                var internshipRecord = _context.Internship_Records
                    .Include(ir => ir.UserBatch.Batch)
                    .Where(ir => DateTime.Now >= ir.UserBatch.Batch.StartDate && DateTime.Now <= ir.UserBatch.Batch.EndDate)
                    .Where(ir => ir.UserBatch.User.Id.Equals(_userManager.GetUserId(User)))
                    .Include(ir => ir.MonthRecords)
                    .ThenInclude(mn => mn.DayRecords)
                    .Single();

                //Check if intern record exists
                if (internshipRecord == null)
                {
                    return NotFound();
                }
                //Check if the internship record belongs to the user
                if (!(internshipRecord.UserBatch.UserId.Equals(_userManager.GetUserId(User))))
                {
                    return BadRequest(new { Message = "Internship record does not belong to you" });
                }

                DateTime date = DateTime.Parse(day_RecordNewInput.Date.Value, CultureInfo.InvariantCulture);
                DateTime startDate = internshipRecord.UserBatch.Batch.StartDate;
                var noDaysFromStartDate = (date - startDate).Days;
                Month_Record monthRecord = null;

                //If the internship record has month records
                if (!(internshipRecord.MonthRecords.Count < 1))
                {
                    //Check if the date already exists
                    foreach (var month in internshipRecord.MonthRecords)
                    {
                        foreach (var aDayRecord in month.DayRecords)
                        {
                            if (aDayRecord.Date.Equals(date) && !aDayRecord.DayId.ToString().Equals(day_RecordNewInput.Id.Value.ToString()))
                            {
                                return BadRequest(new { Message = "Day record for " + date.ToString("dd/MM/yyyy") + " already exists!" });
                            }
                        }

                    }
                }
                //Calculate the weekno
                int weekNo = (Int32.Parse(date.Subtract(startDate).Days.ToString()) / 7) + 1;
                //Calculate month number
                var monthNo = Int32.Parse((weekNo / 4).ToString());

                if (monthNo < internshipRecord.MonthRecords.Count)//If month number is less than the count (meaning it exists) 
                {
                    monthRecord = internshipRecord.MonthRecords[monthNo];
                    if (monthRecord.Approved == true)
                    {
                        return BadRequest(new { Message = "This month's records have already been approved and cannot be edited" });
                    }
                }
                else//Else the month does not exist and create a new month record.
                {
                    monthRecord = new Month_Record
                    {
                        InternshipRecord = internshipRecord
                    };
                    _context.Month_Records.Add(monthRecord);
                }

                string arrivalTimeStr = "";
                string departTimeStr = "";
                if (day_RecordNewInput.ArrivalTime != null && day_RecordNewInput.DepartureTime != null)
                {
                    arrivalTimeStr = day_RecordNewInput.ArrivalTime.Value.ToString();
                    departTimeStr = day_RecordNewInput.DepartureTime.Value.ToString();
                }
                var timeIn = new DateTime();
                var timeOut = new DateTime();
                bool isPresent = false;
                string remarks = day_RecordNewInput.Remarks.Value.ToString().Trim();

                Day_Record dayRecord = null;

                if (day_RecordNewInput.Id.Value.Equals("", StringComparison.OrdinalIgnoreCase))
                {//No Id, create record
                    dayRecord = new Day_Record
                    {
                        Date = date,
                        WeekNo = weekNo,
                        Month = monthRecord
                    };
                    _context.Day_Records.Add(dayRecord);
                }
                else
                {//Get the day record
                    int dayId = Int32.Parse(day_RecordNewInput.Id.Value);
                    dayRecord = _context.Day_Records
                        .Include(dr => dr.Month)
                        .ThenInclude(mr => mr.InternshipRecord)
                        .ThenInclude(ir => ir.UserBatch)
                        .Where(dr => dr.DayId == dayId)
                        .Single();

                    if (!dayRecord.Month.InternshipRecord.UserBatch.UserId.Equals(_userManager.GetUserId(User)))
                    {
                        return BadRequest(new { Message = "Day record does not belong to you" });
                    }

                    dayRecord.Date = date;
                    dayRecord.WeekNo = weekNo;
                    dayRecord.Month = monthRecord;
                }


                if (remarks.Equals(""))
                {
                    try
                    {
                        timeIn = DateTime.ParseExact(arrivalTimeStr, "d/M/yyyy H:m:s", CultureInfo.InvariantCulture);
                        timeOut = DateTime.ParseExact(departTimeStr, "d/M/yyyy H:m:s", CultureInfo.InvariantCulture);
                    }
                    catch (Exception e)
                    {
                        return BadRequest(new { Message = "Incorrect timing" });
                    }
                    dayRecord.ArrivalTime = timeIn;
                    dayRecord.DepartureTime = timeOut;
                    dayRecord.IsPresent = true;
                    dayRecord.Remarks = "";
                }
                else
                {
                    dayRecord.ArrivalTime = null;
                    dayRecord.DepartureTime = null;
                    dayRecord.IsPresent = false;
                    if (remarks.Equals("OTHERS"))
                    {
                        dayRecord.Remarks = day_RecordNewInput.Specify.Value;
                    }
                    else
                    {
                        dayRecord.Remarks = remarks;
                    }
                }

                await _context.SaveChangesAsync();

            }
            catch (Exception exceptionObject)
            {
                //return BadRequest(new { Message = exceptionObject.ToString() });
                return BadRequest(new { Message = "Error occured. Please try again." });
            }

            return new OkObjectResult(new { Message = "Saved day record successfully." });
        }

        [HttpPut("updateComment/{id}")]
        [Authorize(Roles = "SUPERVISOR")]
        public IActionResult updateComment(int id, [FromBody] string value)
        {
            Day_Record dayRecord = _context.Day_Records.Where(dr => dr.DayId == id)
                .Include(dr => dr.Month)
                .ThenInclude(mr => mr.InternshipRecord)
                .ThenInclude(ir => ir.Project)
                .SingleOrDefault();

            if (dayRecord == null)
            {
                return new BadRequestObjectResult(new { Message = "Day Record does not exist!" });
            }
            else if (!dayRecord.Month.InternshipRecord.Project.SupervisorId.Equals(_userManager.GetUserId(User)))
            {
                return new BadRequestObjectResult(new { Message = "You are not authorized to perform ths action." });
            }

            var commentInput = JsonConvert.DeserializeObject<dynamic>(value);

            dayRecord.SupervisorRemarks = commentInput.Comment.Value;

            _context.SaveChanges();

            return new OkObjectResult(new { Message = "Comment edited successfully!" });
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
        private int BusinessDaysUntil(DateTime firstDay, DateTime lastDay, params DateTime[] bankHolidays)
        {
            //REFERENCE FROM https://stackoverflow.com/questions/1617049/calculate-the-number-of-business-days-between-two-dates
            firstDay = firstDay.Date;
            lastDay = lastDay.Date;
            if (firstDay > lastDay)
                throw new ArgumentException("Incorrect last day " + lastDay);

            TimeSpan span = lastDay - firstDay;
            int businessDays = span.Days + 1;
            int fullWeekCount = businessDays / 7;
            // find out if there are weekends during the time exceedng the full weeks
            if (businessDays > fullWeekCount * 7)
            {
                // we are here to find out if there is a 1-day or 2-days weekend
                // in the time interval remaining after subtracting the complete weeks
                //int firstDayOfWeek = (int)firstDay.DayOfWeek;
                //int lastDayOfWeek = (int)lastDay.DayOfWeek;
                int firstDayOfWeek = firstDay.DayOfWeek == DayOfWeek.Sunday ? 7 : (int)firstDay.DayOfWeek;
                int lastDayOfWeek = lastDay.DayOfWeek == DayOfWeek.Sunday ? 7 : (int)lastDay.DayOfWeek;
                if (lastDayOfWeek < firstDayOfWeek)
                    lastDayOfWeek += 7;
                if (firstDayOfWeek <= 6)
                {
                    if (lastDayOfWeek >= 7)// Both Saturday and Sunday are in the remaining time interval
                        businessDays -= 2;
                    else if (lastDayOfWeek >= 6)// Only Saturday is in the remaining time interval
                        businessDays -= 1;
                }
                else if (firstDayOfWeek <= 7 && lastDayOfWeek >= 7)// Only Sunday is in the remaining time interval
                    businessDays -= 1;
            }

            // subtract the weekends during the full weeks in the interval
            businessDays -= fullWeekCount + fullWeekCount;

            // subtract the number of bank holidays during the time interval
            //foreach (DateTime bankHoliday in bankHolidays)
            //{
            //    DateTime bh = bankHoliday.Date;
            //    if (firstDay <= bh && bh <= lastDay)
            //        --businessDays;
            //}

            return businessDays;
        }
    }
}