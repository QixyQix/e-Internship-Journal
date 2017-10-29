using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace E_Internship_Journal.Models
{
    public class RegistrationPin
    {
        public int RegistrationPinId { get; set; }
        public string Pin { get; set; }

        public ApplicationUser User { get; set; }
        public string UserId { get; set; }
    }
}
