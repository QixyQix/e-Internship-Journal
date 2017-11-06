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
                Day_Record newDay_Record = new Day_Record
                {
                    ArrivalTime = day_RecordNewInput.ArrivalTime.Value,
                    DepartureTime = day_RecordNewInput.DepartureTime.Value,
                    CompanyOff = day_RecordNewInput.CompanyOff.Value,
                    Date = day_RecordNewInput.Date.Value,
                    MC = day_RecordNewInput.MC.Value,
                    MonthRecordId = day_RecordNewInput.MonthRecordId.Value,
                    WeekNo = day_RecordNewInput.WeekNo.Value
                };

                _context.Day_Records.Add(newDay_Record);
                await _context.SaveChangesAsync();

            }
            catch (Exception exceptionObject)
            {
                customMessage = "Unable to save to database";
                //return 
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