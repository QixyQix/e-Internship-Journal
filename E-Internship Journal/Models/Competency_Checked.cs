using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace E_Internship_Journal.Models
{
    public class Competency_Checked
    {
        public int Compentency_CheckedId { get; set; }
        public Month_Record MonthId { get; set; }
        public Competencies CompentencyId { get; set; }
    }
}
