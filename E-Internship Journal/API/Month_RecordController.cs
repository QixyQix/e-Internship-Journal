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
using E_Internship_Journal.Services;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace E_Internship_Journal.API
{
    [Produces("application/json")]
    [Route("api/month_record")]
    public class Month_RecordController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IEmailSender _emailSender;
        public Month_RecordController(UserManager<ApplicationUser> userManager, IEmailSender emailSender, ApplicationDbContext context)
        {
            _userManager = userManager;
            _emailSender = emailSender;
            _context = context;
        }

        // GET: api/values
        [HttpGet("InternshipMonthRecords/{id}")]
        [Authorize(Roles = "STUDENT , LO, SUPERVISOR")]
        public IActionResult GetInternshipMonthRecords(int id)
        {
            var internshipRecord = _context.Internship_Records
                .Where(ir => ir.InternshipRecordId == id)
                .Include(ir => ir.MonthRecords)
                .ThenInclude(mr => mr.CompetencyCheckeds)
                .SingleOrDefault();

            if (internshipRecord == null)
            {
                return NotFound();
            }

            List<object> monthRecordObjList = new List<object>();

            foreach (var monthRecord in internshipRecord.MonthRecords)
            {
                List<object> checkedCompetencyObjList = new List<object>();

                foreach (var cc in monthRecord.CompetencyCheckeds)
                {
                    checkedCompetencyObjList.Add(new
                    {
                        CompentencyCheckedId = cc.CompentencyCheckedId,
                        MonthRecordId = cc.MonthRecordId,
                        CompetencyId = cc.CompetencyId
                    });
                }

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
                    OverallFeedback = monthRecord.OverallFeedback,
                    CompetencyCheckeds = checkedCompetencyObjList
                });
            }

            return new OkObjectResult(monthRecordObjList);
        }

        [HttpGet("getMonthsTasks/{id}")]
        [Authorize(Roles = "STUDENT , LO, SUPERVISOR")]
        public IActionResult GetInternshipMonthRecordsTasks(int id)
        {
            Internship_Record internshipRecord = _context.Internship_Records.Where(ir => ir.InternshipRecordId == id)
                .Include(ir => ir.MonthRecords)
                .ThenInclude(mr => mr.TaskRecords)
                .SingleOrDefault();

            if (internshipRecord == null)
            {
                return new BadRequestObjectResult(new { Message = "Internship record does not exist!" });
            }

            List<object> monthObjList = new List<object>();


            foreach (var mr in internshipRecord.MonthRecords)
            {
                List<object> taskObjList = new List<object>();
                foreach (var tr in mr.TaskRecords)
                {
                    taskObjList.Add(new
                    {
                        TaskRecordId = tr.TaskRecordId,
                        Description = tr.Description,
                        Date = tr.Date,
                        WeekNo = tr.WeekNo,
                        Remarks = tr.Remarks
                    });
                }
                monthObjList.Add(new { TaskRecords = taskObjList});
            }

            return new OkObjectResult(monthObjList);
        }

        [HttpGet("GetMonthsAttendance/{id}")]
        [Authorize(Roles = "STUDENT , LO, SUPERVISOR")]
        public IActionResult GetMonthAttendance(int id) {
            Internship_Record internshipRecord = _context.Internship_Records.Where(ir => ir.InternshipRecordId == id)
                .Include(ir => ir.MonthRecords)
                .ThenInclude(mr => mr.DayRecords)
                .SingleOrDefault();

            if (internshipRecord == null)
            {
                return new BadRequestObjectResult(new { Message = "Internship record does not exist!" });
            }

            List<object> monthObjList = new List<object>();


            foreach (var mr in internshipRecord.MonthRecords)
            {
                List<object> dayObjList = new List<object>();
                foreach (var dr in mr.DayRecords)
                {
                    dayObjList.Add(new
                    {
                        DayId = dr.DayId,
                        Date = dr.Date,
                        ArrivalTime = dr.ArrivalTime,
                        DepartureTime = dr.DepartureTime,
                        WeekNo = dr.WeekNo,
                        IsPresent = dr.IsPresent,
                        Remarks = dr.Remarks,
                        SupervisorRemarks = dr.SupervisorRemarks
                    });
                }
                monthObjList.Add(new { DayRecords = dayObjList });
            }

            return new OkObjectResult(monthObjList);
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
                .Include(ir => ir.MonthRecords)
                .ThenInclude(mn => mn.CompetencyCheckeds)
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

            List<object> checkedCompetencyObjList = new List<object>();

            foreach (var cc in monthRecord.CompetencyCheckeds)
            {
                checkedCompetencyObjList.Add(new
                {
                    CompentencyCheckedId = cc.CompentencyCheckedId,
                    MonthRecordId = cc.MonthRecordId,
                    CompetencyId = cc.CompetencyId
                });
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
                OverallFeedback = monthRecord.OverallFeedback,
                CompetencyCheckeds = checkedCompetencyObjList
            };

            return new OkObjectResult(monthRecordObj);

        }

        [HttpGet("DataForReview/{id}")]
        [Authorize(Roles = "SUPERVISOR")]
        public IActionResult GetDataForReview(int id) {
            Internship_Record internshipRecord = _context.Internship_Records.Where(ir => ir.InternshipRecordId == id)
                .Include(ir => ir.MonthRecords)
                .ThenInclude(mr => mr.TaskRecords)
                .Include(ir => ir.MonthRecords)
                .ThenInclude(mr => mr.DayRecords)
                .Include(ir => ir.MonthRecords)
                .ThenInclude(mr => mr.CompetencyCheckeds)
                .Include(ir => ir.Project)
                .SingleOrDefault();

            if (internshipRecord == null)
            {
                return new BadRequestObjectResult(new { Message = "Day Record does not exist!" });
            }
            else if (!internshipRecord.Project.SupervisorId.Equals(_userManager.GetUserId(User)))
            {
                return new BadRequestObjectResult(new { Message = "You are not authorized to perform ths action." });
            }

            List<object> monthRecordObjs = new List<object>();

            int monthnumber = 0;

            foreach (var monthRecord in internshipRecord.MonthRecords) {
                monthnumber++;
                if (monthRecord.Approved != true) {
                    List<object> taskRecordList = new List<object>();
                    List<object> attendanceList = new List<object>();
                    List<object> checkedCompetencyObjList = new List<object>();

                    //Tasks
                    foreach (var tr in monthRecord.TaskRecords) {
                        taskRecordList.Add(new
                        {
                            TaskRecordId = tr.TaskRecordId,
                            Description = tr.Description,
                            Date = tr.Date,
                            WeekNo = tr.WeekNo,
                            Remarks = tr.Remarks
                        });
                    }

                    //DayRecords
                    foreach (var dr in monthRecord.DayRecords)
                    {
                        attendanceList.Add(new
                        {
                            DayId = dr.DayId,
                            Date = dr.Date,
                            ArrivalTime = dr.ArrivalTime,
                            DepartureTime = dr.DepartureTime,
                            WeekNo = dr.WeekNo,
                            IsPresent = dr.IsPresent,
                            Remarks = dr.Remarks,
                            SupervisorRemarks = dr.SupervisorRemarks
                        });
                    }

                    //Competencies
                    foreach (var cc in monthRecord.CompetencyCheckeds)
                    {
                        checkedCompetencyObjList.Add(new
                        {
                            CompentencyCheckedId = cc.CompentencyCheckedId,
                            MonthRecordId = cc.MonthRecordId,
                            CompetencyId = cc.CompetencyId
                        });
                    }


                    monthRecordObjs.Add(new
                    {
                        MonthNumber = monthnumber,
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
                        OverallFeedback = monthRecord.OverallFeedback,
                        TaskRecords = taskRecordList,
                        DayRecords = attendanceList,
                        CompetencyCheckeds = checkedCompetencyObjList
                    });
                }
            }
            return new OkObjectResult(monthRecordObjs);
        }

        [HttpGet("ReviewInternshipMonth/{id}")]
        [Authorize(Roles = "LO")]
        public IActionResult GetReviewInternshipMonth(int id)
        {
            //Internship_Record internshipRecord = _context.Internship_Records.Where(ir => ir.InternshipRecordId == id)
            //.Include(ir => ir.MonthRecords)
            //.ThenInclude(mr => mr.TaskRecords)
            //.Include(ir => ir.MonthRecords)
            //.ThenInclude(mr => mr.DayRecords)
            //.Include(ir => ir.MonthRecords)
            //.ThenInclude(mr => mr.CompetencyCheckeds)
            //.Include(ir => ir.Project)
            //.SingleOrDefault();
            var internshipRecord = _context.Internship_Records
                .Where(ir => ir.InternshipRecordId == id)
                .Include(ir => ir.MonthRecords)
                .ThenInclude(mr => mr.CompetencyCheckeds)
                .Include(ir => ir.MonthRecords)
                .ThenInclude(mr => mr.DayRecords)
                .Include(ir => ir.MonthRecords)
                .ThenInclude(mr => mr.TaskRecords)
                .Include(ir => ir.UserBatch)
                .ThenInclude(ub => ub.Batch)
                .ThenInclude(batch => batch.Course)
                .ThenInclude(course => course.CompetencyTitle)
                .SingleOrDefault();

            //    .SingleOrDefault();

            if (internshipRecord == null)
            {
                return NotFound();
            }

            List<object> monthRecordObjList = new List<object>();

            foreach (var monthRecord in internshipRecord.MonthRecords)
            {
                List<object> checkedCompetencyObjList = new List<object>();
                List<object> taskRecordObjs = new List<object>();
                List<object> Day_Records_List = new List<object>();
                foreach (var cc in monthRecord.CompetencyCheckeds)
                {
                    checkedCompetencyObjList.Add(new
                    {
                        CompentencyCheckedId = cc.CompentencyCheckedId,
                        MonthRecordId = cc.MonthRecordId,
                        CompetencyId = cc.CompetencyId
                    });
                }
                foreach (var task in monthRecord.TaskRecords)
                {
                    taskRecordObjs.Add(new
                    {
                        Date = task.Date,
                        Description = task.Description,
                        TaskRecordId = task.TaskRecordId,
                        TaskRemarks = task.Remarks,
                        WeekNo = task.WeekNo,
                        task.MonthRecordId

                    });
                }
                foreach (var dayRecord in monthRecord.DayRecords)
                {
                    Day_Records_List.Add(new
                    {
                        dayRecord.DayId,
                        dayRecord.Date,
                        dayRecord.ArrivalTime,
                        dayRecord.DepartureTime,
                        dayRecord.WeekNo,
                        dayRecord.Remarks,
                        dayRecord.MonthRecordId,
                        dayRecord.SupervisorRemarks
                    });
                }
                List<object> competencyObjList = new List<object>();
                var foundCompetency = _context.CompetencyTitles.Include(c => c.Competencies).Where(ct => ct.CourseId == internshipRecord.UserBatch.Batch.Course.CourseId).ToList();
                foreach (var oneCompetency in foundCompetency)
                {

                    competencyObjList.Add(new
                    {
                        oneCompetency.CompetencyTitleId,
                        oneCompetency.TitleCompetency,
                        oneCompetency.ViewBy,
                        CompetencyList = oneCompetency.Competencies.Select(desc => new
                        {
                            desc.CompetencyId,
                            desc.Description,
                            desc.CreatedBy,
                            desc.DeletedAt,

                            desc.ModifiedAt,
                            desc.ModifiedBy,
                            desc.ViewBy
                        }).Where(d => d.DeletedAt == null)
                    });
                }
                monthRecordObjList.Add(new
                {
                    monthRecord.MonthId,
                    monthRecord.Approved,
                    monthRecord.SoftSkillsCompetencyDoneWell,
                    monthRecord.SoftSkillsCompetencyImprove,
                    monthRecord.TechnicalCompetencyApplied,
                    monthRecord.TechnicalCompetencyDoneWell,
                    monthRecord.TechnicalCompetencyImprove,
                    monthRecord.MentorSessionDateTimeStart,
                    monthRecord.MentorSessionDateTimeEnd,
                    monthRecord.MentorSessionReflection,
                    monthRecord.CommunicationGrading,
                    monthRecord.TechnicalGrading,
                    monthRecord.IndependenceGrading,
                    monthRecord.PerformanceGrading,
                    monthRecord.OverallGrading,
                    monthRecord.OverallFeedback,
                    CompetencyCheckeds = checkedCompetencyObjList,
                    TaskRecords = taskRecordObjs,
                    AttendanceRecords = Day_Records_List,
                    ListOfCompetency = competencyObjList
                    });
            }

            return new OkObjectResult(monthRecordObjList);
        }
        [HttpPut("UpdateCompetencyCheckList/{id}")]
        [Authorize(Roles = "STUDENT")]
        public IActionResult UpdateCompetencyCheckList(int id, [FromBody] string value)
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

            var competencyInput = JsonConvert.DeserializeObject<dynamic>(value);

            var competencyChecked = _context.Competency_Checkeds.Where(cc => cc.MonthRecordId == id)
                .ToList();

            foreach (var cc in competencyChecked)
            {
                _context.Competency_Checkeds.Remove(cc);
            }

            _context.SaveChanges();

            foreach (var competencyId in competencyInput.CompetencyIds)
            {
                _context.Competency_Checkeds.Add(new Competency_Checked { MonthRecord = monthRecord, CompetencyId = competencyId });
            }

            _context.SaveChanges();

            return new OkObjectResult(new { Message = "Month Record Updated Successfully!" });
        }

        [HttpPut("UpdateMentorSession/{id}")]
        [Authorize(Roles = "STUDENT")]
        public IActionResult UpdateMentorSession(int id, [FromBody] string value)
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

        [HttpPut("gradeMonth/{id}")]
        [Authorize(Roles = "SUPERVISOR")]
        public IActionResult gradeMonth(int id, [FromBody] string value) {
            Month_Record monthRecord = _context.Month_Records.Where(mr => mr.MonthId == id)
                .Include(mr => mr.InternshipRecord)
                .ThenInclude(ir => ir.Project)
                .Include(ir=>ir.InternshipRecord)
                .ThenInclude(ub=>ub.UserBatch)
                .ThenInclude(u=>u.User)
                .SingleOrDefault();
            
            if (monthRecord == null)
            {
                return BadRequest(new { Message = "This month record is not editable or does not exist." });
            }
            else if (!monthRecord.InternshipRecord.Project.SupervisorId.Equals(_userManager.GetUserId(User)))
            {
                return BadRequest(new { Message = "You are not authorized to perform this action!" });
            }
            else if (monthRecord.Approved == true)
            {
                return BadRequest(new { Message = "This month's records have already been approved and cannot be regraded!" });
            }

            var gradingInput = JsonConvert.DeserializeObject<dynamic>(value);

            try
            {
                monthRecord.CommunicationGrading = Int32.Parse(gradingInput.CommunicationGrading.Value.ToString());
                monthRecord.TechnicalGrading = Int32.Parse(gradingInput.TechnicalGrading.Value.ToString());
                monthRecord.IndependenceGrading = Int32.Parse(gradingInput.IndependenceGrading.Value.ToString());
                monthRecord.PerformanceGrading = Int32.Parse(gradingInput.PerformanceGrading.Value.ToString());
                monthRecord.OverallGrading = (monthRecord.CommunicationGrading + monthRecord.TechnicalGrading + monthRecord.IndependenceGrading + monthRecord.PerformanceGrading) / 4;
                monthRecord.OverallFeedback = gradingInput.OverallFeedback.Value.ToString();
                monthRecord.Approved = true;
                _context.SaveChanges();
                var studentEmail = monthRecord.InternshipRecord.UserBatch.User.Email;
                var studentName = monthRecord.InternshipRecord.UserBatch.User.FullName;
               _emailSender.SendChangeEmailAsync(false,studentEmail, "Your Supervisor graded you!", "Hi, " + studentName, "Your Supervisor has graded your previous month record. You may view your grade via the website.");
                return new OkObjectResult(new { Message = "Month record graded!!" });
            }
            catch (Exception e)
            {
                return new BadRequestObjectResult(new { Message = e.Message.ToString() });
            }

        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
