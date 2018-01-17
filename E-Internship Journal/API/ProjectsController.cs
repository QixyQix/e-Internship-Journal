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
using System.Security.Claims;

namespace E_Internship_Journal.API
{
    [Produces("application/json")]
    [Route("api/Projects")]
    public class ProjectsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        public ProjectsController(ApplicationDbContext context, UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _context = context;
        }

        // GET: api/Projects
        [HttpGet]
        [AllowAnonymous]
        public IActionResult GetProjects()
        {

            //var ww = rr.Id;
            //var ttt = _userManager.GetUserId(rr);
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            List<object> projectList = new List<object>();
            var projects = _context.Projects
            .Include(eacbProjectEntity => eacbProjectEntity.Supervisor)
            .Include(eacbProjectEntity => eacbProjectEntity.Company)
            .AsNoTracking();
            foreach (var oneProject in projects)
            {
                //var eachUser = _context.ApplicationUsers
                //    .Where(userItem => userItem.Id == oneProject.SupervisorId);

                //   List<int> categoryIdList = new List<int>();
                projectList.Add(new
                {
                    oneProject.ProjectId,
                    oneProject.ProjectName,
                    oneProject.Supervisor.PhoneNumber,
                    oneProject.Supervisor.Email,
                    oneProject.Supervisor.FullName,
                    oneProject.Company.CompanyName
                });
            }


            return new JsonResult(projectList);
        }
        [HttpGet("GetSupervisorCompany")]
        public async Task<IActionResult> GetSupervisorCompany()
        {
            var company = _context.Companies.AsNoTracking();
            var usersList = new List<Object>();
            //var user = new Object();
            foreach (ApplicationUser eachUser in _userManager.Users)
            {
                var roles = (await _userManager.GetRolesAsync(eachUser));
                if (roles.Contains("SUPERVISOR"))
                {
                    usersList.Add(new
                    {
                        Name = eachUser.FullName,
                        Email = eachUser.Email

                    });//End of Add
                }// End of If statement
            }//End of foreach loop

            var companyList = _context.Companies.Select(companyItem => new { CompanyId = companyItem.CompanyId, CompanyName = companyItem.CompanyName });

            return new JsonResult(new { UserList = usersList, CompanyList = companyList });
        }
        // GET: api/Projects/5
        [HttpGet("{id}")]
        public IActionResult GetProject(int id)
        {
            if (ProjectExists(id))
            {
                try
                {
                    var foundProject = _context.Projects
                    .Where(eacbProjectEntity => eacbProjectEntity.ProjectId == id)
                    .Include(eacbProjectEntity => eacbProjectEntity.Supervisor)
                    .Include(eacbProjectEntity => eacbProjectEntity.Company).Single();
                    var response = new
                    {
                        ProjectName = foundProject.ProjectName,
                        CompanyId = foundProject.Company.CompanyId,
                        CompanyName = foundProject.Company.CompanyName,
                        Email = foundProject.Supervisor.Email
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

        // PUT: api/Projects/5
        [HttpPut("UpdateOneProject/{id}")]
        public async Task<IActionResult> UpdateOneProject(int id, [FromBody] string value)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            string customMessage = "";
            if (ProjectExists(id))
            {
                var projectNewInput = JsonConvert.DeserializeObject<dynamic>(value);
                var foundOneProject = _context.Projects.Find(id);

                foundOneProject.ProjectName = projectNewInput.ProjectName.Value;
                foundOneProject.CompanyID = Convert.ToInt32(projectNewInput.Company.Value);
                foundOneProject.SupervisorId = (await _userManager.FindByEmailAsync(projectNewInput.Supervisor.Value)).Id;
                await _context.SaveChangesAsync();
            }
            else
            {

            }

            return NoContent();
        }

        // POST: api/Projects
        [HttpPost("SaveNewProjectInformation")]
        public async Task<IActionResult> SaveNewProjectInformation([FromBody] string value)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }                //string tqq = "ADMIN@TEST.com";
                             //_userManager.
                             //var userManager = await _userManager.FindByEmailAsync(tqq);
                             //userManager.Id;
                             //var qqq = userManager.Id;
                             //var claims = _userManager.GetClaimsAsync();
                             //_userManager.GetUserId((ClaimsPrincipal)qqq);
                             //_userManager.
                             //var ttt = await _userManager.GetClaimsAsync(userManager);
                             //var qw = _userManager.GetClaimsAsync(userManager);
                             //var user = User;
                             //var iden = (ClaimsIdentity)User;
                             //var claims = _userManager.GetClaimsAsync(userManager);
                             //IEnumerable<Claim> claims = iden.Claims;
                             //var ww = _context.ApplicationUsers.Find("ADMIN@TEST.com");
                             //var ttt = _userManager.GetUserId(userManager);
            string customMessage = "";
            try
            {

                var projectNewInput = JsonConvert.DeserializeObject<dynamic>(value);
                Project newProject = new Project
                {
                    ProjectName = projectNewInput.ProjectName.Value,
                    CompanyID = Convert.ToInt32(projectNewInput.Company.Value),
                    SupervisorId = (await _userManager.FindByEmailAsync(projectNewInput.Supervisor.Value)).Id
                };
                // newProject.SupervisorId = _userManager.FindByEmailAsync(projectNewInput.SupervisorEmail);
                //var ttt = _userManager.GetUserId(_userManager.FindByEmailAsync(projectNewInput.SupervisorEmail));

                _context.Projects.Add(newProject);
                await _context.SaveChangesAsync();

            }
            catch (Exception exceptionObject)
            {
                customMessage = "Unable to save to database";
                //return 
            }
            var successRequestResultMessage = new
            {
                Message = "Saved Course into database"
            };

            OkObjectResult httpOkResult =
            new OkObjectResult(successRequestResultMessage);
            return httpOkResult;
        }

        // DELETE: api/Projects/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProject([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var project = await _context.Projects.SingleOrDefaultAsync(m => m.ProjectId == id);
            if (project == null)
            {
                return NotFound();
            }

            _context.Projects.Remove(project);
            await _context.SaveChangesAsync();

            return Ok(project);
        }

        private bool ProjectExists(int id)
        {
            return _context.Projects.Any(e => e.ProjectId == id);
        }
    }
}