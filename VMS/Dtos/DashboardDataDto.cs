using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VMS.Dtos
{
    public class DashboardDataDto
    {
        public List<int> CheckIn { get; set; }
        public List<int> CheckOut { get; set;}

        public List<int> LastDaysCheckIn { get; set; }

        public List<int> LastTenDaysLabel { get; set; }

        public int TotalVisitors { get; set; }
        public int TotalEmployees { get; set; }
        public int TotalAppointment { get; set; }
        public int TodayVisitors { get; set; }

        public int VisitorOnsite { get; set; }

    }
}
