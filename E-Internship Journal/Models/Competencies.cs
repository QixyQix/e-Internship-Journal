using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace E_Internship_Journal.Models
{
    public class Competencies
    {
        public int CompetenciesId { get; set; }
        public string TitleDescriotion { get; set; }
        public string Description { get; set; }
        public Course CourseId { get; set; }

    }
}
