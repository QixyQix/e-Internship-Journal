using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace E_Internship_Journal.Models
{
    public class Competency
    {
        public int CompetencyId { get; set; }
        public string TitleDescriotion { get; set; }
        public string Description { get; set; }

        public Course Course { get; set; }
        public int CourseId { get; set; }

        public List<Competency_Checked> CompetencyCheckeds { get; set; }

    }
}
