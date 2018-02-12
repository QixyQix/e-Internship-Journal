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
using E_Internship_Journal.Services;

namespace E_Internship_Journal.API
{
    [Produces("application/json")]
    [Route("api/Projects")]
    public class ProjectsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IEmailSender _emailSender;
        public ProjectsController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, IEmailSender emailSender,
            SignInManager<ApplicationUser> signInManager)
        {
            _userManager = userManager;
            _emailSender = emailSender;
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
                        foundProject.Supervisor.FullName,
                        foundProject.Supervisor.Id,
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

            Project project = await _context.Projects.Where(p => p.ProjectId == id).Include(p => p.Supervisor).Include(c=>c.Company).SingleOrDefaultAsync();

            if (project != null)
            {
                var projectNewInput = JsonConvert.DeserializeObject<dynamic>(value);

                project.ProjectName = projectNewInput.ProjectName.Value;
                project.CompanyID = Convert.ToInt32(projectNewInput.Company.Value);
                string email = projectNewInput.SupervisorEmail.Value.ToString().Trim();



                if (project.Supervisor.Email.Equals(email, StringComparison.OrdinalIgnoreCase))
                {
                    //Update project
                    project.Supervisor.FullName = projectNewInput.SupervisorName.Value.ToString().Trim();
                    project.Supervisor.PhoneNumber = projectNewInput.SupervisorNumber.Value.ToString().Trim();
                }
                else
                {
                    var user = _userManager.Users.Any(u => u.Email.Equals(email, StringComparison.OrdinalIgnoreCase));

                    if (user)
                    {
                        return BadRequest();
                    }

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
                    project.Supervisor = newSupervisorUser;

                    await _context.SaveChangesAsync();

                    //Registration Pin
                    var repeatPinGeneration = true;
                    string registationPin;
                    RegistrationPin createdRegistrationPin = new RegistrationPin();
                    do
                    {
                        registationPin = generateRandomString(50);
                        if (!_context.RegistrationPins.Any(rp => rp.RegistrationPinId.Equals(registationPin)))
                        {
                            //Create new registration pin
                            var newRegistrationPin = new RegistrationPin
                            {
                                User = newSupervisorUser,
                                RegistrationPinId = generateRandomString(50)
                            };
                            createdRegistrationPin = newRegistrationPin;
                            _context.RegistrationPins.Add(newRegistrationPin);
                            repeatPinGeneration = false;
                        }
                    } while (repeatPinGeneration);

                    var absoluteUri = string.Concat(
      Request.Scheme,
      "://",
      Request.Host.ToUriComponent(),
      Request.PathBase.ToUriComponent());
                    await _emailSender.SendChangeEmailAsync(true, newSupervisorUser.Email, "Your account has been created!",
               "Hi, " + newSupervisorUser.FullName, "Your supervisor account has been created on behalf of you." +
               "Your account has been assigned to Project " + project.ProjectName + " and Company " + project.Company.CompanyName + ". Kindly proceed to activate your account.",
               absoluteUri + "/Account/SetPassword?registrationPin=" + createdRegistrationPin.RegistrationPinId, "Activate Account");
                }

            }


            await _context.SaveChangesAsync();
            return new OkObjectResult(new { Messages = "Updated project", AlertType = "success" });
        }

        [HttpPut("SaveNewProjectRecord")]
        [Authorize(Roles = "SLO, ADMIN")]
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
                    if (projectNewInput.SupervisorName.Value.Equals(checkSupervisorExist.FullName, StringComparison.OrdinalIgnoreCase) && (projectNewInput.SupervisorNumber.Value.Equals(checkSupervisorExist.PhoneNumber)))
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

                    var createdProject = new Project();
                    var registrationPin = new RegistrationPin();
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
                        createdProject = newProject;
                        await _context.SaveChangesAsync();
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

                        //_context.SaveChanges();

                        createdProject = projectRecord;
                        await _context.SaveChangesAsync();
                    }

                    //Registration Pin
                    //Registration Pin
                    var repeatPinGeneration = true;
                    string registationPin;
                    do
                    {
                        registationPin = generateRandomString(50);
                        if (!_context.RegistrationPins.Any(rp => rp.RegistrationPinId.Equals(registationPin)))
                        {
                            //Create new registration pin
                            var newRegistrationPin = new RegistrationPin
                            {
                                User = newSupervisorUser,
                                RegistrationPinId = generateRandomString(50)
                            };
                            registrationPin = newRegistrationPin;
                            _context.RegistrationPins.Add(newRegistrationPin);
                            repeatPinGeneration = false;
                        }
                    } while (repeatPinGeneration);
                    await _context.SaveChangesAsync();

                    var supervisorEmail = newSupervisorUser.Email;
                    var supervisorName = newSupervisorUser.FullName;
                    var projectName = createdProject.ProjectName;
                    var CompanyName = _context.Companies.Where(c => c.CompanyId == createdProject.CompanyID).Select(c => c.CompanyName).Single();
                    var absoluteUri = string.Concat(
      Request.Scheme,
      "://",
      Request.Host.ToUriComponent(),
      Request.PathBase.ToUriComponent());
                    await _emailSender.SendChangeEmailAsync(true, supervisorEmail, "Your account has been created and enrolled!",
                        "Hi, " + supervisorName, "Your supervisor account has been created on behalf of you." +
                        "Your account has been assigned to Project " + projectName + " and Company " + CompanyName + ". Kindly proceed to activate your account.",
                        absoluteUri + "/Account/SetPassword?registrationPin=" + registrationPin.RegistrationPinId, "Activate Account");


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
        [Authorize(Roles = "SLO, ADMIN")]
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

        // DELETE: api/Projects/5
        [HttpPut("bulkDelete")]
        [Authorize(Roles = "SLO, ADMIN")]
        public async Task<IActionResult> BulKDeleteProject([FromBody]string value)
        {
            var idList = JsonConvert.DeserializeObject<dynamic>(value);

            string alertType = "success";
            string title = "Action Completed";
            List<String> messageList = new List<String>();

            try
            {
                foreach (var idstr in idList)
                {
                    int projId = Int32.Parse(idstr.ToString());

                    var project = _context.Projects.Where(p => p.ProjectId == projId).Include(p => p.InternshipRecords).SingleOrDefault();

                    if (project.InternshipRecords.Count > 0)
                    {
                        messageList.Add(project.ProjectName + " could not be deleted as it is attached to internship records.");
                        alertType = "warning";
                    }
                    else
                    {
                        _context.Projects.Remove(project);

                    }
                }
                if (messageList.Count < 1)
                    messageList.Add("Deleted all projects successfully");
                _context.SaveChanges();
                return new OkObjectResult(new { Messages = messageList, AlertType = alertType, Title = title });
            }
            catch (Exception exceptionObject)
            {
                object httpFailRequestResultMessage = new { Message = "Unable to Process" };
                //Return a bad http response message to the client
                return BadRequest(httpFailRequestResultMessage);
            }
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "ADMIN, SLO")]
        public async Task<IActionResult> DeleteAProject(int id)
        {
            var project = _context.Projects.Where(p => p.ProjectId == id).Include(p => p.InternshipRecords).SingleOrDefault();

            if (project.InternshipRecords.Count > 0)
            {
                return new OkObjectResult(new { Message = "This project could not be deleted as there are internship records attached to it", AlertType = "warning" });
            }
            else
            {
                _context.Projects.Remove(project);
                _context.SaveChanges();
            }

            return new OkObjectResult(new { Message = "Deleted project", AlertType = "success" });
        }

        [HttpPost("MassEnrollProjects/")]
        [Authorize(Roles = "SLO, ADMIN")]
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
                            var company = _context.Companies.Include(pr => pr.Projects).Where(companyData => companyData.CompanyName.Equals(oneProjectData[1], StringComparison.OrdinalIgnoreCase)).SingleOrDefault();
                            // var user = _context.ApplicationUsers.SingleOrDefault(appuser => appuser.UserName.Equals(oneStudentData[1], StringComparison.OrdinalIgnoreCase));
                            var checkError = false;
                            if (!Regex.IsMatch(oneProjectData[3], "^[0-9]+$"))
                            {
                                alertType = "warning";
                                messageList.Add("Enter valid Phone Number at Row " + currentRow);
                                checkError = true;
                                // return BadRequest(FailedMessage);
                            }
                            if (company != null)
                            {
                                if (company.Projects.Select(pr => pr.ProjectName).Contains(oneProjectData[0]))
                                {
                                    alertType = "warning";
                                    messageList.Add("Duplicate Project Name at Row " + currentRow);
                                    checkError = true;
                                }
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
                                messageList.Add("Unable to locate Company Details at Row " + currentRow);// oneProjectData[1]);
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