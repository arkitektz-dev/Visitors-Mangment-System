using System;
using System.Collections.Generic;

#nullable disable

namespace VMS.Models
{
    public partial class Tenant
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public int? UpdateBy { get; set; }
    }
}
