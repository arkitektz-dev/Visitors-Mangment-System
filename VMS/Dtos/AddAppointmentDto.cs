using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace VMS.Dtos
{
    public class AddAppointmentDto
    {
        [Required]
        public string FullName { get; set; }
        public string PhoneNumber { get; set; }
        [Required]
        public string CompanyName { get; set; }

        [Required]
        public int MeetingPurpose { get; set; }
        public string MeetingDescription { get; set; }
        public string CarRegistration { get; set; }
        public string PhotoName { get; set; }
        [Required]
        public string VisitingEmployee { get; set;}
        public bool isFlu { get; set;}
        public DateTime CheckIn { get; set; }

         

    }
}
