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
        public ProjectsController(ApplicationDbContext context , UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _context = context;
        }
        
        // GET: api/Projects
        [HttpGet]
        [AllowAnonymous]
        public IEnumerable<Project> GetProjectsAsync()
        {
            return _context.Projects;
        }

        // GET: api/Projects/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetProject([FromRoute] int id)
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

            return Ok(project);
        }

        // PUT: api/Projects/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutProject([FromRoute] int id, [FromBody] Project project)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != project.ProjectId)
            {
                return BadRequest();
            }

            _context.Entry(project).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProjectExists(id))
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
                Project newProject = new Project();
                newProject.ProjectName = projectNewInput.ProjectName;
                newProject.CompanyID = projectNewInput.CompanyID;
                newProject.SupervisorId = await _userManager.FindByEmailAsync(projectNewInput.SupervisorEmail);
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