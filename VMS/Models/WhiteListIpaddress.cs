using System;
using System.Collections.Generic;

#nullable disable

namespace VMS.Models
{
    public partial class WhiteListIpaddress
    {
        public int Id { get; set; }
        public string Ipaddress { get; set; }
        public string Description { get; set; }
        public DateTime? CreatedDate { get; set; }
    }
}
