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
using Microsoft.AspNetCore.Authorization;
using System.IO;

namespace E_Internship_Journal.API
{
    [Produces("application/json")]
    [Route("api/Companies")]
    public class CompaniesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CompaniesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/Companies
        [HttpGet]
        [Authorize(Roles = "SLO,ADMIN")]
        public IActionResult GetCompanies()
        {
            var Companies = _context.Companies
                .ToList();
            List<object> companyList = new List<object>();
            foreach (var oneCompany in Companies)
            {
                companyList.Add(new
                {
                    oneCompany.CompanyId,
                    oneCompany.CompanyName,
                    oneCompany.CompanyAddress,
                    oneCompany.ContactPersonNumber,
                    oneCompany.ContactPersonEmail,
                    oneCompany.ContactPersonName,
                    oneCompany.ContactPersonFax
                });
            }
            return new JsonResult(companyList);
        }

        // GET: api/Companies/5
        [HttpGet("{id}")]
        [Authorize(Roles = "SLO,ADMIN")]
        public async Task<IActionResult> GetCompany(int id)
        {
            if (CompanyExists(id))
            {
                try
                {
                    var foundCompany = await _context.Companies.SingleOrDefaultAsync(m => m.CompanyId == id);
                    var response = new
                    {
                        foundCompany.CompanyId,
                        foundCompany.CompanyName,
                        foundCompany.CompanyAddress,
                        foundCompany.ContactPersonName,
                        foundCompany.ContactPersonNumber,
                        foundCompany.ContactPersonEmail,
                        foundCompany.ContactPersonFax
                    };//end of creation of the response object
                    return new JsonResult(response);
                }
                catch (Exception ex)
                {
                    object httpFailRequestResultMessage =
                    new { Message = "Unable to obtain Company information." };
                    return BadRequest(httpFailRequestResultMessage);
                }

            }
            else
            {
                return NotFound();
            }

        }

        // PUT: api/Companies/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCompany(int id, [FromBody] string value)
        {
            if (CompanyExists(id))
            {
                var companyNewInput = JsonConvert.DeserializeObject<dynamic>(value);
                var foundOneCompany = _context.Companies.Find(id);

                foundOneCompany.CompanyName = companyNewInput.CompanyName.Value;
                foundOneCompany.CompanyAddress = companyNewInput.CompanyAddress.Value;
                foundOneCompany.ContactPersonName = companyNewInput.ContactName.Value;
                foundOneCompany.ContactPersonEmail = companyNewInput.ContactEmail.Value;
                foundOneCompany.ContactPersonNumber = companyNewInput.ContactNumber.Value;
                await _context.SaveChangesAsync();
            }
            else
            {
                return new BadRequestObjectResult(new { Message = "Company does not exist" });
            }

            return new OkObjectResult(new { Message = "Company updated successfully" });
        }

        // POST: api/Companies
        [HttpPut("SaveNewCompanyRecord")]
        [Authorize(Roles = "ADMIN, SLO")]
        public async Task<IActionResult> SaveNewCompanyRecord([FromBody] string value)
        {
            string messageList = "";
            string alertType = "success";
            var companyNewInputs = JsonConvert.DeserializeObject<dynamic>(value);

            try
            {

                if (companyNewInputs.CompanyId == null)
                {
                    string CompanyName = companyNewInputs.CompanyName.Value;
                    if (_context.Companies.Any(c => c.CompanyName.Equals(CompanyName, StringComparison.OrdinalIgnoreCase)))
                    {
                        messageList = "Duplcate Company";
                        alertType = "error";
                    }
                    else
                    {
                        //Save new Company
                        Company newCompany = new Company
                        {
                            CompanyName = CompanyName,
                            //  CompanyName = companyNewInputs.CompanyName.Value,
                            CompanyAddress = companyNewInputs.CompanyAddress.Value,
                            ContactPersonName = companyNewInputs.ContactName.Value,
                            ContactPersonEmail = companyNewInputs.ContactEmail.Value,
                            ContactPersonNumber = companyNewInputs.ContactNumber.Value,
                            ContactPersonFax = companyNewInputs.ContactFax.Value
                        };
                        _context.Companies.Add(newCompany);

                        messageList = "Saved Company";
                        alertType = "success";
                    }

                }//end of if else statement for new Company
                else
                {


                    string CompanyName = companyNewInputs.CompanyName.Value;
                    if (_context.Companies.Any(c => c.CompanyName.Equals(CompanyName, StringComparison.OrdinalIgnoreCase)))
                    {
                        messageList = "Duplcate Company";
                        alertType = "error";
                    }
                    else
                    {
                        int CompanyId = Int32.Parse(companyNewInputs.CompanyId.Value);
                        var foundCompany = _context.Companies.Where(c => c.CompanyId == CompanyId).Single();
                        //Update existing company
                        foundCompany.CompanyName = CompanyName;
                        foundCompany.CompanyAddress = companyNewInputs.CompanyAddress.Value;
                        foundCompany.ContactPersonName = companyNewInputs.ContactName.Value;
                        foundCompany.ContactPersonEmail = companyNewInputs.ContactEmail.Value;
                        foundCompany.ContactPersonNumber = companyNewInputs.ContactNumber.Value;
                        foundCompany.ContactPersonFax = companyNewInputs.ContactFax.Value;

                        messageList = "Updated Company";
                        alertType = "success";
                    }


                }

                await _context.SaveChangesAsync();
                var responseObject = new
                {
                    AlertType = alertType,
                    Messages = messageList
                };
                return new OkObjectResult(responseObject);
            }
            catch (Exception exceptionObject)
            {
                return BadRequest(new
                {
                    exceptionObject.Message
                });
            }
        }

