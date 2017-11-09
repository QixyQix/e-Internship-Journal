using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using E_Internship_Journal.Data;
using E_Internship_Journal.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Authorization;
using System.Text;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace E_Internship_Journal.API
{
    [Route("api/[controller]")]
    public class RegistrationPinsController : Controller
    {

        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public RegistrationPinsController(UserManager<ApplicationUser> userManager, ApplicationDbContext context)
        {
            _userManager = userManager;
            _context = context;
        }
        //// GET: api/values
        //[HttpGet]
        //public IEnumerable<string> Get()
        //{
        //    return new string[] { "value1", "value2" };
        //}

        // GET api/values/5
        [HttpGet("{id}")]
        public IActionResult Get(string id)
        {
            if (_context.RegistrationPins.Any(rp => rp.RegistrationPinId.Equals(id) && rp.Valid == true))
            {
                object jsonResult = new
                {
                    validity = true
                };
                return new JsonResult(jsonResult);
            }
            return NotFound();
        }

        // POST api/values
        [HttpPost]
        public async Task<IActionResult> Post([FromBody]string value)
        {
            var newPinInput = JsonConvert.DeserializeObject<dynamic>(value);

            string email = newPinInput.Email.Value.ToString();

            ApplicationUser user = _context.Users.Where(usr => usr.Email.Equals(email, StringComparison.OrdinalIgnoreCase))
                .Include(usr => usr.RegistrationPins)
                .Single();

            if (user != null)
            {
                foreach (var pin in user.RegistrationPins)
                {
                    pin.Valid = false;
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
                            User = user,
                            RegistrationPinId = generateRandomString(50)
                        };
                        _context.RegistrationPins.Add(newRegistrationPin);
                        repeatPinGeneration = false;
                    }
                } while (repeatPinGeneration);

                await _context.SaveChangesAsync();
            }

            return NotFound();
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(string id, [FromBody]string value)
        {
            RegistrationPin registrationPin = null;
            registrationPin = _context.RegistrationPins
                .Where(rp => rp.RegistrationPinId.Equals(id))
                .Include(rp => rp.User)
                .Single();

            if (registrationPin != null)
            {
                var newPasswordInput = JsonConvert.DeserializeObject<dynamic>(value);

                ApplicationUser user = registrationPin.User;

                if (user.Email.Equals(newPasswordInput.Email.Value, StringComparison.OrdinalIgnoreCase))
                {
                    PasswordHasher<ApplicationUser> ph = new PasswordHasher<ApplicationUser>();
                    user.PasswordHash = ph.HashPassword(user, newPasswordInput.NewPassword.Value);
                    user.IsEnabled = true;
                    registrationPin.Valid = false;
                }
                await _context.SaveChangesAsync();
                return Ok();
            }
            return NotFound();
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

        //// DELETE api/values/5
        //[HttpDelete("{id}")]
        //public void Delete(int id)
        //{
        //}
    }
}
