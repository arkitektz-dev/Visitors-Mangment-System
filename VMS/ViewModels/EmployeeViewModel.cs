using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VMS.ViewModels
{
    public class EmployeeViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int? TenantId { get; set; } 
        public string Email { get; set; }
        public string Phone { get; set; }
    }
}
