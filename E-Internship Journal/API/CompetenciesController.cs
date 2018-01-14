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
using Microsoft.AspNetCore.Identity;

namespace E_Internship_Journal.API
{
    [Produces("application/json")]
    [Route("api/Competencies")]
    public class CompetenciesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        public CompetenciesController(UserManager<ApplicationUser> userManager, ApplicationDbContext context)
        {
            _userManager = userManager;
            _context = context;
        }

        // GET: api/Competencies
        [HttpGet]
        public IActionResult GetCompetencies()
        {
            List<object> competencies_List = new List<object>();
            var competencies = _context.Competencies

            .AsNoTracking();
            foreach (var oneCompetency in competencies)
            {
                //   List<int> categoryIdList = new List<int>();
                competencies_List.Add(new
                {
                    oneCompetency.Description,
                    oneCompetency.TitleDescription
                });
            }

            return new JsonResult(competencies_List);
        }

        [HttpGet("getInternshipCompetncies/{id}")]
        public IActionResult GetInternshipCompetncies(int id) {
            Internship_Record internshipRecord = _context.Internship_Records.Where(ir => ir.InternshipRecordId == id)
                .Include(ir => ir.UserBatch)
                .ThenInclude(ub => ub.Batch)
                .ThenInclude(batch => batch.Course)
                .ThenInclude(course => course.Competencies)
                .Include(ir => ir.UserBatch)
                .ThenInclude(ub => ub.User)
                .SingleOrDefault();

            if (internshipRecord == null)
            {
                return BadRequest(new { Message = "This internshipRecord does not exist." });
            }

            List<object> competencyObjList = new List<object>();

            foreach (var competency in internshipRecord.UserBatch.Batch.Course.Competencies) {
                if (competency.DeletedAt != null) {
                    competencyObjList.Add(new
                    {
                        CompetencyId = competency.CompetencyId,
                        TitleDescription = competency.TitleDescription,
                        Description = competency.Description,
                        DeletedAt = competency.DeletedAt,
                        ModifiedBy = competency.ModifiedBy,
                        ModifiedAt = competency.ModifiedAt,
                        CreatedBy = competency.CreatedBy,
                        CourseId = competency.CourseId
                    });
                }
            }

            return new OkObjectResult(competencyObjList);

        }

        // GET: api/Competencies/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetCompetency([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            if (CompetencyExists(id))
            {
                try
                {
                    var foundOneCompetency = _context.Competencies
                        .Where(eachCompetencyEntity => eachCompetencyEntity.CompetencyId == id)
                        .Single();
                    var response = new
                    {
                        CompetencyId = foundOneCompetency.CompetencyId,
                        Description = foundOneCompetency.Description,
                        TitleDescription = foundOneCompetency.TitleDescription
                    };//end of creation of the response object
                    return new JsonResult(response);
                }
                catch (Exception exceptionObject)
                {
                    //Create a fail message anonymous object
                    //This anonymous object only has one Message property 
                    //which contains a simple string message
                    object httpFailRequestResultMessage =
                    new { Message = "Unable to obtain Competencies information." };
                    //Return a bad http response message to the client
                    return BadRequest(httpFailRequestResultMessage);
                }
            }
            else
            {
                object httpFailRequestResultMessage =
                new { Message = "Unable to locate Competencies Item information." };
                //Return a bad http response message to the client
                return BadRequest(httpFailRequestResultMessage);


            }//End of Get(id) Web API method
        }

        // PUT: api/Competencies/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCompetency(int id, [FromBody] string value)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (CompetencyExists(id))
            {
                var competencies_NewInput = JsonConvert.DeserializeObject<dynamic>(value);
                try
                {
                    var foundOneCompetencies = _context.Competencies
                        .Where(eachCompetencyEntity => eachCompetencyEntity.CompetencyId == id)
                        .Single();

                    Competency newCompetencies = new Competency
                    {
                        CompetencyId = id,
                        Description = competencies_NewInput.Description.Value,
                        TitleDescription = competencies_NewInput.TitleDescription.Value
                    };
                    _context.Competencies.Update(newCompetencies);
                    await _context.SaveChangesAsync();

                    var successRequestResultMessage = new
                    {
                        Message = "Updated Competencies into database"
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
                new { Message = "Competencies ID not found" };
                //Return a bad http response message to the client
                return BadRequest(httpFailRequestResultMessage);


            }//End of Get(id) Web API method
        }

        // POST: api/Competencies
        [HttpPost]
        public async Task<IActionResult> SaveNewCompetenciesInfromation([FromBody] string value)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            string customMessage = "";
            try
            {

                var competencies_NewInput = JsonConvert.DeserializeObject<dynamic>(value);
                Task_Record newTask_Record = new Task_Record
                {
                    Description = competencies_NewInput.Description.Value,
                    //DayRecordId = competencies_NewInput.DayRecordId.Value,

                };

                _context.Tasks.Add(newTask_Record);
                await _context.SaveChangesAsync();

            }
            catch (Exception exceptionObject)
            {
                customMessage = "Unable to save to database";
                //return 
            }
            var successRequestResultMessage = new
            {
                Message = "Saved Task into database"
            };

            OkObjectResult httpOkResult =
            new OkObjectResult(successRequestResultMessage);
            return httpOkResult;
        }

        // DELETE: api/Competencies/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCompetency([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var competency = await _context.Competencies.SingleOrDefaultAsync(m => m.CompetencyId == id);
            if (competency == null)
            {
                return NotFound();
            }

            _context.Competencies.Remove(competency);
            await _context.SaveChangesAsync();

            return Ok(competency);
        }

        private bool CompetencyExists(int id)
        {
            return _context.Competencies.Any(e => e.CompetencyId == id);
        }
    }
}