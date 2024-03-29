using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using E_Internship_Journal.Data;
using E_Internship_Journal.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace E_Internship_Journal.API
{
    [Produces("application/json")]
    [Route("api/TouchPoint")]
    [Authorize]
    public class TouchPointController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public TouchPointController(UserManager<ApplicationUser> userManager, ApplicationDbContext context)
        {
            _userManager = userManager;
            _context = context;
        }
        [HttpGet("getInternshipTouchPointRecords/{id}")]
        [Authorize(Roles = "LO,STUDENT")]
        public IActionResult Get_TouchPoint_Records(int id)
        {
            List<object> TouchPoint_Records_List = new List<object>();


            var touchPointRecord = _context.TouchPointRecords
                .Include(ir => ir.InternshipRecord)
                .Where(ir => ir.InternshipRecord.InternshipRecordId == id).ToList();

            foreach (var touchRecord in touchPointRecord)
            {
                TouchPoint_Records_List.Add(new
                {
                    touchRecord.TouchPointId,
                    touchRecord.Comments,
                    touchRecord.TouchPointDate
                });
            }
            return new JsonResult(TouchPoint_Records_List);
        }
        [HttpPost("SaveTouchPointRecord/{id}")]
        [Authorize(Roles = "LO")]
        public async Task<IActionResult> SaveTouchPointRecordAsync(int id, [FromBody] string value)
        {
            var touchPointNewInput = JsonConvert.DeserializeObject<dynamic>(value);
            try
            {
                DateTime date = touchPointNewInput.TouchPointDate.Value.ToLocalTime();
                //DateTime date = DateTime.ParseExact(touchPointNewInput.TouchPointDate.Value, "d/M/yyyy H:m:s", CultureInfo.InvariantCulture);

                //DateTime date = DateTime.Parse(touchPointNewInput.TouchPointDate.Value, CultureInfo.InvariantCulture);
                TouchPoint_Record newTouchPoint = new TouchPoint_Record
                {
                    InternshipRecordId = id,
                    TouchPointDate = date,
                    Comments = touchPointNewInput.Comments
                };
                _context.TouchPointRecords.Add(newTouchPoint);
                await _context.SaveChangesAsync();

                var successRequestResultMessage = new
                {
                    Message = "Saved TouchPoint into database"
                };
                OkObjectResult httpOkResult =
                new OkObjectResult(successRequestResultMessage);
                return httpOkResult;
            }
            catch (Exception exceptionObject)
            {
                return BadRequest(new { Message = exceptionObject.Message });
            }

        }//End of Post

        [HttpPut("UpdateTouchPointRecord/{id}")]
        [Authorize(Roles = "LO")]
        public async Task<IActionResult> UpdateTouchPointRecordAsync(int id, [FromBody] string value)
        {
            var touchPointNewInput = JsonConvert.DeserializeObject<dynamic>(value);
            var touchPointRecord = _context.TouchPointRecords
                .Where(tr => tr.TouchPointId == id).SingleOrDefault();


            //var touchPointRecord = _context.TouchPointRecords
            //    .Include(ir => ir.InternshipRecord)
            //    .Where(ir => ir.InternshipRecord.InternshipRecordId == id).SingleOrDefault();
            try
            {
                DateTime date = touchPointNewInput.TouchPointDate.Value.ToLocalTime();
                touchPointRecord.TouchPointDate = date;
                touchPointRecord.Comments = touchPointNewInput.Comments;

                _context.SaveChanges();
                var successRequestResultMessage = new
                {
                    Message = "Saved TouchPoint into database"
                };
                OkObjectResult httpOkResult =
                new OkObjectResult(successRequestResultMessage);
                return httpOkResult;
            }
            catch (Exception exceptionObject)
            {
                return BadRequest(new { exceptionObject.Message });
            }



        }//End of Http Put

        [HttpDelete("DeleteTouchPointRecord/{id}")]
        [Authorize(Roles = "LO")]
        public async Task<IActionResult> DeleteTouchPointRecordAsync(int id)
        {
            try
            {
                var foundOneTouchPointRecord = _context.TouchPointRecords
                .Single(eachTouchPointRecord => eachTouchPointRecord.TouchPointId == id);
                //foundOneCategory.DeletedAt = DateTime.Now;

                _context.TouchPointRecords.Remove(foundOneTouchPointRecord);
                //   Database.Remove(foundOneCategory);
                _context.SaveChanges();
                var successRequestResultMessage = new
                {
                    Message = "Deleted TouchPoint from database"
                };
                OkObjectResult httpOkResult =
                new OkObjectResult(successRequestResultMessage);
                return httpOkResult;
            }
            catch (Exception exceptionObject)
            {
                return BadRequest(new { exceptionObject.Message });
            }

        }
    }
}