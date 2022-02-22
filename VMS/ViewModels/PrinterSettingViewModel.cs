using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace VMS.ViewModels
{
    public class PrinterSettingViewModel
    {
        [Required]
        public string PrinterName { get; set; }
    }
}
