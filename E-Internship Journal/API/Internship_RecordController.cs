using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using E_Internship_Journal.Data;
using E_Internship_Journal.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using Newtonsoft.Json;
using System.Globalization;
using E_Internship_Journal.Services;

namespace E_Internship_Journal.API
{
    [Authorize]
    [Produces("application/json")]
    [Route("api/Internship_Record")]
    public class Internship_RecordController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IEmailSender _emailSender;
        public Internship_RecordController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, IEmailSender emailSender)
        {
            _context = context;
            _emailSender = emailSender;
            _userManager = userManager;
        }

        // GET: api/Internship_Record
        [HttpGet]
        public IEnumerable<Internship_Record> GetInternship_Records()
        {
            return _context.Internship_Records;
        }

        // GET: api/Internship_Record/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetInternship_Record([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var internship_Record = await _context.Internship_Records.SingleOrDefaultAsync(m => m.InternshipRecordId == id);

            if (internship_Record == null)
            {
                return NotFound();
            }

            return Ok(internship_Record);
        }

        [HttpGet("getCurrentInternshipRecord")]
        [Authorize(Roles = "STUDENT")]
        public async Task<IActionResult> getCurrentInternshipRecord()
        {

            var userId = _userManager.GetUserId(User);

            Internship_Record internshipRecord = _context.Internship_Records
                .Include(ir => ir.UserBatch)
                .ThenInclude(ub => ub.Batch)
                .Include(m => m.MonthRecords)
                .Include(p => p.Project)
                .ThenInclude(s => s.Supervisor)
                .Where(ir => ir.UserBatch.UserId.Equals(userId) && ir.UserBatch.Batch.StartDate <= DateTime.Now && ir.UserBatch.Batch.EndDate.AddDays(1) >= DateTime.Now)
                .Single();

            var internshiprecordId = internshipRecord.InternshipRecordId;
            DateTime startDate = internshipRecord.UserBatch.Batch.StartDate;
            DateTime endDate = internshipRecord.UserBatch.Batch.EndDate;
            // var tryDate = DateTime.ParseExact("06/11/2017", "dd/MM/yyyy", CultureInfo.InvariantCulture);
            double BusinessDay = BusinessDaysUntil(startDate, endDate);
            var weekNo = Math.Ceiling(BusinessDay / 5.0);
            // var tt = weekNo / 4;
            if (internshipRecord == null)
            {
                return NotFound(new { Message = "No internship record found" });
            }
            else
            {
                Month_Record monthRecord;
                if (internshipRecord.MonthRecords.Count * 4 < (weekNo))
                // if (internshipRecord.MonthRecords.Count > 0)
                {
                    //Create new record
                    monthRecord = new Month_Record { InternshipRecord = internshipRecord };
                    _context.Month_Records.Add(monthRecord);
                    _context.SaveChanges();


                    var supervisorEmail = internshipRecord.Project.Supervisor.Email;
                    var supervisorName = internshipRecord.Project.Supervisor.FullName;
                    var studentName = _context.Users.Where(u => u.Id == internshipRecord.UserBatch.UserId).Select(u => u.FullName).Single();
                    // var gradeRecord = internshipRecord.MonthRecords[internshipRecord.MonthRecords.Count - 1];
                    await _emailSender.SendChangeEmailAsync(false, supervisorEmail, "Grade your student monthly record",
           "Hi, " + supervisorName, "The student " + studentName + ", last month's record is ready to be graded. Kindly login to grade the student.", "", "");
                    //Get the latest record
                    // monthRecord = internshipRecord.MonthRecords[internshipRecord.MonthRecords.Count - 1];
                }
                //   if (weekNo )


                return new JsonResult(new { InternshipRecordId = internshipRecord.InternshipRecordId });
            }

        }

        [HttpPut("updateInternshipSurvey/{id}")]
        [Authorize(Roles = "STUDENT")]
        public IActionResult updateInternshipSurvey(int id, [FromBody] String value)
        {
            var internshipRecord = _context.Internship_Records
                .Include(ir => ir.UserBatch)
                .ThenInclude(ir => ir.Batch)
                .Where(ir => ir.InternshipRecordId == id && _userManager.GetUserId(User).Equals(ir.UserBatch.User.Id))
                .SingleOrDefault();

            if (internshipRecord == null)
            {
                return NotFound();
            }

            var surveyInput = JsonConvert.DeserializeObject<dynamic>(value);

            try
            {
                internshipRecord.FeedbackUseful = Boolean.Parse(surveyInput.FeedbackUseful.Value);
                internshipRecord.FeedbackImproved = Boolean.Parse(surveyInput.FeedbackImproved.Value);
                internshipRecord.FeedbackExperiences = Boolean.Parse(surveyInput.FeedbackExperiences.Value);
                internshipRecord.FeedbackRecommend = Boolean.Parse(surveyInput.FeedbackRecommend.Value);

                internshipRecord.FeedbackEnjoy = surveyInput.FeedbackEnjoy.Value;
                internshipRecord.FeedbackLeastEnjoy = surveyInput.FeedbackLeastEnjoy.Value;
                internshipRecord.FeedbackTakeaway = surveyInput.FeedbackTakeaway.Value;
                internshipRecord.FeedbackCareer = surveyInput.FeedbackCareer.Value;

                _context.SaveChanges();
            }
            catch (Exception e)
            {
                return BadRequest(new { Message = e.Message.ToString() });
            }



            return new OkObjectResult(new { Message = "Survey updated successfully!" });
        }
        [HttpGet("viewInternshipSurvey/{id}")]
        [Authorize(Roles = "LO")]
        public IActionResult ViewInternshipSurvey(int id)
        {
            var internshipRecord = _context.Internship_Records
                .Where(ir => ir.InternshipRecordId == id).SingleOrDefault();
            if (internshipRecord == null)
            {
                return NotFound();
            }
            try
            {
                var response = new
                {
                    internshipRecord.FeedbackUseful,
                    internshipRecord.FeedbackImproved,
                    internshipRecord.FeedbackExperiences,
                    internshipRecord.FeedbackRecommend,
                    internshipRecord.FeedbackEnjoy,
                    internshipRecord.FeedbackLeastEnjoy,
                    internshipRecord.FeedbackTakeaway,
                    internshipRecord.FeedbackCareer

                };
                return new JsonResult(response);
            }
            catch (Exception exceptionObject)
            {
                return BadRequest(new { Message = exceptionObject.Message });
            }

        }//End of viewInternshipSurvey method

        [HttpGet("ViewCompanyChecklist/{id}")]
        [Authorize(Roles = "LO, SUPERVISOR")]
        public IActionResult ViewCompanyChecklist(int id)
        {
            var internshipRecord = _context.Internship_Records
                .Where(ir => ir.InternshipRecordId == id).SingleOrDefault();
            if (internshipRecord == null)
            {
                return NotFound();
            }
            try
            {
                var response = new
                {
                    internshipRecord.CompanyCheck1a,
                    internshipRecord.CompanyCheck1b,
                    internshipRecord.CompanyCheck2a,
                    internshipRecord.CompanyCheck2b,
                    internshipRecord.CompanyCheck2c,
                    internshipRecord.CompanyCheck2d,
                    internshipRecord.CompanyCheck2e,
                    internshipRecord.CompanyCheck2f,
                    internshipRecord.CompanyCheck3a,
                    internshipRecord.CompanyCheck3b,
                    internshipRecord.CompanyCheck3c

                };
                return new JsonResult(response);
            }
            catch (Exception exceptionObject)
            {
                return BadRequest(new { Message = exceptionObject.Message });
            }

        }//End of viewCompanyChecklist method

        [HttpPut("updateCompanyChecklist/{id}")]
        [Authorize(Roles = "SUPERVISOR")]
        public IActionResult UpdateCompanyChecklist(int id, [FromBody] string value)
        {
            var internshipRecord = _context.Internship_Records
                .Where(ir => ir.InternshipRecordId == id)
                .Include(ir => ir.Project)
                .ThenInclude(p => p.Supervisor)
                .SingleOrDefault();

            if (internshipRecord == null)
            {
                return new BadRequestObjectResult(new { Message = "Internship record does not exist!" });
            }
            else if (!internshipRecord.Project.Supervisor.Id.Equals(_userManager.GetUserId(User)))
            {
                return new BadRequestObjectResult(new { Message = "You are not authorized to perform this action!" });
            }

            var checklistInput = JsonConvert.DeserializeObject<dynamic>(value);

            try
            {
                internshipRecord.CompanyCheck1a = checklistInput.CompanyCheck1a.Value;
                internshipRecord.CompanyCheck1b = checklistInput.CompanyCheck1b.Value;
                internshipRecord.CompanyCheck2a = checklistInput.CompanyCheck2a.Value;
                internshipRecord.CompanyCheck2b = checklistInput.CompanyCheck2b.Value;
                internshipRecord.CompanyCheck2c = checklistInput.CompanyCheck2c.Value;
                internshipRecord.CompanyCheck2d = checklistInput.CompanyCheck2d.Value;
                internshipRecord.CompanyCheck2e = checklistInput.CompanyCheck2e.Value;
                internshipRecord.CompanyCheck2f = checklistInput.CompanyCheck2f.Value;
                internshipRecord.CompanyCheck3a = checklistInput.CompanyCheck3a.Value;
                internshipRecord.CompanyCheck3b = checklistInput.CompanyCheck3b.Value;
                internshipRecord.CompanyCheck3c = checklistInput.CompanyCheck3c.Value;

                _context.SaveChanges();

                return new OkObjectResult(new { Message = "Updated company checklist successfully!" });
            }
            catch (Exception e)
            {
                return new BadRequestObjectResult(new { Message = e.Message.ToString() });
            }


        }

        // PUT: api/Internship_Record/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutInternship_Record([FromRoute] int id, [FromBody] Internship_Record internship_Record)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != internship_Record.InternshipRecordId)
            {
                return BadRequest();
            }

            _context.Entry(internship_Record).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!Internship_RecordExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Internship_Record
        [HttpPost]
        public async Task<IActionResult> PostInternship_Record([FromBody] Internship_Record internship_Record)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.Internship_Records.Add(internship_Record);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetInternship_Record", new { id = internship_Record.InternshipRecordId }, internship_Record);
        }

        // DELETE: api/Internship_Record/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteInternship_Record([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var internship_Record = await _context.Internship_Records.SingleOrDefaultAsync(m => m.InternshipRecordId == id);
            if (internship_Record == null)
            {
                return NotFound();
            }

            _context.Internship_Records.Remove(internship_Record);
            await _context.SaveChangesAsync();

            return Ok(internship_Record);
        }
        // GET: api/Internship_Record/5
        [HttpGet("Lo_GetStudentInternship_Record")]
        public IActionResult Lo_GetStudentInternship_Record()
        {

            var internship_Record = _context.Internship_Records.Include(ir => ir.UserBatch).ThenInclude(ub => ub.User)
                //.Where(ir =>ir.UserBatch.User.Id == ir.UserBatch.UserId)
                .Where(m => m.LiaisonOfficerId == _userManager.GetUserId(User));

            //var studentBatch = _context.ApplicationUsers.SingleOrDefault(d => d.Id ==
            //(_context.UserBatches.Where(m => m.UserBatchId == internship_Record.UserBatchId)).Select(a=>a.UserId).ToString());


            if (internship_Record == null)
            {
                return NotFound();
            }
            List<object> studentList = new List<object>();
            foreach (var ir in internship_Record)
            {
                studentList.Add(new
                {
                    StudentId = ir.UserBatch.User.Email,
                    StudentName = ir.UserBatch.User.FullName,
                    StudentJournal = ir.InternshipRecordId,


                });
            }

            return new JsonResult(studentList);
        }

        [HttpGet("StudentInfo/{id}")]
        public IActionResult GetStudentInfo(int id)
        {
            var sir = _context.Internship_Records.Where(ir => ir.InternshipRecordId == id)
                .Include(ir => ir.UserBatch)
                .ThenInclude(ub => ub.User)
                .Include(ir => ir.UserBatch)
                .ThenInclude(ub => ub.Batch)
                .ThenInclude(b => b.Course)
                .Include(ir => ir.Project)
                .ThenInclude(p => p.Supervisor)
                .Include(ir => ir.Project)
                .ThenInclude(p => p.Company)
                .Include(ir => ir.LiaisonOfficer)
                .SingleOrDefault();

            if (sir == null)
            {
                return new BadRequestObjectResult(new { Message = "Internship record not found" });
            }

            var studentdetailsobj = new
            {
                StudentName = sir.UserBatch.User.FullName,
                StudentCourse = sir.UserBatch.Batch.Course.CourseName,
                StudentMobileNo = sir.UserBatch.User.PhoneNumber,
                StudentEmail = sir.UserBatch.User.Email,
                LOName = sir.LiaisonOfficer.FullName,
                LOMobileNo = sir.LiaisonOfficer.PhoneNumber,
                LOEmail = sir.LiaisonOfficer.Email,
                CompanyName = sir.Project.Company.CompanyName,
                CompanyAddress = sir.Project.Company.CompanyAddress,
                CompanySupervisor = sir.Project.Supervisor.FullName,
                CompanySupervisorNo = sir.Project.Supervisor.PhoneNumber,
                CompanySupervisorEmail = sir.Project.Supervisor.Email
            };

            return new OkObjectResult(studentdetailsobj);
        }

        // GET: api/Internship_Record/5
        [HttpGet("Sup_GetStudentInternship_Record")]
        public IActionResult Sup_GetStudentInternship_Record()
        {

            var internship_Record = _context.Internship_Records.Include(ir => ir.UserBatch).ThenInclude(ub => ub.User)
                .Include(ir => ir.Project)
                .Where(ir => ir.Project.Supervisor.Id.Equals(_userManager.GetUserId(User)))
                .Include(ir => ir.MonthRecords);


            if (internship_Record == null)
            {
                return NotFound();
            }

            List<object> studentList = new List<object>();
            foreach (var ir in internship_Record)
            {
                bool needReview = false;
                foreach (var mr in ir.MonthRecords)
                {
                    if (mr.Approved == false)
                    {
                        needReview = true;
                        break;
                    }
                }

                studentList.Add(new
                {
                    StudentId = ir.UserBatch.User.Email,
                    StudentName = ir.UserBatch.User.FullName,
                    InternshipRecordId = ir.InternshipRecordId,
                    Review = needReview
                });
            }

            return new JsonResult(studentList);
        }
        [HttpGet("Lo_GetStudentGrading/{id}")]
        public IActionResult Lo_GetStudentGrading(int id)
        {
            var studentInternship_Record = _context.Internship_Records.Include(month => month.MonthRecords)
                .Where(ir => ir.InternshipRecordId == id).SingleOrDefault();

            //var studentBatch = _context.ApplicationUsers.SingleOrDefault(d => d.Id ==
            //(_context.UserBatches.Where(m => m.UserBatchId == internship_Record.UserBatchId)).Select(a=>a.UserId).ToString());


            if (studentInternship_Record == null)
            {
                // return NotFound();
            }
            List<object> supervisorGrade = new List<object>();
            for (int i = 0; i < studentInternship_Record.MonthRecords.Count; i++)
            {
                supervisorGrade.Add(new
                {
                    studentInternship_Record.MonthRecords[i].Approved,
                    studentInternship_Record.MonthRecords[i].PerformanceGrading,
                    studentInternship_Record.MonthRecords[i].TechnicalGrading,
                    studentInternship_Record.MonthRecords[i].IndependenceGrading,
                    studentInternship_Record.MonthRecords[i].CommunicationGrading
                });


            }
            if (studentInternship_Record.Approved == true)
            {
                var response = new
                {
                    studentInternship_Record.Approved,
                    studentInternship_Record.PresentationGrading,
                    studentInternship_Record.JournalGrading,
                    studentInternship_Record.PosterGrading,
                    studentInternship_Record.OverallPerformance,
                    studentInternship_Record.OverallGrading,
                    studentInternship_Record.SLOOverallGrading,
                    supervisorGrade
                    // studentInternship_Record.MonthRecords
                };
                return new JsonResult(response);
            }
            else
            {
                var response = new
                {
                    studentInternship_Record.Approved,
                    supervisorGrade
                };
                return new JsonResult(response);
            }
        }//End of Lo_GetStudentGrading

        [HttpPut("gradeStudentInternship/{id}")]
        [Authorize(Roles = "LO")]
        public async Task<IActionResult> gradeStudentInternshipAsync(string id, [FromBody] string value)
        {

            var foundUserId = (await _userManager.FindByEmailAsync(id)).Id;
            var internshipRecord = _context.Internship_Records.Where(ir => ir.UserBatch.UserId.Equals(foundUserId))
                .Where(ir => ir.LiaisonOfficerId.Equals(_userManager.GetUserId(User)))
                .Include(ub => ub.UserBatch)
                .SingleOrDefault();

            if (internshipRecord == null)
            {
                return BadRequest(new { Message = "This Internship record is not editable or does not exist." });
            }
            else if (!internshipRecord.LiaisonOfficerId.Equals(_userManager.GetUserId(User)))
            {
                return BadRequest(new { Message = "You are not authorized to perform this action!" });
            }
            else if (internshipRecord.Approved == true)
            {
                return BadRequest(new { Message = "This Internship's records have already been approved and cannot be regraded!" });
            }

            var gradingInput = JsonConvert.DeserializeObject<dynamic>(value);

            try
            {

                internshipRecord.PosterGrading = Int32.Parse(gradingInput.PosterGrade.Value.ToString());
                internshipRecord.PresentationGrading = Int32.Parse(gradingInput.PresentationGrade.Value.ToString());
                internshipRecord.JournalGrading = Int32.Parse(gradingInput.StudentJournalGrade.Value.ToString());
                internshipRecord.OverallPerformance = Int32.Parse(gradingInput.OverallPerformanceGrade.Value.ToString());
                internshipRecord.OverallGrading = Decimal.Parse(gradingInput.OverallGrading.Value.ToString());
                internshipRecord.FinalGrading = Decimal.Parse(gradingInput.FinalGrading.Value.ToString());

                internshipRecord.Approved = true;
                _context.SaveChanges();

                return new OkObjectResult(new { Message = "Internship record graded!!" });
            }
            catch (Exception e)
            {
                return new BadRequestObjectResult(new { Message = e.Message.ToString() });
            }

        }
        [HttpGet("SLo_GetBatchGrading/{id}")]
        public IActionResult SLo_GetBatchGrading(int id)
        {
            var studentInternship_Record = _context.Internship_Records.Include(month => month.MonthRecords)
                .Where(ir => ir.InternshipRecordId == id).SingleOrDefault();
            var batchInternship_Record = _context.Internship_Records
                .Include(ub => ub.UserBatch)
                .ThenInclude(u => u.User)
                .Include(m => m.MonthRecords)
                .Where(b => b.UserBatch.BatchId == id)
                .ToList();
            //var studentBatch = _context.ApplicationUsers.SingleOrDefault(d => d.Id ==
            //(_context.UserBatches.Where(m => m.UserBatchId == internship_Record.UserBatchId)).Select(a=>a.UserId).ToString());


            if (batchInternship_Record.Count == 0)
            {
                //return NotFound();
            }
            var approved = true;
            List<object> studentGrade = new List<object>();

            foreach (var eachBatchInternshipRecord in batchInternship_Record)
            {
                if (eachBatchInternshipRecord.Approved != true)
                {
                    studentGrade.Add(new
                    {
                        eachBatchInternshipRecord.InternshipRecordId,
                        eachBatchInternshipRecord.UserBatch.User.FullName,
                        eachBatchInternshipRecord.UserBatch.User.StudentId,
                        OverallGrading = "Not Graded",
                        SupervisorGrade = "Not Graded",
                        FinalGrading = "Not Graded",
                        eachBatchInternshipRecord.SLOApproved,
                        //supervisorGrade
                    });
                    approved = false;
                }
                else
                {
                    if (eachBatchInternshipRecord.SLOApproved != true)
                    {
                        approved = false;
                    }
                    decimal supervisorGrading = 0;
                    for (int i = 0; i < eachBatchInternshipRecord.MonthRecords.Count; i++)
                    {
                        if (eachBatchInternshipRecord.MonthRecords[i].Approved == true)
                        {
                            supervisorGrading = supervisorGrading + decimal.Parse(eachBatchInternshipRecord.MonthRecords[i].OverallGrading.ToString());
                        }
                    }
                    studentGrade.Add(new
                    {
                        eachBatchInternshipRecord.InternshipRecordId,
                        eachBatchInternshipRecord.UserBatch.User.FullName,
                        eachBatchInternshipRecord.UserBatch.User.StudentId,
                        eachBatchInternshipRecord.OverallGrading,
                        eachBatchInternshipRecord.SLOOverallGrading,
                        SupervisorGrade = (supervisorGrading / eachBatchInternshipRecord.MonthRecords.Count),
                        eachBatchInternshipRecord.FinalGrading,
                        eachBatchInternshipRecord.SLOApproved,
                    });
                }//End of else statement
            }
            return new JsonResult(new { approved, studentGrade });

        }//End of Lo_GetStudentGrading

        [HttpPut("editStudentGrade/{id}")]
        [Authorize(Roles = "SLO")]
        public async Task<IActionResult> EditStudentGrade(int id, [FromBody] string value)
        {

            //  var foundUserId = (await _userManager.FindByEmailAsync(id)).Id;
            var internshipRecord = _context.Internship_Records.Where(ir => ir.InternshipRecordId == id)
                .SingleOrDefault();

            if (internshipRecord == null)
            {
                return BadRequest(new { Message = "This Internship record is not editable or does not exist." });
            }
            else if (internshipRecord.SLOApproved == true)
            {
                return BadRequest(new { Message = "This Internship's records have already been approved and cannot be regraded!" });
            }

            var gradingInput = JsonConvert.DeserializeObject<dynamic>(value);

            try
            {
                internshipRecord.FinalGrading = Decimal.Parse(gradingInput.FinalGrading.Value.ToString());

                //internshipRecord.Approved = true;
                _context.SaveChanges();

                return new OkObjectResult(new { Message = "Final Grading Changed!!" });
            }
            catch (Exception e)
            {
                return new BadRequestObjectResult(new { Message = e.Message.ToString() });
            }

        }

        [HttpPut("SLOGradeStudentInternship/")]
        [Authorize(Roles = "SLO")]
        public async Task<IActionResult> SLOGradeStudentInternship([FromBody] string value)
        {

            //var internshipRecord = _context.Internship_Records.Where(ir => ir.UserBatch.UserId.Equals(foundUserId))
            //    .Where(ir => ir.LiaisonOfficerId.Equals(_userManager.GetUserId(User)))
            //    .Include(ub => ub.UserBatch)
            //    .SingleOrDefault();


            var gradingInput = JsonConvert.DeserializeObject<dynamic>(value);

            try
            {

                //  new List<int>(intArray);
                var internshipIdList = gradingInput.InternshipIdList;
                for (int i = 0; i < internshipIdList.Count; i++)
                {
                    var eachId = (int)internshipIdList[i].Value;
                    var foundInternshipRecord = _context.Internship_Records.Where(ir => ir.InternshipRecordId == eachId).Single();

                    foundInternshipRecord.SLOApproved = true;

                }
                await _context.SaveChangesAsync();

                return new OkObjectResult(new { Message = "Internship record graded!!" });
            }
            catch (Exception e)
            {
                return new BadRequestObjectResult(new { Message = e.Message.ToString() });
            }

        }//End of SLOGradeStudentInternship

        [HttpGet("AllInternshipRecords")]
        [Authorize(Roles = "ADMIN")]
        public IActionResult GetAllInternshipRecords()
        {
            var internshipRecords = _context.Internship_Records.Include(ir => ir.UserBatch).ThenInclude(ub => ub.Batch).ThenInclude(b => b.Course).Include(ir => ir.UserBatch).ThenInclude(ub => ub.User).ToList();

            List<object> internshipRecordList = new List<object>();

            foreach (var ir in internshipRecords)
            {
                internshipRecordList.Add(new
                {
                    ir.InternshipRecordId,
                    ir.Approved,
                    ir.SLOApproved,
                    ir.PosterGrading,
                    ir.PresentationGrading,
                    ir.JournalGrading,
                    ir.OverallPerformance,
                    ir.OverallGrading,
                    ir.FinalGrading,
                    ir.SLOOverallGrading,
                    ir.Comment,
                    ir.PosterUrl,
                    ir.UserBatch.Batch.BatchId,
                    ir.UserBatch.Batch.BatchName,
                    ir.UserBatch.Batch.StartDate,
                    ir.UserBatch.Batch.EndDate,
                    ir.UserBatch.Batch.Course.CourseId,
                    ir.UserBatch.Batch.Course.CourseName,
                    ir.UserBatch.Batch.Course.CourseCode,
                    StudentAccId = ir.UserBatch.User.Id,
                    StudentId = ir.UserBatch.User.StudentId,
                    StudentName = ir.UserBatch.User.FullName
                });
            }

            return new OkObjectResult(internshipRecordList);
        }

        [HttpPut("toggleLock/{id}")]
        public IActionResult toggleLock(int id)
        {
            var internshipRecord = _context.Internship_Records.Where(ir => ir.InternshipRecordId == id).SingleOrDefault();

            if (internshipRecord != null)
            {
                internshipRecord.SLOApproved = !internshipRecord.SLOApproved;
                _context.SaveChanges();
            }
            return new OkObjectResult(new { State = internshipRecord.SLOApproved });
        }

        private bool Internship_RecordExists(int id)
        {
            return _context.Internship_Records.Any(e => e.InternshipRecordId == id);
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