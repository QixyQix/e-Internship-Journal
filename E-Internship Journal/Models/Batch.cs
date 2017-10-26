using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace E_Internship_Journal.Models
{
    public class Batch
    {
        public int BatchId { get; set; }
        public string BatchName { get; set; }
        public string Description { get; set; }
        public Course CourseId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }


    }
}
