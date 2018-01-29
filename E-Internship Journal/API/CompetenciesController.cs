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
using Microsoft.AspNetCore.Authorization;

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
        [HttpGet("getInternshipCompetenciesByCourse/")]
        [Authorize(Roles = "SLO")]
        public IActionResult GetInternshipCompetenciesByCourse(int id, string Deleted)
        {
            List<object> competencies_List = new List<object>();
            List<CompetencyTitle> ttt = new List<CompetencyTitle>();
            var competencies = _context.CompetencyTitles.Where(c => c.CourseId == id)
                .Include(competency => competency.Competencies)
                .Include(course => course.Course)
                .ToList();
            var aa = _context.Competencies.Include(c => c.CompetencyTitle).ThenInclude(s => s.Course)
                .Where(qq => qq.CompetencyTitle.Course.CourseId == id);
            try
            {
                if (Deleted.Equals("Y"))
                {
                    foreach (var oneCompetency in competencies)
                    {
                        //oneCompetency.Competencies
                        competencies_List.Add(new
                        {
                            oneCompetency.CompetencyTitleId,
                            oneCompetency.CourseId,
                            oneCompetency.Course.CourseName,
                            oneCompetency.TitleCompetency,
                            oneCompetency.ViewBy,
                            CompetencyList = oneCompetency.Competencies.Select(desc => new
                            {
                                CompetencyId = desc.CompetencyId,
                                Description = desc.Description,
                                CreatedBy = desc.CreatedBy,
                                DeletedAt = desc.DeletedAt,
                                desc.DeletedBy,
                                ViewBy = desc.ViewBy
                            }).Where(d => d.DeletedAt != null)
                        });
                    }
                }
                else
                {
                    foreach (var oneCompetency in competencies)
                    {
                        //   List<int> categoryIdList = new List<int>();

                        competencies_List.Add(new
                        {
                            oneCompetency.CompetencyTitleId,
                            oneCompetency.CourseId,
                            oneCompetency.Course.CourseName,
                            oneCompetency.TitleCompetency,
                            oneCompetency.ViewBy,
                            CompetencyList = oneCompetency.Competencies.Select(desc => new
                            {
                                CompetencyId = desc.CompetencyId,
                                Description = desc.Description,
                                CreatedBy = desc.CreatedBy,
                                DeletedAt = desc.DeletedAt,
                                ModifiedAt =
                                desc.ModifiedAt,
                                ModifiedBy = desc.ModifiedBy,
                                ViewBy = desc.ViewBy
                            }).Where(d => d.DeletedAt == null)
                        });
                    }
                }


            }
            catch (Exception ex)
            {

            }


            return new JsonResult(competencies_List);
        }

        [HttpGet("getInternshipCompetncies/{id}")]
        //Student / Supervisor / LO  Retrieve the whole Competency for the form
        public IActionResult GetInternshipCompetncies(int id)
        {
            Internship_Record internshipRecord = _context.Internship_Records.Where(ir => ir.InternshipRecordId == id)
                .Include(ir => ir.UserBatch)
                .ThenInclude(ub => ub.Batch)
                .ThenInclude(batch => batch.Course)
                .ThenInclude(course => course.CompetencyTitle)

                // .ThenInclude(course => course.Competencies)
                .Include(ir => ir.UserBatch)
                .ThenInclude(ub => ub.User)
                .SingleOrDefault();

            if (internshipRecord == null)
            {
                return BadRequest(new { Message = "This internshipRecord does not exist." });
            }

            List<object> competencyObjList = new List<object>();
            var foundCompetency = _context.CompetencyTitles.Include(c => c.Competencies).Where(ct => ct.CourseId == internshipRecord.UserBatch.Batch.Course.CourseId).ToList();
            foreach (var oneCompetency in foundCompetency)
            {

                //if (competency.DeletedAt == null)
                //{
                //    competencyObjList.Add(new
                //    {
                //        CompetencyId = competency.CompetencyId,
                //        // TitleDescription = competency.TitleDescription,
                //        Description = competency.Description,
                //        DeletedAt = competency.DeletedAt,
                //        ModifiedBy = competency.ModifiedBy,
                //        ModifiedAt = competency.ModifiedAt,
                //        CreatedBy = competency.CreatedBy,
                //        CourseId = competency.CourseId
                //    });
                //}

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

            return new OkObjectResult(competencyObjList);

        }
        [HttpPut("SaveNewCompetencyTitleRecord")]
        public async Task<IActionResult> SaveNewCompetencyTitleRecord([FromBody] string value)
        {
            string messageList = "";
            string alertType = "success";
            var CompetencyTitle_NewInput = JsonConvert.DeserializeObject<dynamic>(value);
            try
            {
                if (CompetencyTitle_NewInput.Method.Value.Equals("SaveNew"))
                {
                    CompetencyTitle newCompetencyTitle = new CompetencyTitle
                    {
                        CourseId = Convert.ToInt32(CompetencyTitle_NewInput.Id.Value),
                        TitleCompetency = CompetencyTitle_NewInput.Title.Value,
                        ViewBy = Convert.ToInt32(CompetencyTitle_NewInput.ViewBy.Value),
                    };
                    _context.CompetencyTitles.Add(newCompetencyTitle);
                    _context.SaveChanges();
                    messageList = "Saved Competency";
                    alertType = "success";
                }
                else
                {
                    int existingCompetencyTitleId = Convert.ToInt32(CompetencyTitle_NewInput.Id.Value);
                    var foundCompetencyTitle = _context.CompetencyTitles.Where(ct => ct.CompetencyTitleId == existingCompetencyTitleId).SingleOrDefault();

                    foundCompetencyTitle.TitleCompetency = CompetencyTitle_NewInput.Title.Value;
                    foundCompetencyTitle.ViewBy = Convert.ToInt32(CompetencyTitle_NewInput.ViewBy.Value);


                    _context.SaveChanges();
                    messageList = "Saved Competency";
                    alertType = "success";
                }
            }
            catch (Exception exceptionObject)
            {
                object httpFailRequestResultMessage =
                new { Message = exceptionObject };
                return BadRequest(httpFailRequestResultMessage);
            }
            var responseObject = new
            {
                AlertType = alertType,
                Messages = messageList
            };

            return new OkObjectResult(responseObject);
        }

        [HttpPut("SaveNewCompetenciesRecord")]
        public async Task<IActionResult> SaveNewCompetenciesRecord([FromBody] string value)
        {
            string messageList = "";
            string alertType = "success";
            var competencies_NewInput = JsonConvert.DeserializeObject<dynamic>(value);
            try
            {
                if (competencies_NewInput.Method.Value.Equals("SaveNew"))
                {
                    Competency newCompetencies = new Competency
                    {
                        CompetencyTitleId = Convert.ToInt32(competencies_NewInput.Id.Value),
                        Description = competencies_NewInput.Description.Value,
                        ViewBy = Convert.ToInt32(competencies_NewInput.ViewBy.Value),
                        CreatedBy = _userManager.GetUserName(User)
                        //TitleDescription = competencies_NewInput.TitleDescription.Value
                    };
                    _context.Competencies.Add(newCompetencies);
                    _context.SaveChanges();
                    messageList = "Saved Competency";
                    alertType = "success";
                }
                else
                {
                    int existingCompetencyId = Convert.ToInt32(competencies_NewInput.Id.Value);
                    var foundCompetencies = _context.Competencies.Where(cp => cp.CompetencyId == existingCompetencyId).SingleOrDefault();

                    foundCompetencies.Description = competencies_NewInput.Description.Value;
                    foundCompetencies.ViewBy = Convert.ToInt32(competencies_NewInput.ViewBy.Value);
                    foundCompetencies.ModifiedBy = _userManager.GetUserName(User);
                    foundCompetencies.ModifiedAt = DateTime.Now;

                    _context.SaveChanges();
                    messageList = "Saved Competency";
                    alertType = "success";
                }
            }
            catch (Exception exceptionObject)
            {
                object httpFailRequestResultMessage =
                new { Message = exceptionObject };
                return BadRequest(httpFailRequestResultMessage);
            }
            var responseObject = new
            {
                AlertType = alertType,
                Messages = messageList
            };

            return new OkObjectResult(responseObject);
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
                        //TitleDescription = foundOneCompetency.TitleDescription
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
                        //TitleDescription = competencies_NewInput.TitleDescription.Value
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
        [HttpDelete("DeleteCompetencies/{id}")]
        public async Task<IActionResult> DeleteCompetency([FromRoute] int id)
        {
            string alertType = "success";
            var competency = await _context.Competencies.SingleOrDefaultAsync(m => m.CompetencyId == id);
            if (competency == null)
            {
                return NotFound();
            }
            try
            {
                // _context.Competencies.Remove(competency);
                // await _context.SaveChangesAsync();
                competency.DeletedAt = DateTime.Now;
                competency.DeletedBy = (await _userManager.FindByNameAsync(_userManager.GetUserName(User))).FullName;
                _context.SaveChanges();
                var responseObject = new
                {
                    AlertType = alertType,
                    Messages = "Success"
                };

                return new OkObjectResult(responseObject);
            }
            catch (Exception exceptionObject)
            {
                object httpFailRequestResultMessage = new { Message = "Unable to Process" };
                //Return a bad http response message to the client
                return BadRequest(httpFailRequestResultMessage);
            }


        }
        [HttpDelete("DeleteCategory/{id}")]
        public async Task<IActionResult> DeleteCategory([FromRoute] int id)
        {
            string alertType = "success";
            var competency = await _context.Competencies.SingleOrDefaultAsync(m => m.CompetencyId == id);
            if (competency == null)
            {
                return NotFound();
            }
            try
            {
                // _context.Competencies.Remove(competency);
                // await _context.SaveChangesAsync();
                competency.DeletedAt = DateTime.Now;
                _context.SaveChanges();
                var responseObject = new
                {
                    AlertType = alertType,
                    Messages = "Success"
                };

                return new OkObjectResult(responseObject);
            }
            catch (Exception exceptionObject)
            {
                object httpFailRequestResultMessage = new { Message = "Unable to Process" };
                //Return a bad http response message to the client
                return BadRequest(httpFailRequestResultMessage);
            }


        }

        private bool CompetencyExists(int id)
        {
            return _context.Competencies.Any(e => e.CompetencyId == id);
        }
    }
}