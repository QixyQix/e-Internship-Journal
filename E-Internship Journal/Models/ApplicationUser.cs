﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace E_Internship_Journal.Models
{
    // Add profile data for application users by adding properties to the ApplicationUser class
    public class ApplicationUser : IdentityUser
    {
        public List<UserBatch> UserBatches { get; set; }
        public List<Project> Projects { get; set; }
        public List<Internship_Record> InternshipRecords { get; set; }
        public List<RegistrationPin> RegistrationPins { get; set; }
        public DateTime? LastLoginDate { get; set; }

        public Boolean IsEnabled { get; set; }
        public string FullName { get; set; }
        public string StudentId { get; set; }
    }
}
