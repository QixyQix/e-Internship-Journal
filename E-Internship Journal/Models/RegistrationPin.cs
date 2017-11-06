using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace E_Internship_Journal.Models
{
    public class RegistrationPin
    {
        public string RegistrationPinId { get; set; }
        public Boolean Valid { get; set; }

        public ApplicationUser User { get; set; }
        public string UserId { get; set; }
    }
}
