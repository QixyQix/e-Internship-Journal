using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace E_Internship_Journal.Models
{
    public class Month_Record
    {
        public int MonthId { get; set; }
        public Boolean? Approved { get; set; }
        public string SoftSkillsCompetencyDoneWell { get; set; }
        public string SoftSkillsCompetencyImprove { get; set; }
        public string TechnicalCompetencyApplied { get; set; }
        public string TechnicalCompetencyDoneWell { get; set; }
        public string TechnicalCompetencyImprove { get; set; }

        public DateTime? MentorSessionDateTimeStart { get; set; }
        public DateTime? MentorSessionDateTimeEnd { get; set; }
        public string MentorSessionReflection { get; set; }

        public int? CommunicationGrading { get; set; }
        public int? TechnicalGrading { get; set; }
        public int? IndependenceGrading { get; set; }
        public int? PerformanceGrading { get; set; }
        public int? OverallGrading { get; set; }
        public string OverallFeedback { get; set; }
        
        public Internship_Record InternshipRecord { get; set; }
        public int InternshipRecordId { get; set; }

        public List<Day_Record> DayRecords { get; set; }
        public List<Competency_Checked> CompetencyCheckeds { get; set; }
        public List<Task_Record> TaskRecords { get; set; }

    }
}
