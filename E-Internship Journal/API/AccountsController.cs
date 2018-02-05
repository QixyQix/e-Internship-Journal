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
using Microsoft.AspNetCore.Authorization;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using System.IO;
using E_Internship_Journal.Services;

namespace E_Internship_Journal.API
{
    [Produces("application/json")]
    [Route("api/Accounts")]
    public class AccountsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IEmailSender _emailSender;
        public AccountsController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, IEmailSender emailSender,
            SignInManager<ApplicationUser> signInManager)
        {
            _userManager = userManager;
            _emailSender = emailSender;
            _signInManager = signInManager;
            _context = context;
        }

        [HttpGet]
        [Authorize(Roles = "ADMIN")]
        public IActionResult GetAllUsers()
        {
            List<object> userObjects = new List<object>();

            var users = _userManager.Users.Include(u => u.Roles).Include(u => u.UserBatches).ThenInclude(ub => ub.Batch).ToList();
            var roles = _context.Roles.ToList();

            foreach (var user in users)
            {

                List<string> rolesList = new List<string>();
                List<object> batchList = new List<object>();

                //Roles
                foreach (var role in user.Roles)
                {
                    foreach (var appRole in roles)
                    {
                        if (role.RoleId.Equals(appRole.Id))
                            rolesList.Add(appRole.Name);
                    }
                }

                //Batch
                foreach (var ub in user.UserBatches)
                {
                    var batch = ub.Batch;
                    batchList.Add(new
                    {
                        BatchId = batch.BatchId,
                        BatchName = batch.BatchName,
                        Description = batch.Description,
                        StartDate = batch.StartDate,
                        EndDate = batch.EndDate
                    });
                }

                userObjects.Add(new
                {
                    Id = user.Id,
                    Email = user.Email,
                    UserName = user.UserName,
                    FullName = user.FullName,
                    PhoneNumber = user.PhoneNumber,
                    Activated = user.IsEnabled,
                    Roles = rolesList,
                    Batches = batchList
                });

            }
            return new OkObjectResult(userObjects);
        }

        [HttpGet("{id}")]
        public IActionResult GetUser([FromRoute] string id)
        {

            var user = _userManager.Users.Include(u => u.Roles).Include(u => u.UserBatches).ThenInclude(ub => ub.Batch).Where(u => u.Id.Equals(id)).SingleOrDefault();
            var roles = _context.Roles.ToList();

            List<string> rolesList = new List<string>();
            List<object> batchList = new List<object>();

            if (user == null)
            {
                return new BadRequestObjectResult(new { Message = "User not found" });
            }

            //Roles
            foreach (var role in user.Roles)
            {
                foreach (var appRole in roles)
                {
                    if (role.RoleId.Equals(appRole.Id))
                        rolesList.Add(appRole.Name);
                }
            }

            //Batch
            foreach (var ub in user.UserBatches)
            {
                var batch = ub.Batch;
                batchList.Add(new
                {
                    BatchId = batch.BatchId,
                    BatchName = batch.BatchName,
                    Description = batch.Description,
                    StartDate = batch.StartDate,
                    EndDate = batch.EndDate
                });
            }

            return new OkObjectResult(new
            {
                Id = user.Id,
                Email = user.Email,
                UserName = user.UserName,
                FullName = user.FullName,
                PhoneNumber = user.PhoneNumber,
                Activated = user.IsEnabled,
                Roles = rolesList,
                Batches = batchList
            });
        }

        [HttpGet("UsersInRole/{role}")]
        public async Task<IActionResult> GetUsersInRoleAsync([FromRoute] string role)
        {
            var users = await _userManager.GetUsersInRoleAsync(role);

            List<object> userlist = new List<object>();

            foreach (var user in users)
            {
                userlist.Add(new
                {
                    user.Id,
                    user.Email,
                    user.UserName,
                    user.StudentId,
                    user.FullName,
                    user.IsEnabled,
                    user.PhoneNumber
                });
            }

            return Ok(userlist);
        }


        [HttpGet("LiaisonOfficers")]
        public async Task<IActionResult> LiaisonOfficers()
        {
            List<object> LOObjects = new List<object>();

            var LOUsers = _userManager.GetUsersInRoleAsync("LO").Result;

            foreach (var user in LOUsers)
            {
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
                        .ThenInclude(b => b.Batch)
                        .ThenInclude(c => c.Course)
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
                        SupervisorName = _context.Users.Where(p => p.Id.Equals(internshipRecord.Project.SupervisorId)).SingleOrDefault().FullName,
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

        [HttpPost]
        [Authorize(Roles = "ADMIN")]
        public async Task<IActionResult> CreateNewUserAsync([FromBody] string value)
        {
            var newUserInput = JsonConvert.DeserializeObject<dynamic>(value);

            string email = newUserInput.Email.Value.ToString().Trim();

            //Check if user exists
            if (_context.Users.Any(u => u.Email.Equals(email)))
            {
                return new BadRequestObjectResult(new { Message = "A user with the email already exists!" });
            }

            //Check if roles are correct first
            List<string> roleList = new List<string>();

            foreach (var role in newUserInput.Roles)
            {
                roleList.Add(role.ToString());
            }

            if (roleList.Any(role => role.Equals("STUDENT") && (role.Equals("SLO") || role.Equals("LO") || role.Equals("SUPERVISOR") || role.Equals("ADMIN"))))
            {
                return new BadRequestObjectResult(new { Message = "User cannot be assigned more than 1 role if student" });
            }
            else if (roleList.Any(role => role.Equals("ADMIN") && (role.Equals("SLO") || role.Equals("LO") || role.Equals("SUPERVISOR") || role.Equals("STUDENT"))))
            {
                return new BadRequestObjectResult(new { Message = "User cannot be assigned more than 1 role if admin" });
            }

            //Create user
            var userStore = new UserStore<ApplicationUser>(_context);
            var userManager = new UserManager<ApplicationUser>(userStore, null, null, null, null, null, null, null, null);

            var newUser = new ApplicationUser
            {
                UserName = newUserInput.Email.Value,
                Email = newUserInput.Email.Value,
                FullName = newUserInput.FullName.Value,
                PhoneNumber = newUserInput.PhoneNumber.Value,
                IsEnabled = Boolean.Parse(newUserInput.Enabled.Value.ToString())
            };

            PasswordHasher<ApplicationUser> ph = new PasswordHasher<ApplicationUser>();
            if (newUserInput.Password.Value.ToString().Equals(newUserInput.PasswordRepeat.Value.ToString()))
            {
                newUser.PasswordHash = ph.HashPassword(newUser, newUserInput.Password.Value.ToString());
            }
            else
            {
                return new BadRequestObjectResult(new { Message = "Passwords do not match" });
            }

            await userManager.CreateAsync(newUser);

            foreach (string role in roleList)
            {
                if (role.Equals("STUDENT") || role.Equals("ADMIN") || role.Equals("SLO") || role.Equals("LO") || role.Equals("SUPERVISOR"))
                {
                    await userManager.AddToRoleAsync(newUser, role);
                }
            }
            if (roleList.Contains("SLO") || roleList.Contains("LO"))
            {
                var newUserEmail = newUser.Email;
                var newUserName = newUser.FullName;
                //var role = roleList.Contains("SLO") ? null : "test";
                await _emailSender.SendChangeEmailAsync(false, newUserEmail, "Your account has been created!",
                    "Hi, " + newUserName, "Your account has been created!" +
                    "Kindly proceed to activate/login your account." , "" , "");
            }
            


            return new OkObjectResult(new { Message = "User created successfully!" });
        }

        // POST: api/Courses
        [HttpPost("BulkAddUser")]
        public async Task<IActionResult> BulkAddUsers()
        {
            List<string> messageList = new List<string>();
            var alertType = "success";
            string title = "Success";
            var files = Request.Form.Files;

            var csvFile = files[0];

            using (var memoryStream = new MemoryStream())
            {
                List<string> csvLine = new List<string>();
                await csvFile.CopyToAsync(memoryStream);
                memoryStream.Position = 0;

                using (var streamReader = new StreamReader(memoryStream))
                {
                    var heading = streamReader.ReadLine();
                    string[] headingArray = heading.Split(',');
                    //Check if CSV file is in correct order
                    if (!headingArray[0].Equals("Email", StringComparison.OrdinalIgnoreCase) || !headingArray[1].Equals("Full Name", StringComparison.OrdinalIgnoreCase) || !headingArray[2].Equals("Phone Number", StringComparison.OrdinalIgnoreCase) || !headingArray[3].Equals("Password", StringComparison.OrdinalIgnoreCase))
                    {
                        return new BadRequestObjectResult(new { Message = "CSV file does not follow correct format" });
                    }

                    //Read the file
                    string fileLine = "";

                    while ((fileLine = streamReader.ReadLine()) != null)
                    {
                        csvLine.Add(fileLine);
                    }
                }

                var userStore = new UserStore<ApplicationUser>(_context);
                var userManager = new UserManager<ApplicationUser>(userStore, null, null, null, null, null, null, null, null);

                foreach (var line in csvLine)
                {
                    if (!(line.Replace(",", "").Trim().Equals("")))
                    {
                        try
                        {
                            //Get individual data
                            string[] userData = line.Split(',');

                            if (!_userManager.Users.Any(u => u.Email.Equals(userData[0])))
                            {
                                //Create user


                                var newUser = new ApplicationUser
                                {
                                    UserName = userData[0],
                                    Email = userData[0],
                                    FullName = userData[1],
                                    PhoneNumber = userData[2],
                                    IsEnabled = false
                                };

                                PasswordHasher<ApplicationUser> ph = new PasswordHasher<ApplicationUser>();

                                newUser.PasswordHash = ph.HashPassword(newUser, userData[3]);
                                await userManager.CreateAsync(newUser);
                            }
                            else
                            {
                                if (messageList.Count < 1)
                                {
                                    messageList.Add("Completed with the following errors:");
                                    alertType = "warning";
                                    title = "Completed with errors";
                                }

                                messageList.Add(userData[1] + "(" + userData[0] + ") Another user already exists and was therefore not created");
                            }
                        }
                        catch (Exception ex)
                        {
                            return BadRequest();
                        }
                    }
                }
                await _context.SaveChangesAsync();

                if (messageList.Count < 1)
                    messageList.Add("Courses in csv file created successfully! Please assign their roles individually.");

                return new OkObjectResult(new { Title = title, Messages = messageList, AlertType = alertType });
            }
        }


        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUserAsync(string id, [FromBody] string value)
        {
            var newUserInput = JsonConvert.DeserializeObject<dynamic>(value);

            //Get the user
            var user = _userManager.Users.Where(u => u.Id.Equals(id)).Include(u => u.UserBatches).Include(u => u.Projects).SingleOrDefault();

            if (user == null)
            {
                return new BadRequestObjectResult(new { Message = "User does not exist" });
            }

            string email = newUserInput.Email.Value.ToString().Trim();

            //Check if user exists
            if (!user.Email.Equals(email, StringComparison.OrdinalIgnoreCase))
            {
                if (_context.Users.Any(u => u.Email.Equals(email)))
                {
                    return new BadRequestObjectResult(new { Message = "A user with the email already exists!" });
                }
            }

            //Check if roles are correct first
            List<string> roleList = new List<string>();

            foreach (var role in newUserInput.Roles)
            {
                roleList.Add(role.ToString());
            }

            if (roleList.Any(role => role.Equals("STUDENT") && (role.Equals("SLO") || role.Equals("LO") || role.Equals("SUPERVISOR") || role.Equals("ADMIN"))))
            {
                return new BadRequestObjectResult(new { Message = "User cannot be assigned more than 1 role if student" });
            }
            else if (roleList.Any(role => role.Equals("ADMIN") && (role.Equals("SLO") || role.Equals("LO") || role.Equals("SUPERVISOR") || role.Equals("STUDENT"))))
            {
                return new BadRequestObjectResult(new { Message = "User cannot be assigned more than 1 role if admin" });
            }

            //Update user
            var userStore = new UserStore<ApplicationUser>(_context);
            var userManager = new UserManager<ApplicationUser>(userStore, null, null, null, null, null, null, null, null);

            user.UserName = newUserInput.Email.Value;
            user.Email = newUserInput.Email.Value;
            user.FullName = newUserInput.FullName.Value;
            user.PhoneNumber = newUserInput.PhoneNumber.Value;
            user.IsEnabled = Boolean.Parse(newUserInput.Enabled.Value.ToString());

            //Change password if necessary
            if (!(newUserInput.Password.Value.ToString() == null || newUserInput.Password.Value.ToString().Equals("")))
            {
                PasswordHasher<ApplicationUser> ph = new PasswordHasher<ApplicationUser>();
                if (newUserInput.Password.Value.ToString().Equals(newUserInput.PasswordRepeat.Value.ToString()))
                {
                    user.PasswordHash = ph.HashPassword(user, newUserInput.Password.Value.ToString());
                }
            }


            //Roles
            var roles = await _userManager.GetRolesAsync(user);
            await _userManager.RemoveFromRolesAsync(user, roles.ToArray());

            foreach (string role in roleList)
            {
                if (role.Equals("STUDENT") || role.Equals("ADMIN") || role.Equals("SLO") || role.Equals("LO") || role.Equals("SUPERVISOR"))
                {
                    await userManager.AddToRoleAsync(user, role);
                }
            }

            return new OkObjectResult(new { Message = "User updated successfully!" });
        }

        [HttpPut("BulkActivate")]
        public IActionResult BulkActivate([FromBody] string value)
        {
            var bulkActivateInput = JsonConvert.DeserializeObject<dynamic>(value);

            foreach (var idinput in bulkActivateInput)
            {
                string id = idinput.ToString();
                var user = _userManager.Users.Where(u => u.Id.Equals(id)).SingleOrDefault();

                if (user != null)
                {
                    user.IsEnabled = true;
                }

            }

            _context.SaveChanges();
            return new OkObjectResult(new { Message = "Users activated successfully" });
        }

        [HttpPut("BulkDeactivate")]
        public IActionResult BulkDeactivate([FromBody] string value)
        {
            var bulkActivateInput = JsonConvert.DeserializeObject<dynamic>(value);

            foreach (var idinput in bulkActivateInput)
            {
                string id = idinput.ToString();
                var user = _userManager.Users.Where(u => u.Id.Equals(id)).SingleOrDefault();

                if (user != null)
                {
                    user.IsEnabled = false;
                }

            }

            _context.SaveChanges();
            return new OkObjectResult(new { Message = "Users activated successfully" });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUserAsync(string id)
        {

            var user = _userManager.Users.Where(u => u.Id.Equals(id))
                .Include(u => u.UserBatches)
                .Include(u => u.InternshipRecords)
                .Include(u => u.Projects)
                .SingleOrDefault();

            if (user != null)
            {
                if (user.UserBatches.Count > 0 || user.InternshipRecords.Count > 0 || user.Projects.Count > 0)
                {
                    return new OkObjectResult(new { Message = "User could not be deleted. Account is attached to an internship record or project", AlertType = "error", Title = "Error" });
                }
                else
                {
                    await _userManager.DeleteAsync(user);
                    _context.SaveChanges();
                }
            }
            return new OkObjectResult(new { Message = "User deleted successfully", AlertType = "success", Title = "User Deleted" });
        }

        [HttpDelete("BulkDelete")]
        public async Task<IActionResult> BulkDeleteAsync([FromBody] string value)
        {
            var bulkActivateInput = JsonConvert.DeserializeObject<dynamic>(value);
            List<string> messageList = new List<string>();

            foreach (var idinput in bulkActivateInput)
            {
                string id = idinput.ToString();
                var user = _userManager.Users.Where(u => u.Id.Equals(id))
                    .Include(u => u.UserBatches)
                    .Include(u => u.InternshipRecords)
                    .Include(u => u.Projects)
                    .SingleOrDefault();

                if (user != null)
                {
                    if (user.UserBatches.Count > 0 || user.InternshipRecords.Count > 0 || user.Projects.Count > 0)
                    {
                        messageList.Add(user.FullName + " could not be deleted. Account is attached to an internship record or project");
                    }
                    else
                    {
                        await _userManager.DeleteAsync(user);
                    }
                }
            }

            _context.SaveChanges();
            if (messageList.Count < 1)
            {
                messageList.Add("All users deleted successfully");
                return new OkObjectResult(new { Messages = messageList, AlertType = "success" });
            }
            else
            {
                return new OkObjectResult(new { Messages = messageList, AlertType = "warning" });
            }
        }

        [HttpGet("loggedInUserDetails")]
        public IActionResult GetLoggedInUserDetail()
        {
            string userId = _userManager.GetUserId(User);
            ApplicationUser user = _userManager.Users.Where(u => u.Id.Equals(userId)).SingleOrDefault();

            if (user != null)
            {
                return new OkObjectResult(new
                {
                    FullName = user.FullName,
                    PhoneNumber = user.PhoneNumber,
                    Email = user.Email
                });
            }
            else
            {
                return BadRequest();
            }
        }

        [HttpPut("updateParticulars")]
        public IActionResult UpdateParticulars([FromBody] string value) {
            string userId = _userManager.GetUserId(User);
            ApplicationUser user = _userManager.Users.Where(u => u.Id.Equals(userId)).SingleOrDefault();

            var particularsInput = JsonConvert.DeserializeObject<dynamic>(value);
            string email = particularsInput.Email.Value.ToString().Trim();

            if (user != null && _userManager.Users.Any(u => u.Email.Equals(email, StringComparison.OrdinalIgnoreCase)))
            {
                user.PhoneNumber = particularsInput.PhoneNumber.Value.ToString().Trim();
                user.Email = email;
                user.UserName = email;

                _context.SaveChanges();

                return new OkObjectResult(new
                {
                    Message = "Updated particulars successully"
                });
            }
            else
            {
                return BadRequest(new { Message = "Another user with that email already exists!" });
            }
        }

        private async Task<bool> UserExistsAsync(string email)
        {
            var id = (await _userManager.FindByEmailAsync(email)).Id;
            return _context.ApplicationUsers.Any(e => e.Id == id);
        }
    }
}