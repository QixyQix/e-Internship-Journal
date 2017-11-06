using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace E_Internship_Journal.Models
{
    public class Internship_Record
    {
        public int InternshipRecordId { get; set; }
        public int? PresentationGrading { get; set; }
        public int? ReflectionGrading { get; set; }
        public string Comment { get; set; }
        public string PosterUrl { get; set; }

        public Boolean CompanyCheck1a { get; set; }
        public Boolean CompanyCheck1b { get; set; }
        public Boolean CompanyCheck2a { get; set; }
        public Boolean CompanyCheck2b { get; set; }
        public Boolean CompanyCheck2c { get; set; }
        public Boolean CompanyCheck2d { get; set; }
        public Boolean CompanyCheck2e { get; set; }
        public Boolean CompanyCheck2f { get; set; }
        public Boolean CompanyCheck3a { get; set; }
        public Boolean CompanyCheck3b { get; set; }
        public Boolean CompanyCheck3c { get; set; }

        public Boolean FeedbackUseful { get; set; }
        public Boolean FeedbackImproved { get; set; }
        public Boolean FeedbackExperiences { get; set; }
        public Boolean FeedbackRecommend { get; set; }
        public string FeedbackEnjoy { get; set; }
        public string FeedbackLeastEnjoy { get; set; }

        public ApplicationUser LiaisonOfficer { get; set; }
       public string LiaisonOfficerId { get; set; }

        public UserBatch UserBatch { get; set; }
        public int UserBatchId { get; set; }

        public Project Project { get; set; }
        public int ProjectId { get; set; }

        public List<TouchPoint_Record> TouchPoints { get; set; }
        public List<Month_Record> MonthRecords { get; set; }


    }
}
