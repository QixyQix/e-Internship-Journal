using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace E_Internship_Journal.Models
{
    public class Competency_Checked
    {
        public int CompentencyCheckedId { get; set; }
        
        public int MonthRecordId { get; set; }
        public Month_Record MonthRecord { get; set; }
        public int CompetencyId { get; set; }
        public Competency Competency { get; set; }
    }
}
