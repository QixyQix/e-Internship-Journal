using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace E_Internship_Journal.Models
{
    public class Project
    {
        public int ProjectId { get; set; }
        public string Name { get; set; }

        public ApplicationUser Supervisor { get; set; }
        public int SupervisorId { get; set; }
        public Company Company { get; set; }
        public int CompanyID { get; set; }

        public List<Internship_Record> InternshipRecords { get; set; }
    }
}
