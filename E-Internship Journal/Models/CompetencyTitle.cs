using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace E_Internship_Journal.Models
{
    public class CompetencyTitle
    {
        public int CompetencyTitleId { get; set; }
        public string TitleCompetency { get; set; }
        public int ViewBy { get; set; }
        public List<Competency> Competencies { get; set; }

        public Course Course { get; set; }
        public int CourseId { get; set; }
    }
}
