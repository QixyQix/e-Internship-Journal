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
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using System.Text;
using System.IO;
using System.Text.RegularExpressions;

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
                        Email = foundProject.Supervisor.Email,
                        foundProject.Supervisor.PhoneNumber,
                        foundProject.Supervisor.FullName
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
            //string tqq = "ADMIN@TEST.com";
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

        [HttpPut("SaveNewProjectRecord")]
        [Authorize(Roles = "SLO")]
        public async Task<IActionResult> SaveNewProjectRecord([FromBody] string value)
        {
            string messageList = "";
            string alertType = "success";
            var projectNewInput = JsonConvert.DeserializeObject<dynamic>(value);
            //var projectRecord = _context.TouchPointRecords
            //    .Where(tr => tr.TouchPointId == id).SingleOrDefault();
            // var www = projectNewInput.sss;

            try
            {
                var checkSupervisorExist = (ApplicationUser)await _userManager.FindByEmailAsync(projectNewInput.SupervisorEmail.Value);

                int companyId = Convert.ToInt32(projectNewInput.Company.Value);
                var existingProject = _context.Projects.Where(pr => pr.CompanyID == companyId).ToList();
                var checkError = false;
                //foreach (var oneExistingProject in existingProject)
                //{
                //    if (oneExistingProject.ProjectName.Equals(projectNewInput.ProjectName.Value, StringComparison.OrdinalIgnoreCase))
                //    {
                //        messageList = "Duplicate Project in the same company";
                //        alertType = "error";
                //        checkError = true;
                //        continue;
                //    }
                //}
                //if (checkError == true)
                //{
                //    var responseObject = new
                //    {
                //        AlertType = alertType,
                //        Messages = messageList
                //    };

                //    return new OkObjectResult(responseObject);
                //}


                //Check if supervisor acount exist
                if (checkSupervisorExist != null)
                {
                    if (projectNewInput.ProjectId == null)
                    {
                        foreach (var oneExistingProject in existingProject)
                        {
                            if (oneExistingProject.ProjectName.Equals(projectNewInput.ProjectName.Value, StringComparison.OrdinalIgnoreCase))
                            {
                                messageList = "Duplicate Project in the same company";
                                alertType = "error";
                                checkError = true;
                                continue;
                            }
                        }
                        if (checkError == true)
                        {
                            return new OkObjectResult(new
                            {
                                AlertType = alertType,
                                Messages = messageList
                            });
                        }
                        //Save new project
                        Project newProject = new Project
                        {
                            ProjectName = projectNewInput.ProjectName.Value,
                            CompanyID = Convert.ToInt32(projectNewInput.Company.Value),
                            SupervisorId = checkSupervisorExist.Id
                        };
                        _context.Projects.Add(newProject);

                        messageList = "Saved Project";
                        alertType = "success";

                    }
                    else
                    {
                        foreach (var oneExistingProject in existingProject)
                        {
                            if (oneExistingProject.ProjectName.Equals(projectNewInput.ProjectName.Value, StringComparison.OrdinalIgnoreCase) && oneExistingProject.ProjectId != Convert.ToInt32(projectNewInput.ProjectId.Value))
                            {
                                messageList = "Duplicate Project in the same company";
                                alertType = "error";
                                checkError = true;
                                continue;
                            }
                        }
                        if (checkError == true)
                        {
                            return new OkObjectResult(new
                            {
                                AlertType = alertType,
                                Messages = messageList
                            });
                        }
                        var projectId = (int)Int32.Parse(projectNewInput.ProjectId.Value);
                        var projectRecord = _context.Projects.Where(pr => pr.ProjectId == projectId).SingleOrDefault();

                        projectRecord.ProjectName = projectNewInput.ProjectName.Value;
                        projectRecord.CompanyID = Int32.Parse(projectNewInput.Company.Value);
                        messageList = "Saved Project";
                        alertType = "success";

                    }
                    //Check if Supervisor name & Phone Number matches with existing record in the DB
                    if (checkSupervisorExist.FullName.Equals(projectNewInput.SupervisorName.Value, StringComparison.OrdinalIgnoreCase) && (checkSupervisorExist.PhoneNumber.Equals(projectNewInput.SupervisorNumber.Value)))
                    {
                        //Do nothing
                        messageList = "Saved Project";
                        alertType = "success";
                        _context.SaveChanges();
                    }
                    else
                    {
                        checkSupervisorExist.FullName = projectNewInput.SupervisorName.Value;
                        checkSupervisorExist.PhoneNumber = projectNewInput.SupervisorNumber.Value;
                        messageList = "Supervisor details has been overriden";
                        alertType = "warning";
                        _context.SaveChanges();
                        // Return Status code with existing Supervisor account value for further action from the user
                        //return StatusCode(202, new { checkSupervisorExist.FullName, checkSupervisorExist.PhoneNumber });
                    }
                    var responseObject = new
                    {
                        AlertType = alertType,
                        Messages = messageList
                    };
                    return new OkObjectResult(responseObject);
                }
                else
                {
                    //Create user
                    var userStore = new UserStore<ApplicationUser>(_context);
                    var userManager = new UserManager<ApplicationUser>(userStore, null, null, null, null, null, null, null, null);
                    var newSupervisorUser = new ApplicationUser
                    {
                        UserName = projectNewInput.SupervisorEmail.Value,
                        Email = projectNewInput.SupervisorEmail.Value,
                        FullName = projectNewInput.SupervisorName.Value,
                        PhoneNumber = projectNewInput.SupervisorNumber.Value
                    };
                    PasswordHasher<ApplicationUser> ph = new PasswordHasher<ApplicationUser>();
                    newSupervisorUser.PasswordHash = ph.HashPassword(newSupervisorUser, generateRandomString(11));

                    await userManager.CreateAsync(newSupervisorUser);
                    await userManager.AddToRoleAsync(newSupervisorUser, "SUPERVISOR");

                    if (projectNewInput.ProjectId == null)
                    {
                        foreach (var oneExistingProject in existingProject)
                        {
                            if (oneExistingProject.ProjectName.Equals(projectNewInput.ProjectName.Value, StringComparison.OrdinalIgnoreCase))
                            {
                                messageList = "Duplicate Project in the same company";
                                alertType = "error";
                                checkError = true;
                                continue;
                            }
                        }
                        if (checkError == true)
                        {
                            return new OkObjectResult(new
                            {
                                AlertType = alertType,
                                Messages = messageList
                            });
                        }
                        //Save new project and assign with supervisor's new acount
                        Project newProject = new Project
                        {
                            ProjectName = projectNewInput.ProjectName.Value,
                            CompanyID = Convert.ToInt32(projectNewInput.Company.Value),
                            SupervisorId = newSupervisorUser.Id
                        };
                        _context.Projects.Add(newProject);
                    }
                    else
                    {
                        foreach (var oneExistingProject in existingProject)
                        {
                            if (oneExistingProject.ProjectName.Equals(projectNewInput.ProjectName.Value, StringComparison.OrdinalIgnoreCase))
                            {
                                messageList = "Duplicate Project in the same company";
                                alertType = "error";
                                checkError = true;
                                continue;
                            }
                        }
                        if (checkError == true)
                        {
                            return new OkObjectResult(new
                            {
                                AlertType = alertType,
                                Messages = messageList
                            });
                        }
                        var projectId = (int)Int32.Parse(projectNewInput.ProjectId.Value);
                        var projectRecord = _context.Projects.Where(pr => pr.ProjectId == projectId).SingleOrDefault();

                        projectRecord.ProjectName = projectNewInput.ProjectName.Value;
                        projectRecord.CompanyID = Int32.Parse(projectNewInput.Company.Value);
                        projectRecord.SupervisorId = newSupervisorUser.Id;

                        _context.SaveChanges();
                    }
                    var repeatPinGeneration = true;
                    do
                    {
                        var registationPin = generateRandomString(50);
                        if (!_context.RegistrationPins.Any(rp => rp.RegistrationPinId.Equals(registationPin)))
                        {
                            //Create new registration pin
                            var newRegistrationPin = new RegistrationPin
                            {
                                User = newSupervisorUser,
                                RegistrationPinId = generateRandomString(50)
                            };
                            _context.RegistrationPins.Add(newRegistrationPin);
                            repeatPinGeneration = false;
                        }
                    } while (repeatPinGeneration);

                    await _context.SaveChangesAsync();

                    messageList = "Saved Project & Created Supervisor Account";
                    alertType = "success";
                    var responseObject = new
                    {
                        AlertType = alertType,
                        Messages = messageList
                    };

                    return new OkObjectResult(responseObject);

                }


                //var supervisorRecord = _context.Users
                //    .Where(ur => ur.Id == exitingSupervisor);

            }
            catch (Exception exceptionObject)
            {
                return BadRequest(new
                {
                    exceptionObject.Message
                });
            }

        }//End of Http Put

        // DELETE: api/Projects/5
        [HttpDelete("DeleteProjects/bulk")]
        public async Task<IActionResult> DeleteProject([FromQuery]string selectedProjects)
        {
            var listOfId = selectedProjects.Split(',').Select(Int32.Parse).ToList();
            //   var project = "tr";
            //var project = await _context.Projects.SingleOrDefaultAsync(m => m.ProjectId == id);
            string alertType = "success";
            List<String> messageList = new List<String>();
            try
            {
                var foundProjects = _context.Projects.Include(ir => ir.InternshipRecords).ToList();

                foreach (var oneProject in foundProjects)
                {
                    if (listOfId.Contains(oneProject.ProjectId) && oneProject.InternshipRecords.Count != 0)
                    {
                        alertType = "warning";
                        messageList.Add(oneProject.ProjectName + " not removed");
                    }
                    else if (listOfId.Contains(oneProject.ProjectId) && oneProject.InternshipRecords.Count == 0)
                    {
                        _context.Projects.Remove(oneProject);
                        await _context.SaveChangesAsync();
                    }
                }

                //messageList.Add("Selected Records removed successfully!");
                var responseObject = new
                {
                    AlertType = alertType,
                    Messages = messageList
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
        [HttpPost("MassEnrollProjects/")]
        [Authorize(Roles = "SLO")]
        public async Task<IActionResult> MassEnrollProjects()
        {
            List<string> messageList = new List<string>();
            string alertType = "success";
            var files = Request.Form.Files;
            ////Get the batch & course
            //var thisBatch = _context.Batches
            //    .Where(batch => batch.BatchId == id)
            //    .Include(batch => batch.Course)
            //    .Single();

            var csvFile = files[0];

            using (var memoryStream = new MemoryStream())
            {
                List<string> csvLine = new List<string>();
                await csvFile.CopyToAsync(memoryStream);
                memoryStream.Position = 0;
                using (var streamReader = new StreamReader(memoryStream))
                {
                    var heading = streamReader.ReadLine();
                    //Check if CSV file is in correct order
                    if (!heading.Equals("Project Name,Company Name,Supervisor Name,Supervisor Contact Number,Supervisor Email"))
                    {
                        return BadRequest(new { Message = "CSV file does not follow correct format" });
                    }

                    //Read the file
                    string fileLine = "";

                    while ((fileLine = streamReader.ReadLine()) != null)
                    {
                        csvLine.Add(fileLine);
                    }
                }
                //  var projects = _context.Projects.Select(c)
                //var projects = from c in _context.Projects
                int currentRow = 2;
                foreach (var line in csvLine)
                {

                    if (!(line.Replace(",", "").Trim().Equals("")))
                    {
                        try
                        {


                            //Get individual data
                            string[] oneProjectData = line.Split(',');
                            //var www = oneProjectData[3];
                            var company = _context.Companies.Include(pr => pr.Projects).SingleOrDefault(companyData => companyData.CompanyName.Equals(oneProjectData[1], StringComparison.OrdinalIgnoreCase));
                            // var user = _context.ApplicationUsers.SingleOrDefault(appuser => appuser.UserName.Equals(oneStudentData[1], StringComparison.OrdinalIgnoreCase));
                            var checkError = false;
                            if (!Regex.IsMatch(oneProjectData[3], "^[0-9]+$"))
                            {
                                alertType = "warning";
                                messageList.Add("Enter valid Phone Number at Row " + currentRow);
                                checkError = true;
                                // return BadRequest(FailedMessage);
                            }
                            if (company.Projects.Select(pr => pr.ProjectName).Contains(oneProjectData[0]))
                            {
                                alertType = "warning";
                                messageList.Add("Duplicate Project Name at Row " + currentRow);
                                checkError = true;
                            }
                            //Regex reference http://www.rhyous.com/2010/06/15/regular-expressions-in-cincluding-a-new-comprehensive-email-pattern/
                            string emailPattern = @"^(([^<>()[\]\\.,;:\s@\""""]+(\.[^<>()[\]\\.,;:\s@\""""]+)*)|(\"""".+\""""))@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\])|(([a-zA-Z\-0-9]+\.)+[a-zA-Z]{2,}))$";
                            if (!Regex.IsMatch(oneProjectData[4], emailPattern))
                            {
                                alertType = "warning";
                                messageList.Add("Invalid Email at Row " + currentRow);
                                checkError = true;
                            }
                            if (checkError == true)
                            {
                                continue;
                            }
                            if (company != null)
                            {
                                //Check if Supervisor is already existed
                                var existingSupervisorAccount = await _userManager.FindByEmailAsync(oneProjectData[4]);
                                if (existingSupervisorAccount != null)
                                {
                                    if (existingSupervisorAccount.FullName.Equals(oneProjectData[2], StringComparison.OrdinalIgnoreCase) && (existingSupervisorAccount.PhoneNumber.Equals(oneProjectData[3])))
                                    {
                                        //Save new project
                                        Project newProject = new Project
                                        {

                                            ProjectName = oneProjectData[0],
                                            CompanyID = company.CompanyId,
                                            SupervisorId = existingSupervisorAccount.Id
                                        };
                                        _context.Projects.Add(newProject);
                                        //  _context.SaveChanges();
                                    }
                                    else
                                    {
                                        // Return Status code with existing Supervisor account value for further action from the user
                                        //return StatusCode(202, new { SupervisorName = oneProjectData[2], SupervisorContact = oneProjectData[3] });
                                        existingSupervisorAccount.FullName = oneProjectData[2];
                                        existingSupervisorAccount.PhoneNumber = oneProjectData[3];
                                        alertType = "warning";
                                        messageList.Add("Supervisor details for " + oneProjectData[2] + " has been overriden");
                                    }
                                }
                                else
                                {
                                    var userStore = new UserStore<ApplicationUser>(_context);
                                    var userManager = new UserManager<ApplicationUser>(userStore, null, null, null, null, null, null, null, null);
                                    var newSupervisorUser = new ApplicationUser
                                    {
                                        UserName = oneProjectData[4],
                                        Email = oneProjectData[4],
                                        FullName = oneProjectData[2],
                                        PhoneNumber = oneProjectData[3]
                                    };

                                    PasswordHasher<ApplicationUser> ph = new PasswordHasher<ApplicationUser>();
                                    newSupervisorUser.PasswordHash = ph.HashPassword(newSupervisorUser, generateRandomString(11));

                                    await userManager.CreateAsync(newSupervisorUser);
                                    await userManager.AddToRoleAsync(newSupervisorUser, "SUPERVISOR");

                                    //Save new project and assign with supervisor's new acount
                                    Project newProject = new Project
                                    {
                                        ProjectName = oneProjectData[0],
                                        CompanyID = company.CompanyId,
                                        SupervisorId = newSupervisorUser.Id
                                    };
                                    _context.Projects.Add(newProject);

                                    var repeatPinGeneration = true;
                                    do
                                    {
                                        var registationPin = generateRandomString(50);
                                        if (!_context.RegistrationPins.Any(rp => rp.RegistrationPinId.Equals(registationPin)))
                                        {
                                            //Create new registration pin
                                            var newRegistrationPin = new RegistrationPin
                                            {
                                                User = newSupervisorUser,
                                                RegistrationPinId = generateRandomString(50)
                                            };
                                            _context.RegistrationPins.Add(newRegistrationPin);
                                            repeatPinGeneration = false;
                                        }
                                    } while (repeatPinGeneration);

                                    // await _context.SaveChangesAsync();
                                }
                                await _context.SaveChangesAsync();
                            }
                            else
                            {
                                alertType = "warning";
                                messageList.Add("Unable to locate Company Details " + oneProjectData[1]);
                            }
                        }
                        catch (Exception ex)
                        {
                            return BadRequest();

                        }
                        finally
                        {
                            ++currentRow;
                        }
                    }
                }//End of foreach loop
            }//End of reading files 

            //messageList.Add("All Records saved successfully!");
            var responseObject = new
            {
                AlertType = alertType,
                Messages = messageList
            };

            return new OkObjectResult(responseObject);
        }
        private bool ProjectExists(int id)
        {
            return _context.Projects.Any(e => e.ProjectId == id);
        }
        public string generateRandomString(int size)
        {
            //ACKNOWLEDGEMENT: http://azuliadesigns.com/generate-random-strings-characters/
            StringBuilder builder = new StringBuilder();
            Random random = new Random();
            char ch;
            for (int i = 1; i < size + 1; i++)
            {
                ch = Convert.ToChar(Convert.ToInt32(Math.Floor(26 * random.NextDouble() + 65)));
                builder.Append(ch);
            }

            return builder.ToString();
        }
    }
}