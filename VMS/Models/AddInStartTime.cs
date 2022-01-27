using System;
using System.Collections.Generic;

#nullable disable

namespace VMS.Models
{
    public partial class AddInStartTime
    {
        public int Id { get; set; }
        public int? TenantId { get; set; }
        public DateTime? StartTime { get; set; }
    }
}
