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
        public DateTime Date { get; set; }
        public int WeekNo { get; set; }

        public Month_Record MonthRecord { get; set; }
        public int MonthRecordId { get; set; }
    }
}
