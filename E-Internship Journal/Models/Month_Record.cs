using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace E_Internship_Journal.Models
{
    public class Month_Record
    {
        public int MonthId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public Boolean Approved { get; set; }
        public string Soft_Skills_Competency { get; set; }
        public string Technical_Competency { get; set; }
        public Internship_Record InternshipId { get; set; }
    }
}
