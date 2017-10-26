using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace E_Internship_Journal.Models
{
    public class Company
    {
        public int CompanyId { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
    
        public List<Project> Projects { get; set; }

    }
}
