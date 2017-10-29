using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace E_Internship_Journal.Models
{
    // Add profile data for application users by adding properties to the ApplicationUser class
    public class ApplicationUser : IdentityUser
    {
      /*  public List<UserBatch> UserBatches { get; set; }
        public List<Project> Projects { get; set; }
        public List<RegistrationPin> RegistrationPins { get; set; }*/
        public Course Course { get; set; }
        public int? CourseId { get; set; }
    }
}
