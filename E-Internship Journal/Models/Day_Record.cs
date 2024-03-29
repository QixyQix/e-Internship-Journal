﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace E_Internship_Journal.Models
{
    public class Day_Record
    {
        public int DayId { get; set; }
        public DateTime Date { get; set; }
        public DateTime? ArrivalTime { get; set; }
        public DateTime? DepartureTime { get; set; }
        public int WeekNo { get; set; }
        public Boolean IsPresent { get; set; }
        public string Remarks { get; set; }
        public string SupervisorRemarks { get; set; }

        public Month_Record Month { get; set; }
        public int MonthRecordId { get; set; }
    }
}
