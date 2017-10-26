using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace E_Internship_Journal.Models
{
    public class UserBatch
    {
        public int UserBatchId { get; set; }
        public Batch BatchId { get; set; }
        public ApplicationUser UserId { get; set; }
    }
}
