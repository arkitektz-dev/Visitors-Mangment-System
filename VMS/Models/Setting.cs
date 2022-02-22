using System;
using System.Collections.Generic;

#nullable disable

namespace VMS.Models
{
    public partial class Setting
    {
        public int Id { get; set; }
        public string SettingKey { get; set; }
        public string SettingValue { get; set; }
    }
}