        [HttpPost("MassEnrollCompanies/")]
        [Authorize(Roles = "SLO,ADMIN")]
        public async Task<IActionResult> MassEnrollCompanies()
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
                    if (!heading.Equals("Company Name,Company Address,Contact Person Name,Contact Person Number,Contact Person Email,Contact Person Fax"))
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

                int currentRow = 2;
                foreach (var line in csvLine)
                {

                    if (!(line.Replace(",", "").Trim().Equals("")))
                    {
                        try
                        {


                            //Get individual data
                            string[] oneCompanyData = line.Split(',');
                            //var www = oneProjectData[3];
                            var company = _context.Companies.SingleOrDefault(companyData => companyData.CompanyName.Equals(oneCompanyData[0], StringComparison.OrdinalIgnoreCase));
                            // var user = _context.ApplicationUsers.SingleOrDefault(appuser => appuser.UserName.Equals(oneStudentData[1], StringComparison.OrdinalIgnoreCase));

                            if (company == null)
                            {
                                Company newCompany = new Company
                                {
                                    CompanyName = oneCompanyData[0],
                                    CompanyAddress = oneCompanyData[1],
                                    ContactPersonName = oneCompanyData[2],
                                    ContactPersonNumber = oneCompanyData[3],
                                    ContactPersonEmail = oneCompanyData[4],
                                    ContactPersonFax = oneCompanyData[5]
                                };
                                _context.Companies.Add(newCompany);

                                await _context.SaveChangesAsync();
                            }
                            else
                            {
                                alertType = "warning";
                                messageList.Add("Company Name exist at Row " + currentRow);
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
        }//End of MassEnroll

        // DELETE: api/Companies/5
        [HttpDelete("DeleteCompany/bulk")]
        public async Task<IActionResult> DeleteCompany([FromQuery]string selectedCompanies)
        {
            var listOfId = selectedCompanies.Split(',').Select(Int32.Parse).ToList();
            //   var project = "tr";
            //var project = await _context.Projects.SingleOrDefaultAsync(m => m.ProjectId == id);
            string alertType = "success";
            List<String> messageList = new List<String>();
            try
            {
                var foundCompanies = _context.Companies.Include(ir => ir.Projects).ToList();

                foreach (var oneCompany in foundCompanies)
                {
                    if (listOfId.Contains(oneCompany.CompanyId) && oneCompany.Projects.Count != 0)
                    {
                        alertType = "warning";
                        messageList.Add(oneCompany.CompanyName + " not removed");
                    }
                    else if (listOfId.Contains(oneCompany.CompanyId) && oneCompany.Projects.Count == 0)
                    {
                        _context.Companies.Remove(oneCompany);
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

        // DELETE: api/Companies/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCompany(int id)
        {
            try
            {
                var company = _context.Companies.Include(ir => ir.Projects).Where(c => c.CompanyId == id).SingleOrDefault();

                if (company != null)
                {
                    if (company.Projects.Count > 0)
                    {
                        return new BadRequestObjectResult(new { Message = "Unable to delete company record - there are projects assigned to it" });
                    }
                    else
                    {
                        _context.Companies.Remove(company);
                        await _context.SaveChangesAsync();
                    }
                }

                return new OkObjectResult(new { Message = "Removed company successfully!"});

            }
            catch (Exception exceptionObject)
            {
                object httpFailRequestResultMessage = new { Message = "Unable to Process" };
                //Return a bad http response message to the client
                return BadRequest(httpFailRequestResultMessage);
            }


        }

        private bool CompanyExists(int id)
        {
            return _context.Companies.Any(e => e.CompanyId == id);
        }
    }
}