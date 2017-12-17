using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using E_Internship_Journal.Data;
using E_Internship_Journal.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace E_Internship_Journal.API
{
    [Produces("application/json")]
    [Route("api/Accounts")]
    public class AccountsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        public AccountsController(ApplicationDbContext context, UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _context = context;
        }

        [HttpGet("LiaisonOfficers")]
        public async Task<IActionResult> LiaisonOfficers(){
            List<object> LOObjects = new List<object>();

            var LOUsers = _userManager.GetUsersInRoleAsync("LO").Result;

            foreach (var user in LOUsers) {
                LOObjects.Add(new
                {
                    Name = user.FullName,
                    Email = user.Email,
                    UserId = user.Id
                });
            }

            return new JsonResult(LOObjects);
        }
        // GET: api/Accounts/5
        [HttpGet("GetStudentInfo/{email}")]
        public async Task<IActionResult> GetStudentInfo(string email)
        {
            object response = null;
            if (await UserExistsAsync(email))
            {
                try
                {
                    var userId = (await _userManager.FindByEmailAsync(email)).Id;
                    var foundUser = _context.Users.Where(u => u.Id == userId).SingleOrDefault();
                    //var foundProject = _context.Projects;
                    var internshipRecord = _context.Internship_Records
                        .Include(p => p.Project)
                        .ThenInclude(c => c.Company)
                        .Include(ub => ub.UserBatch)
                        .ThenInclude(b=>b.Batch)
                        .ThenInclude(c =>c.Course)
                        .Where(ir => ir.UserBatch.UserId.Equals(foundUser.Id))
                        .SingleOrDefault();
                    response = new
                    {
                        FullName = foundUser.FullName,
                        CourseCode = internshipRecord.UserBatch.Batch.Course.CourseCode,
                        MobileNo = foundUser.PhoneNumber,
                        Email = foundUser.Email,
                        CompanyName = internshipRecord.Project.Company.CompanyName,
                        CompanyAddress = internshipRecord.Project.Company.CompanyAddress,
                        SupervisorName = _context.Users.Where(p=>p.Id.Equals(internshipRecord.Project.SupervisorId)).SingleOrDefault().FullName,
                        SupervisorNumber = _context.Users.Where(p => p.Id.Equals(internshipRecord.Project.SupervisorId)).SingleOrDefault().PhoneNumber,
                        SupervisorEmail = _context.Users.Where(p => p.Id.Equals(internshipRecord.Project.SupervisorId)).SingleOrDefault().Email,
                        //FullName = foundUser.FullName,
                        //BatchId = _context.UserBatches.Include(b => b.Batch).Where(fe => fe.UserId == userId).Select(selected => selected.Batch.BatchId).ToList<int>(),
                        //ProjectId = _context.Projects.Where(fp => fp.SupervisorId == userId).Select(selected => selected.ProjectId).ToList<int>(),
                        //ProjectName = _context.Projects.Where(fp => fp.SupervisorId == userId).Select(selected => selected.ProjectName).ToList<string>(),
                        //Email = foundUser.Email,
                        //MobileNo = foundUser.PhoneNumber,
                        //Roles = _context.Roles.Where(input => foundUser.Roles.Select(r => r.RoleId).Contains(input.Id)).Select(r => r.Name)

                    };//end of creation of the response object
                    
                }
                catch (Exception outerException)
                {
                    response = new { status = "fail", Message = outerException.InnerException.Message };
                }

            }
            return new JsonResult(response);
        }
        private async Task<bool> UserExistsAsync(string email)
        {
            var id = (await _userManager.FindByEmailAsync(email)).Id;
            return _context.ApplicationUsers.Any(e => e.Id == id);
        }
    }
}