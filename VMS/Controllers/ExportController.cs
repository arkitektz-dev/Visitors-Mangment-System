using ClosedXML.Excel;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using VMS.Models;

namespace VMS.Controllers
{
    public class ExportController : Controller
    {
        private VMSDbContext _context;
        public ExportController(VMSDbContext context)
        {
            _context = context;
        }


        public IActionResult ExportAppointmentExcel(string filterType, DateTime? startDate, DateTime? endDate, int meetingId)
        {
            DataTable dt = getAppointmentData(filterType, startDate, endDate, meetingId);
            //Name of File  
            string fileName = $"Appointment{DateTime.Now.Date}.xlsx";
            using (XLWorkbook wb = new XLWorkbook())
            {
                //Add DataTable in worksheet  
                wb.Worksheets.Add(dt);
                using (MemoryStream stream = new MemoryStream())
                {
                    wb.SaveAs(stream);
                    //Return xlsx Excel File  
                    return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
                }
            }
        }

        public DataTable getAppointmentData(string filterType, DateTime? startDate, DateTime? endDate, int meetingId)
        {
            //Creating DataTable  
            DataTable dt = new DataTable();
            //Setiing Table Name  
            dt.TableName = "Warehouse";
            //Add Columns  
            dt.Columns.Add("#", typeof(int));
            dt.Columns.Add("FullName", typeof(string));
            dt.Columns.Add("PhoneNumber", typeof(string));
            dt.Columns.Add("CompanyName", typeof(string));
            dt.Columns.Add("MeetingPurpose", typeof(string));
            dt.Columns.Add("MeetingDescription", typeof(string));
            dt.Columns.Add("VisitingEmployee", typeof(string));
            dt.Columns.Add("CarRegistration", typeof(string));
            dt.Columns.Add("Has Flu Symptom", typeof(string));
            dt.Columns.Add("CheckIn", typeof(string));
            dt.Columns.Add("CheckOut", typeof(string));

            //Add Rows in DataTable  




            var list = (from appointment in _context.Appointments
                        orderby appointment.CreatedDate descending
                        join meetingType in _context.MeetingPurposes on appointment.MeetingPurpose equals meetingType.Id
                        join meeting in _context.MeetingPurposes on appointment.MeetingPurpose equals meeting.Id
                        select new
                        {
                            appointment.FullName,
                            appointment.PhoneNumber,
                            appointment.CompanyName,
                            meetingType.Name,
                            appointment.MeetingDescription,
                            appointment.VisitingEmployee,
                            appointment.CarRegistration,
                            MeetingType = meetingType.Name,
                            appointment.IsFlu,
                            appointment.CheckIn,
                            appointment.CheckOut,
                            appointment.CreatedDate,
                            meetingType = meetingType.Id
                        }).ToList();

            if (filterType != null)
            {

                if (filterType == "CheckIn")
                {
                    list = list.OrderByDescending(x => x.CheckIn).Where(x => x.CheckIn != null && x.CheckOut == null).ToList();
                }

            }

            if (startDate != null && endDate != null)
            {
                list = list.OrderByDescending(x => x.CheckIn).Where(x => x.CreatedDate > startDate && x.CreatedDate < endDate).ToList();
            }
            else
            {

                if (startDate != null)
                {
                    list = list.OrderByDescending(x => x.CheckIn).Where(x => x.CreatedDate < startDate).ToList();
                }

                if (endDate != null)
                {
                    list = list.OrderByDescending(x => x.CheckIn).Where(x => x.CreatedDate < endDate).ToList();
                }

            }

            if (meetingId != 0)
            {
                list = list.OrderByDescending(x => x.CheckIn).Where(x => x.meetingType == meetingId).ToList();
            }

            int counter = 0;
            foreach (var item in list)
            {
                counter++;
                dt.Rows.Add(counter,
                    item.FullName, item.PhoneNumber, item.CompanyName,
                    item.MeetingType, item.MeetingDescription, item.VisitingEmployee, item.CarRegistration,
                    item.IsFlu, item.CheckIn, item.CheckOut);
            }
            dt.AcceptChanges();
            return dt;
        }

    }
}
