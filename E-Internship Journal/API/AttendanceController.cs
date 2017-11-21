using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using E_Internship_Journal.Data;
using Microsoft.EntityFrameworkCore;
using E_Internship_Journal.Models;
using Microsoft.AspNetCore.Identity;
using System.Globalization;

namespace E_Internship_Journal.API
{
    [Produces("application/json")]
    [Route("api/Attendance")]
    public class AttendanceController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        public AttendanceController(UserManager<ApplicationUser> userManager, ApplicationDbContext context)
        {
            _userManager = userManager;
            _context = context;
        }

        [HttpGet("GetAttendanceRecord")]
        public IActionResult GetAttendanceRecord()
        {
            try
            {
                var id = _userManager.GetUserId(User);
                var batchRecord = _context.UserBatches
                    .Where(br => br.UserId == id)
                    .Include(br => br.Batch)
                    .SingleOrDefault();



                //var internshipRecord = _context.Internship_Records
                //   // .Include(ir => ir.MonthRecords.Select(mn => mn.DayRecords))
                //   .Include(ir => ir.MonthRecords)
                //   .Include(ir => ir.MonthRecords.da)
                //    .SingleOrDefault(ir => ir.UserBatchId == batchRecord.UserBatchId);
                var test = _context.Month_Records
                    .Include(MR => MR.InternshipRecord)
                    .Include(MR => MR.DayRecords)
                    .SingleOrDefault(MR => MR.InternshipRecord.UserBatchId == batchRecord.UserBatchId);
                //.AsNoTracking();
                //If the internship record has month records
                List<object> response = new List<object>();
                //if (!(internshipRecord.MonthRecords.Count < 1))
                //{
                //    //Check if the date already exists
                //    foreach (var month in internshipRecord.MonthRecords)
                //    {
                //        foreach (var dayRecord in month.DayRecords)
                //        {
                //            response.Add(new {
                //                ArrivalTime = dayRecord.ArrivalTime,

                //            });

                //        }

                //    }

                //}
                //Check if the date already exists
                DateTime startDate = batchRecord.Batch.StartDate;

                DateTime date;
                //int days = DateTime.DaysInMonth(year, month);

                foreach (var dayRecord in test.DayRecords)
                {
                    //   date = DateTime.ParseExact(dayRecord.ArrivalTime.ToString(), "d/M/yyyy", CultureInfo.InvariantCulture);
                    var BusinessDay = BusinessDaysUntil(startDate, dayRecord.Date);
                    int days = DateTime.DaysInMonth(dayRecord.Date.Year, dayRecord.Date.Month);
                    var noDaysFromStartDate = Int32.Parse((dayRecord.Date - startDate).TotalDays.ToString());
                    var monthNo = Int32.Parse((noDaysFromStartDate / 28).ToString());
                    double testDecimal = BusinessDay / 5.0;
                    var weekNo = Int32.Parse((BusinessDay / 5).ToString());
                   var rounup =  Math.Ceiling(testDecimal);
                    if (rounup <= 4)
                    {
                        response.Add(new
                        {
                            BusinessDay,
                            days,
                            noDaysFromStartDate,
                            monthNo,
                            weekNo,
                            rounup,
                            dayRecord.Date,
                           dayRecord.ArrivalTime,
                           dayRecord.DepartureTime




                        });
                    }


                }




                return new JsonResult(response);


                //.SingleOrDefault(ir => ir.UserBatchId == batchRecord.)
                // .SingleOrDefault(ir => ir.InternshipRecordId == internshipRecordId);


            }
            catch (Exception ex)
            {

            }
            return NoContent();
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