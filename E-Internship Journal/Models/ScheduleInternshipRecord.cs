using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace E_Internship_Journal.Models
{
    public class ScheduleInternshipRecord
    {
        public int ScheduleIntershipRecordId { get; set; }
        public string LiaisonOfficerId { get; set; }
        public int UserBatchId { get; set; }
        public int ProjectId { get; set; }
    }
}
