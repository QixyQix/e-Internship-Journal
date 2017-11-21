using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using E_Internship_Journal.Data;
using E_Internship_Journal.Models;
using Microsoft.AspNetCore.Identity;

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
    }
}