using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VMS.ViewModels
{
    public class PrintViewModel
    {
        public string ImageUrl { get; set; }
        public string FullName { get; set; }
        public string Company { get; set; }
        public DateTime AppointmentDate { get; set; }
        public string EmployerName { get; set; }
        public string PassNo { get; set; }
    }
}
