using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VMS.Dtos
{
    public class AddAppointmentDto
    {
        public string FullName { get; set; }
        public string PhoneNumber { get; set; }
        public string CompanyName { get; set; }
        public int? MeetingPurpose { get; set; }
        public string MeetingDescription { get; set; }
        public string CarRegistration { get; set; }
        public string PhotoName { get; set; }

         

    }
}
