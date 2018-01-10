using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using E_Internship_Journal.Data;
using Microsoft.AspNetCore.Identity;
using E_Internship_Journal.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using Newtonsoft.Json;
using System.Globalization;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace E_Internship_Journal.API
{
    [Produces("application/json")]
    [Route("api/month_record")]
    public class Month_RecordController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        public Month_RecordController(UserManager<ApplicationUser> userManager, ApplicationDbContext context)
        {
            _userManager = userManager;
            _context = context;
        }

        // GET: api/values
        [HttpGet("InternshipMonthRecords/{id}")]
        public IActionResult GetInternshipMonthRecords(int id)
        {
            var internshipRecord = _context.Internship_Records
                .Where(ir => ir.InternshipRecordId == id)
                .Include(ir => ir.MonthRecords)
                .SingleOrDefault();

            if (internshipRecord == null)
            {
                return NotFound();
            }

            List<object> monthRecordObjList = new List<object>();

            foreach (var monthRecord in internshipRecord.MonthRecords)
            {
                monthRecordObjList.Add(new
                {
                    MonthId = monthRecord.MonthId,
                    Approved = monthRecord.Approved,
                    SoftSkillsCompetencyDoneWell = monthRecord.SoftSkillsCompetencyDoneWell,
                    SoftSkillsCompetencyImprove = monthRecord.SoftSkillsCompetencyImprove,
                    TechnicalCompetencyApplied = monthRecord.TechnicalCompetencyApplied,
                    TechnicalCompetencyDoneWell = monthRecord.TechnicalCompetencyDoneWell,
                    TechnicalCompetencyImprove = monthRecord.TechnicalCompetencyImprove,
                    MentorSessionDateTimeStart = monthRecord.MentorSessionDateTimeStart,
                    MentorSessionDateTimeEnd = monthRecord.MentorSessionDateTimeEnd,
                    MentorSessionReflection = monthRecord.MentorSessionReflection,
                    CommunicationGrading = monthRecord.CommunicationGrading,
                    TechnicalGrading = monthRecord.TechnicalGrading,
                    IndependenceGrading = monthRecord.IndependenceGrading,
                    PerformanceGrading = monthRecord.PerformanceGrading,
                    OverallGrading = monthRecord.OverallGrading,
                    OverallFeedback = monthRecord.OverallFeedback
                });
            }

            return new OkObjectResult(monthRecordObjList);
        }

        // GET Latest month record
        [HttpGet("Latest")]
        [Authorize(Roles = "STUDENT")]
        public IActionResult LatestMonthRecord()
        {
            //Get the student's internship record
            var internshipRecord = _context.Internship_Records
                .Include(ir => ir.UserBatch.Batch)
                .Where(ir => DateTime.Now >= ir.UserBatch.Batch.StartDate && DateTime.Now <= ir.UserBatch.Batch.EndDate)
                .Where(ir => ir.UserBatch.User.Id.Equals(_userManager.GetUserId(User)))
                .Include(ir => ir.MonthRecords)
                .ThenInclude(mn => mn.DayRecords)
                .SingleOrDefault();

            if (internshipRecord == null)
            {
                return NotFound();
            }

            Month_Record monthRecord;

            if (internshipRecord.MonthRecords.Count > 0)
            {
                //Get the latest record
                monthRecord = internshipRecord.MonthRecords[internshipRecord.MonthRecords.Count - 1];
            }
            else
            {
                //Create new record
                monthRecord = new Month_Record { InternshipRecord = internshipRecord };
                _context.Month_Records.Add(monthRecord);
                _context.SaveChanges();
            }

            var monthRecordObj = new
            {
                MonthId = monthRecord.MonthId,
                Approved = monthRecord.Approved,
                SoftSkillsCompetencyDoneWell = monthRecord.SoftSkillsCompetencyDoneWell,
                SoftSkillsCompetencyImprove = monthRecord.SoftSkillsCompetencyImprove,
                TechnicalCompetencyApplied = monthRecord.TechnicalCompetencyApplied,
                TechnicalCompetencyDoneWell = monthRecord.TechnicalCompetencyDoneWell,
                TechnicalCompetencyImprove = monthRecord.TechnicalCompetencyImprove,
                MentorSessionDateTimeStart = monthRecord.MentorSessionDateTimeStart,
                MentorSessionDateTimeEnd = monthRecord.MentorSessionDateTimeEnd,
                MentorSessionReflection = monthRecord.MentorSessionReflection,
                CommunicationGrading = monthRecord.CommunicationGrading,
                TechnicalGrading = monthRecord.TechnicalGrading,
                IndependenceGrading = monthRecord.IndependenceGrading,
                PerformanceGrading = monthRecord.PerformanceGrading,
                OverallGrading = monthRecord.OverallGrading,
                OverallFeedback = monthRecord.OverallFeedback
            };

            return new OkObjectResult(monthRecordObj);

        }

        [HttpPut("UpdateMentorSession/{id}")]
        [Authorize(Roles = "STUDENT")]
        public IActionResult UpdateMentorSession(int id, [FromBody] string value) {
            Month_Record monthRecord = _context.Month_Records
                .Include(mr => mr.InternshipRecord)
                .ThenInclude(ir => ir.UserBatch)
                .ThenInclude(ub => ub.Batch)
                .Include(mr => mr.InternshipRecord)
                .ThenInclude(ir => ir.UserBatch)
                .ThenInclude(ub => ub.User)
                .Where(mr => mr.InternshipRecord.UserBatch.Batch.StartDate < DateTime.Now && mr.InternshipRecord.UserBatch.Batch.EndDate > DateTime.Now && mr.MonthId == id)
                .SingleOrDefault();

            if (monthRecord == null)
            {
                return BadRequest(new { Message = "This month record is not editable or does not exist." });
            }
            else if (!monthRecord.InternshipRecord.UserBatch.User.Id.Equals(_userManager.GetUserId(User)))
            {
                return BadRequest(new { Message = "This month record does not belong to you!" });
            }
            else if (monthRecord.Approved == true)
            {
                return BadRequest(new { Message = "This month's records have already been approved and cannot be edited" });
            }

            var reflectionInput = JsonConvert.DeserializeObject<dynamic>(value);

            string arrivalTimeStr = reflectionInput.MentorSessionDateTimeStart.Value.ToString();
            string departTimeStr = reflectionInput.MentorSessionDateTimeEnd.Value.ToString();
            string reflectionStr = reflectionInput.MentorSessionReflection.Value.ToString();
            var timestart = new DateTime();
            var timeend = new DateTime();

            try
            {
                timestart = DateTime.ParseExact(arrivalTimeStr, "d/M/yyyy H:m:s", CultureInfo.InvariantCulture);
                timeend = DateTime.ParseExact(departTimeStr, "d/M/yyyy H:m:s", CultureInfo.InvariantCulture);
            }
            catch (Exception e)
            {
                return BadRequest(new { Message = "Incorrect timing" });
            }

            monthRecord.MentorSessionDateTimeStart = timestart;
            monthRecord.MentorSessionDateTimeEnd = timeend;
            monthRecord.MentorSessionReflection = reflectionStr;

            _context.SaveChanges();

            return new OkObjectResult(new { Message = "Month Record Updated Successfully!" });
        }

        // PUT api/values/5
        [HttpPut("UpdateReflection/{id}")]
        [Authorize(Roles = "STUDENT")]
        public IActionResult UpdateReflection(int id, [FromBody]string value)
        {
            Month_Record monthRecord = _context.Month_Records
                .Include(mr => mr.InternshipRecord)
                .ThenInclude(ir => ir.UserBatch)
                .ThenInclude(ub => ub.Batch)
                .Include(mr => mr.InternshipRecord)
                .ThenInclude(ir => ir.UserBatch)
                .ThenInclude(ub => ub.User)
                .Where(mr => mr.InternshipRecord.UserBatch.Batch.StartDate < DateTime.Now && mr.InternshipRecord.UserBatch.Batch.EndDate > DateTime.Now && mr.MonthId == id)
                .SingleOrDefault();

            if (monthRecord == null)
            {
                return BadRequest(new { Message = "This month record is not editable or does not exist." });
            }
            else if (!monthRecord.InternshipRecord.UserBatch.User.Id.Equals(_userManager.GetUserId(User)))
            {
                return BadRequest(new { Message = "This month record does not belong to you!" });
            }
            else if (monthRecord.Approved == true)
            {
                return BadRequest(new { Message = "This month's records have already been approved and cannot be edited" });
            }

            var reflectionInput = JsonConvert.DeserializeObject<dynamic>(value);

            monthRecord.SoftSkillsCompetencyDoneWell = reflectionInput.SoftSkillsCompetencyDoneWell.Value;
            monthRecord.SoftSkillsCompetencyImprove = reflectionInput.SoftSkillsCompetencyImprove.Value;

            monthRecord.TechnicalCompetencyApplied = reflectionInput.TechnicalCompetencyApplied.Value;
            monthRecord.TechnicalCompetencyDoneWell = reflectionInput.TechnicalCompetencyDoneWell.Value;
            monthRecord.TechnicalCompetencyImprove = reflectionInput.TechnicalCompetencyImprove.Value;

            _context.SaveChanges();

            return new OkObjectResult(new { Message = "Month Record Updated Successfully!" });
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
