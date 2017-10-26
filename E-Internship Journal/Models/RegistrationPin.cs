using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace E_Internship_Journal.Models
{
    public class RegistrationPin
    {
        public int PinId { get; set; }
        public string Pin { get; set; }

        public ApplicationUser User { get; set; }
        public int UserId { get; set; }
    }
}
