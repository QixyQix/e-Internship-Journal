using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace E_Internship_Journal.Models
{
    public class Internship_Record
    {
        public int Id { get; set; }
        public int PresentationGrading { get; set; }
        public int ReflectionGrading { get; set; }
        public string Comment { get; set; }
        public string PosterUrl { get; set; }

        public ApplicationUser LiaisonOfficer { get; set; }
        public int LiaisonOfficeId { get; set; }
        public UserBatch UserBatch { get; set; }
        public Project Project { get; set; }
        public int ProjectId { get; set; }

        public List<TouchPoint> TouchPoints { get; set; }
        public List<Month_Record> MonthRecords { get; set; }


    }
}
