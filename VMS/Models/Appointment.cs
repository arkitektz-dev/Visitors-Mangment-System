using System;
using System.Collections.Generic;

#nullable disable

namespace VMS.Models
{
    public partial class Appointment
    {
        public int Id { get; set; }
        public int? TenantId { get; set; }
        public string FullName { get; set; }
        public string PhoneNumber { get; set; }
        public string CompanyName { get; set; }
        public int? MeetingPurpose { get; set; }
        public string MeetingDescription { get; set; }
        public string VisitingEmployee { get; set; }
        public string CarRegistration { get; set; }
        public string ProfilePhotoUrl { get; set; }
        public string GlobalAppointmentId { get; set; }
        public bool? IsPhoto { get; set; }
        public bool? IsFlu { get; set; }
        public DateTime? CheckIn { get; set; }
        public DateTime? CheckOut { get; set; }
        public bool? IsPrint { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public int? UpdatedBy { get; set; }
    }
}
