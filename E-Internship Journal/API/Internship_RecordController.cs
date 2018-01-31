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

namespace E_Internship_Journal.API
{
    [Authorize]
    [Produces("application/json")]
    [Route("api/Internship_Record")]
    public class Internship_RecordController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public Internship_RecordController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
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
                .Where(ir => ir.UserBatch.UserId.Equals(userId) && ir.UserBatch.Batch.StartDate <= DateTime.Now && ir.UserBatch.Batch.EndDate >= DateTime.Now)
                .Single();

            var internshiprecordId = internshipRecord.InternshipRecordId;

            if (internshipRecord == null)
            {
                return NotFound(new { Message = "No internship record found" });
            }
            else
            {
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
        public IActionResult UpdateCompanyChecklist(int id, [FromBody] string value) {
            var internshipRecord = _context.Internship_Records
                .Where(ir => ir.InternshipRecordId == id)
                .Include(ir => ir.Project)
                .ThenInclude(p => p.Supervisor)
                .SingleOrDefault();

            if (internshipRecord == null)
            {
                return new BadRequestObjectResult(new { Message = "Internship record does not exist!" });
            }
            else if (!internshipRecord.Project.Supervisor.Id.Equals(_userManager.GetUserId(User))) {
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
            catch (Exception e) {
                return new BadRequestObjectResult(new { Message = e.Message.ToString()});
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
        public IActionResult GetStudentInfo(int id) {
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

            if (sir == null) {
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

        private bool Internship_RecordExists(int id)
        {
            return _context.Internship_Records.Any(e => e.InternshipRecordId == id);
        }
    }
}