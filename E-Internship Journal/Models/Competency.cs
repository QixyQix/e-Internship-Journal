using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace E_Internship_Journal.Models
{
    public class Competency
    {
        public int CompetencyId { get; set; }
        public string Description { get; set; }
        public int ViewBy { get; set; }
        public DateTime? DeletedAt { get; set; }
        public string ModifiedBy { get; set; }
        public DateTime? ModifiedAt { get; set; }
        public string CreatedBy { get; set; }

        //public Course Course { get; set; }
        //public int CourseId { get; set; }

        public List<Competency_Checked> CompetencyCheckeds { get; set; }

        public int CompetencyTitleId { get; set; }
        public CompetencyTitle CompetencyTitle { get; set; }

    }
}
