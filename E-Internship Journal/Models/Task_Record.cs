using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace E_Internship_Journal.Models
{
    public class Task_Record
    {
        public int TaskRecordId { get; set; }
        public string Description { get; set; }

        public Day_Record DayRecord { get; set; }
        public int DayRecordId { get; set; }
    }
}
