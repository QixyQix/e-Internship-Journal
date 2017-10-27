﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace E_Internship_Journal.Models
{
    public class UserBatch
    {
        public int UserBatchId { get; set; }

        public Batch Batch { get; set; }
        public int BatchId { get; set; }
        public ApplicationUser User { get; set; }
        public int UserId { get; set; }
        public Internship_Record InternshipRecord { get; set; }
        public int InternshipRecordId { get; set; }
    }
}
