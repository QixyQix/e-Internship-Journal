using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using E_Internship_Journal.Data;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using E_Internship_Journal.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace E_Internship_Journal.API
{
    [Produces("application/json")]
    [Route("api/UserManager")]
    public class UserManagerController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        public UserManagerController(ApplicationDbContext context, UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _context = context;
        }

        // GET: api/UserManager
        [HttpGet]
        public IActionResult Get()
        {
            List<object> applicationUserList = new List<object>();

            var users = from user in _context.Users
                        select new
                        {
                            Id = user.Id,
                            UserName = user.UserName,
                            Email = user.Email,
                            FullName = user.FullName,
                            Roles = _context.Roles.Where(input => user.Roles.Select(r => r.RoleId).Contains(input.Id)).Select(r => r.Name)
                        };

            return new JsonResult(users);
        }
        [HttpGet("GetUserAccounts")]
        public IActionResult GetUserAccounts()
        {
            List<object> activatedUser = new List<object>();
            List<object> deactivatedUser = new List<object>();
            //var qq = _context.Roles.Where(input => _context.Users.Include(e => e.Roles).Where(r => r.Role).Contains(input.Id)).Select(r => r.Name);
            //var qww = _context.UserRoles.Where(input => input.UserId == )
            //  var pk = _context.Users.Include(e => e.Roles).Select(input => _context.Roles;
            activatedUser = GetActivatedUser();
            deactivatedUser = GetDeactivatedUser();
            //foundActivatedBatch.Where(foundEntity => foundEntity.UserId == oneUser.Id).Select(br=> new { BatchName = br.Batch.BatchName });
            // foundActivatedBatch.Where(foundEntity => foundEntity.UserId == oneUser.Id);

            //var activatedUser = from user in _context.Users
            //                    select new
            //                    {
            //                        Id = user.Id,
            //                        UserName = user.UserName,
            //                        Email = user.Email,
            //                        FullName = user.FullName,
            //                        Roles = _context.Roles.Include(e=>e.Users).Where(rol => test.Contains(rol.Id))
            //                    };
            //var deactivatedUser = from user in _context.Users.Where(u => u.LockoutEnd.HasValue)
            //                      select new
            //                      {
            //                          Id = user.Id,
            //                          UserName = user.UserName,
            //                          Email = user.Email,
            //                          FullName = user.FullName,
            //                          Roles = _context.Roles.Where(input => user.Roles.Select(r => r.RoleId).Contains(input.Id)).Select(r => r.Name)
            //                      };

            return new JsonResult(new { ActivatedUser = activatedUser, DeactivatedUser = deactivatedUser });
            //return new JsonResult(new { ActivatedUsers = activatedUser , DeactivatedUser = deactivatedUser });
        }
        [HttpGet("GetOptionValue")]
        public IActionResult GetOptionValue()
        {
            List<object> applicationUserList = new List<object>();
            var excludeRoleString = "STUDENT";
            var roles = from role in _context.Roles.Where(r => !excludeRoleString.Contains(r.Name))
                        select new
                        {
                            role.Id,
                            role.Name
                        };

            var batches = from batch in _context.Batches
                          select new
                          {
                              batch.BatchId,
                              batch.BatchName
                          };

            var projects = from project in _context.Projects
                           select new
                           {
                               project.ProjectId,
                               project.ProjectName
                           };
            return new JsonResult(new { Roles = roles, Batches = batches, Projects = projects });
        }



        // GET: api/UserManager
        [HttpGet]
        public IEnumerable<string> Getttt()
        {
            return new string[] { "value1", "value2" };
        }

        // GET: api/UserManager/5
        [HttpGet("GetOneUser/{email}")]
        public async Task<IActionResult> GetOneUser(string email)
        {
            object response = null;
            if (await UserExistsAsync(email))
            {
                try
                {
                    var userId = (await _userManager.FindByEmailAsync(email)).Id;
                    var foundUser = _context.Users.Include(e => e.Roles).Where(u => u.Id == userId).SingleOrDefault();
                    var foundProject = _context.Projects;

                    response = new
                    {
                        FullName = foundUser.FullName,
                        BatchId = _context.UserBatches.Include(b => b.Batch).Where(fe => fe.UserId == userId).Select(selected => selected.Batch.BatchId).ToList<int>(),
                        ProjectId = _context.Projects.Where(fp => fp.SupervisorId == userId).Select(selected => selected.ProjectId).ToList<int>(),
                        ProjectName = _context.Projects.Where(fp => fp.SupervisorId == userId).Select(selected => selected.ProjectName).ToList<string>(),
                        Email = foundUser.Email,
                        MobileNo = foundUser.PhoneNumber,
                        Roles = _context.Roles.Where(input => foundUser.Roles.Select(r => r.RoleId).Contains(input.Id)).Select(r => r.Name)

                    };//end of creation of the response object


                }
                catch (Exception outerException)
                {
                    response = new { status = "fail", Message = outerException.InnerException.Message };
                }

            }

            return new JsonResult(response);
        }

        // POST: api/UserManager
        [HttpPost]
        public async Task<IActionResult> Post([FromBody]string value)
        {
            string databaseInnerExceptionMessage = "";
            var userNewInput = JsonConvert.DeserializeObject<dynamic>(value);
            List<object> messages = new List<object>();
            bool status = true; //This variable is used to track the overall success of all the database operations
            object response = null;
            //http://stackoverflow.com/questions/20444022/updating-user-data-asp-net-identity
            //Database is our database context set in this controller.
            //I used the following 2 lines of command to create a userStore which represents AspNetUser table in the DB.
            var userStore = new UserStore<ApplicationUser>(_context);

            var userManager = new UserManager<ApplicationUser>(userStore, null, null, null, null, null, null, null, null);

            //To obtain the full name information, use student.FullName.value
            //To obtain the email information, use student.Email.value
            var newUser = new ApplicationUser
            {
                UserName = userNewInput.Email.Value,
                FullName = userNewInput.Name.Value,
                Email = userNewInput.Email.Value,
                PhoneNumber = userNewInput.Mobile.Value

            };
            PasswordHasher<ApplicationUser> passwordHasher = new PasswordHasher<ApplicationUser>();
            newUser.PasswordHash = passwordHasher.HashPassword(newUser, "P@ssw0rd"); //More complex password
            newUser.SecurityStamp = Guid.NewGuid().ToString();
            newUser.NormalizedEmail = newUser.Email;
            newUser.NormalizedUserName = newUser.UserName;
            //I had problems with the command:
            //await userStore.AddToRoleAsync(newUser, userNewInput.SelectedRoleName.Value);
            //inside the try section. As a result, I need to read out the role name information.
            //selectedRoleName = userNewInput.SelectedRoleName.Value;
            try
            {
                await userManager.CreateAsync(newUser);
                await userManager.AddToRoleAsync(newUser, userNewInput.Role.Value);

                if (userNewInput.Batch.Value != null)
                {
                    UserBatch newUserBatch = new UserBatch
                    {
                        BatchId = Convert.ToInt32(userNewInput.Batch.Value),
                        UserId = newUser.Id
                    };

                    _context.UserBatches.Add(newUserBatch);
                    await _context.SaveChangesAsync();
                    //newUserBatch.UserId = (await _userManager.FindByEmailAsync(batchNewInput.SLOAssigned.Value)).Id;
                }
                //if (userNewInput.Project.Value != null)
                //{
                //    var existingProject = Convert.ToInt32(userNewInput.Project.Value);
                //    var foundExistingProject = _context.Projects.Find(existingProject);
                //    //foundExistingProject.
                //    Project newProject = new Project
                //    {
                //        ProjectId = foundExistingProject.ProjectId,
                //        ProjectName = foundExistingProject.ProjectName,
                //        CompanyID = foundExistingProject.CompanyID,
                //        SupervisorId = newUser.Id
                //    };
                //    _context.Projects.Add(newProject);
                //    await _context.SaveChangesAsync();
                //}

                response = new { Status = "success", Message = "Saved new user record." };


            }
            catch (Exception outerException)
            {
                response = new { status = "fail", Message = outerException.InnerException.Message };
            }
            return new JsonResult(response);
        }//End of Post()

        // PUT: api/UserManager/5
        [HttpPut("UpdateUser/{email}")]
        public async Task<IActionResult> UpdateUser(string email, [FromBody]string value)
        {
            var userChangeInput = JsonConvert.DeserializeObject<dynamic>(value);
            object response = null;

            var userStore = new UserStore<ApplicationUser>(_context);
            var userManager = new UserManager<ApplicationUser>(userStore, null, null, null, null, null, null, null, null);

            var id = (await _userManager.FindByEmailAsync(email)).Id;
            var oneUser = _context.Users
                .Where(item => item.Id == id).Include(p => p.Roles).FirstOrDefault();


            oneUser.UserName = userChangeInput.FullName.Value;
            oneUser.FullName = userChangeInput.FullName.Value;
            oneUser.PhoneNumber = userChangeInput.Mobile.Value;
            oneUser.Email = userChangeInput.Email.Value;


            try
            {
                var removeRoleFromUserResult = userManager.RemoveFromRoleAsync(oneUser, userChangeInput.OriginalRoleName.Value).Result;
                //Add the new role to the user.
                var addRoleToUserResult = userManager.AddToRoleAsync(oneUser, userChangeInput.SelectedRoleName.Value).Result;
                //Update the 
                var updateUserResult = userManager.UpdateAsync(oneUser).Result;
                userStore.Context.SaveChanges();

                //if (userChangeInput.OriginalProject.Value != null)
                //{
                //    var originalProject = (int)userChangeInput.OriginalProject.Value;
                //    var chanedPorject = userChangeInput.Project.Value;
                //    var oneProject = _context.Projects.Where(project => project.ProjectId == originalProject).Single();
                //    oneProject.ProjectId = chanedPorject;

                //    //var oneNewProject
                //    await _context.SaveChangesAsync();
                //    response = new { Status = "success", Message = "Saved new user record." };
                //}
                List<int> ChangedInput = userChangeInput.Batch.ToObject<List<int>>();
                List<int> OriginalBatch = userChangeInput.OriginalBatch.ToObject<List<int>>();
                if (userChangeInput.OriginalBatch != null)
                {
                    if (ChangedInput.Count >= OriginalBatch.Count)
                    {
                        foreach (var eachChange in ChangedInput)
                        {
                            if (!OriginalBatch.Contains(eachChange))
                            {
                                UserBatch newuserbatch = new UserBatch();
                                var newbatchid = (int)Convert.ToInt32(eachChange);
                                newuserbatch.BatchId = newbatchid;
                                newuserbatch.UserId = id;
                                _context.UserBatches.Add(newuserbatch);
                            }
                        }
                    }
                    else if (ChangedInput.Count <= OriginalBatch.Count)
                    {
                        foreach (var eachOriginal in OriginalBatch)
                        {
                            if (!ChangedInput.Contains(eachOriginal))
                            {
                                var originalBatchId = (int)Convert.ToInt32(eachOriginal);
                                var foundUserBatch = _context.UserBatches.Where(ub => ub.BatchId == originalBatchId).Where(ub => ub.UserId == id).SingleOrDefault();
                                _context.UserBatches.Remove(foundUserBatch);

                            }
                        }
                    }


                    //if (batchId == Convert.ToInt32(newChange.Value))
                    //if (ChangedInput.Contains(originalBatchId))
                    //for (int i = 0; i < userChangeInput.OriginalBatch.Count; i++)
                    //{
                    //    var originalBatchId = (int)Convert.ToInt32(userChangeInput.OriginalBatch[i].Value);
                    //if (ChangedInput.Count > userChangeInput.OriginalBatch.Count && !ChangedInput.Contains(originalBatchId))

                    // }
                    //if (ChangedInput.Count > userChangeInput.OriginalBatch.Count && !ChangedInput.Contains(originalBatchId))
                    //{
                    //    UserBatch newuserbatch = new UserBatch();
                    //    var newbatchid = (int)Convert.ToInt32(ChangedInput[i]);
                    //    newuserbatch.BatchId = newbatchid;
                    //    newuserbatch.UserId = id;
                    //    _context.UserBatches.Add(newuserbatch);
                    //var foundUserBatch = _context.UserBatches.Where(ub => ub.BatchId == originalBatchId).Where(ub => ub.UserId == id).SingleOrDefault();
                    //var newBatchId = (int)Convert.ToInt32(ChangedInput[i]);
                    //foundUserBatch.BatchId = newBatchId;
                    //ChangedInput.Remove(originalBatchId);
                    //}
                    //else if (ChangedInput.Count < userChangeInput.OriginalBatch.Count && !ChangedInput.Contains(originalBatchId))
                    //{
                    //    var foundUserBatch = _context.UserBatches.Where(ub => ub.BatchId == originalBatchId).Where(ub => ub.UserId == id).SingleOrDefault();
                    //    _context.UserBatches.Remove(foundUserBatch);
                    //}
                    //else
                    //{
                    //    UserBatch newUserBatch = new UserBatch();
                    //    var newBatchId = (int)Convert.ToInt32(ChangedInput[i]);
                    //    newUserBatch.BatchId = newBatchId;
                    //    newUserBatch.UserId = id;
                    //    _context.UserBatches.Add(newUserBatch);
                    //}

                    await _context.SaveChangesAsync();
                }
                else
                {
                    if (ChangedInput.Count != 0)
                    {
                        foreach (var eachChange in ChangedInput)
                        {

                            UserBatch newuserbatch = new UserBatch();
                            var newbatchid = (int)Convert.ToInt32(eachChange);
                            newuserbatch.BatchId = newbatchid;
                            newuserbatch.UserId = id;
                            _context.UserBatches.Add(newuserbatch);

                        }
                        await _context.SaveChangesAsync();
                    }
                }

                //for (int o =0; o < ChangedInput.Count; o++)
                //{
                //        UserBatch newuserbatch = new UserBatch();
                //        var newbatchid = (int)Convert.ToInt32(ChangedInput[o]);
                //        newuserbatch.BatchId = newbatchid;
                //        newuserbatch.UserId = id;
                //        _context.UserBatches.Add(newuserbatch);


                //}




                response = new { Status = "success", Message = "Saved new user record." };
            }
            catch (Exception outerException)
            {
                response = new { status = "fail", Message = outerException.InnerException.Message };
            }
            return new JsonResult(response);
        }

        // DELETE: api/ApiWithActions/5
        [HttpPut("ActivateAccount/{email}")]
        public async Task<IActionResult> ActivateAccount(string email)
        {
            object response = null;
            var foundUser = (await _userManager.FindByEmailAsync(email));
            try
            {
                var userStore = new UserStore<ApplicationUser>(_context);
                var userManager = new UserManager<ApplicationUser>(userStore, null, null, null, null, null, null, null, null);
                //.SetLockoutEndDateAsync
                await userManager.SetLockoutEndDateAsync(foundUser, null);
                //response = new { status = "Update Successfully" };

                List<object> activatedUser = new List<object>();
                List<object> deactivatedUser = new List<object>();
                activatedUser = GetActivatedUser();
                deactivatedUser = GetDeactivatedUser();
                response = new { ActivatedUser = activatedUser, DeactivatedUser = deactivatedUser };
            }
            catch (Exception outerException)
            {
                response = new { status = "fail", Message = outerException.InnerException.Message };
            }
            return new JsonResult(response);
            //var foundOneUser = UserManager
        }
        [HttpPut("DisableAccount/{email}")]
        public async Task<IActionResult> DisableAccount(string email)
        {
            object response = null;
            var foundUser = (await _userManager.FindByEmailAsync(email));
            try
            {
                var userStore = new UserStore<ApplicationUser>(_context);
                var userManager = new UserManager<ApplicationUser>(userStore, null, null, null, null, null, null, null, null);
                //.SetLockoutEndDateAsync
                await userManager.SetLockoutEndDateAsync(foundUser, DateTimeOffset.MaxValue);
                List<object> activatedUser = new List<object>();
                List<object> deactivatedUser = new List<object>();
                activatedUser = GetActivatedUser();
                deactivatedUser = GetDeactivatedUser();
                response = new { ActivatedUser = activatedUser, DeactivatedUser = deactivatedUser };
            }
            catch (Exception outerException)
            {
                response = new
                {
                    status = "fail",
                    Message = outerException.InnerException.Message
                };
            }
            return new JsonResult(response);
        }
        private List<object> GetActivatedUser()
        {
            List<object> activatedUser = new List<object>();
            var excludeRoleString = "ADMIN";

            var ignoreAdmin = _context.Roles.Where(r => !excludeRoleString.Contains(r.Name)).Select(role => role.Id);
            var foundActivated = _context.Users.Include(e => e.Roles).Where(u => u.LockoutEnd == null);

            var foundActivatedBatch = _context.UserBatches.Include(b => b.Batch);
            var foundProject = _context.Projects;
            foreach (var oneRole in ignoreAdmin)
            {
                var users = foundActivated.Where(roleEntity => roleEntity.Roles.Any(r => r.RoleId == oneRole));
                foreach (var oneUser in users)
                {

                    activatedUser.Add(new
                    {
                        Id = oneUser.Id,
                        UserName = oneUser.UserName,
                        BatchName = foundActivatedBatch.Where(fe => fe.UserId == oneUser.Id).Select(selected => selected.Batch.BatchName).ToList<string>(),
                        ProjectName = foundProject.Where(fp => fp.SupervisorId == oneUser.Id).Select(selected => selected.ProjectName).ToList<string>(),
                        Email = oneUser.Email,
                        FullName = oneUser.FullName,
                        Roles = _context.Roles.Where(input => oneUser.Roles.Select(r => r.RoleId).Contains(input.Id)).Select(r => r.Name)
                    });

                }
            }


            return activatedUser;

        }//End of private function

        private List<Object> GetDeactivatedUser()
        {
            List<object> deactivatedUser = new List<object>();
            var excludeRoleString = "ADMIN";

            var ignoreAdmin = _context.Roles.Where(r => !excludeRoleString.Contains(r.Name)).Select(role => role.Id);
            var foundActivatedBatch = _context.UserBatches.Include(b => b.Batch);
            var foundProject = _context.Projects;

            var foundDeactivated = _context.Users.Include(e => e.Roles).Where(u => u.LockoutEnd.HasValue);
            foreach (var oneRole in ignoreAdmin)
            {
                var users = foundDeactivated.Where(roleEntity => roleEntity.Roles.Any(r => r.RoleId == oneRole));
                foreach (var oneUser in users)
                {

                    deactivatedUser.Add(new
                    {
                        Id = oneUser.Id,
                        UserName = oneUser.UserName,
                        BatchName = foundActivatedBatch.Where(fe => fe.UserId == oneUser.Id).Select(selected => selected.Batch.BatchName).ToList<string>(),
                        ProjectName = foundProject.Where(fp => fp.SupervisorId == oneUser.Id).Select(selected => selected.ProjectName).ToList<string>(),
                        Email = oneUser.Email,
                        FullName = oneUser.FullName,
                        Roles = _context.Roles.Where(input => oneUser.Roles.Select(r => r.RoleId).Contains(input.Id)).Select(r => r.Name)
                    });

                }//End of nested foreach loop
            }// End of ForeachLoop

            return deactivatedUser;
        }
        private async Task<bool> UserExistsAsync(string email)
        {
            var id = (await _userManager.FindByEmailAsync(email)).Id;
            return _context.ApplicationUsers.Any(e => e.Id == id);
        }
    }
}
