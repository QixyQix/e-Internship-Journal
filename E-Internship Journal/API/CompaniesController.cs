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
        public async Task<IActionResult> GetCompany([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var company = await _context.Companies.SingleOrDefaultAsync(m => m.CompanyId == id);

            if (company == null)
            {
                return NotFound();
            }

            return Ok(company);
        }

        // PUT: api/Companies/5
        [HttpPut("UpdateCompany/{id}")]
        public async Task<IActionResult> UpdateCompany(int id, [FromBody] string value)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            string customMessage = "";
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

            }

            return NoContent();
        }

        // POST: api/Companies
        [HttpPut("SaveNewCompanyRecord")]
        [Authorize(Roles = "SLO")]
        public async Task<IActionResult> SaveNewCompanyRecord([FromBody] string value)
        {
            string messageList = "";
            string alertType = "success";
            var companyNewInputs = JsonConvert.DeserializeObject<dynamic>(value);

            try
            {

                if (companyNewInputs.CompanyId == null)
                {
                    //Save new Company
                    Company newCompany = new Company
                    {
                        CompanyName = companyNewInputs.CompanyName.Value,
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
                else
                {

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

        private bool CompanyExists(int id)
        {
            return _context.Companies.Any(e => e.CompanyId == id);
        }
    }
}