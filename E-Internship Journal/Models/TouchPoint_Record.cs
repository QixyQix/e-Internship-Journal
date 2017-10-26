using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace E_Internship_Journal.Models
{
    public class TouchPoint_Record
    {
        public int TouchPointId { get; set; }
        public Internship_Record InternshipId { get; set; }
        public string Comments { get; set; }
    }
}
