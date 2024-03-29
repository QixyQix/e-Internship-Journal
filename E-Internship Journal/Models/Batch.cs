﻿using System;
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

        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        public int CourseId { get; set; }
        public Course Course { get; set; }

        public List<UserBatch> UserBatches { get; set; }
    }
}
