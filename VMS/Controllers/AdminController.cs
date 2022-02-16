using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VMS.Models;
using System.Linq.Dynamic;
using Microsoft.AspNetCore.Authorization;
using System.Drawing;
using System.Drawing.Printing;
using OpenQA.Selenium;
using OpenQA.Selenium.IE;
using System.IO;
using System.Drawing.Imaging;
using System.Net;
using CoreHtmlToImage;
using VMS.Dtos;
using VMS.Extensions;

namespace VMS.Controllers
{
   [Authorize]
    public class AdminController : Controller
    {
        int AppointmentId;
        private IWebHostEnvironment environment;
        private VMSDbContext _context;

        public AdminController(IWebHostEnvironment _environment, VMSDbContext context)
        {
            environment = _environment;
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }
        public IActionResult LoadData(string filterType)
        {
             
                var draw = HttpContext.Request.Form["draw"].FirstOrDefault();

                // Skip number of Rows count  
                var start = Request.Form["start"].FirstOrDefault();

                // Paging Length 10,20  
                var length = Request.Form["length"].FirstOrDefault();

                // Sort Column Name  
                var sortColumn = Request.Form["columns[" + Request.Form["order[0][column]"].FirstOrDefault() + "][name]"].FirstOrDefault();

                // Sort Column Direction (asc, desc)  
                var sortColumnDirection = Request.Form["order[0][dir]"].FirstOrDefault();

                // Search Value from (Search box)  
                var searchValue = Request.Form["search[value]"].FirstOrDefault();

                //Paging Size (10, 20, 50,100)  
                int pageSize = length != null ? Convert.ToInt32(length) : 0;

                int skip = start != null ? Convert.ToInt32(start) : 0;

                int recordsTotal = 0;

                // getting all Customer data  
                var appointmentData = (from appointment in _context.Appointments
                                       join meeting in _context.MeetingPurposes on appointment.MeetingPurpose equals meeting.Id
                                       select new
                                       {
                                           appointment.Id,
                                           appointment.FullName,
                                           appointment.PhoneNumber,
                                           appointment.CompanyName,
                                           meeting.Name,
                                           appointment.CarRegistration,
                                           appointment.CheckIn,
                                           appointment.CheckOut
                                       });

                 if (filterType != null) {
                    
                    if (filterType == "CheckIn") {
                         appointmentData = appointmentData.OrderByDescending(x => x.CheckIn).Where(x => x.CheckIn != null && x.CheckOut == null);
                    }

                    

                 }

            //Sorting  
            if (!(string.IsNullOrEmpty(sortColumn) && string.IsNullOrEmpty(sortColumnDirection)))
                {
                  //  appointmentData = appointmentData.OrderBy(sortColumn + " " + sortColumnDirection);
                }
                //Search  
                if (!string.IsNullOrEmpty(searchValue))
                {
                    appointmentData = appointmentData.Where(m => m.FullName == searchValue);
                }

                //total number of rows counts   
                recordsTotal = appointmentData.Count();
                //Paging   
                var data = appointmentData.Skip(skip).Take(pageSize).ToList();
                //Returning Json Data  
                return Json(new { draw = draw, recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = data });

           

        }

        public IActionResult LoadGraphData()
        {

            var draw = HttpContext.Request.Form["draw"].FirstOrDefault();

            // Skip number of Rows count  
            var start = Request.Form["start"].FirstOrDefault();

            // Paging Length 10,20  
            var length = Request.Form["length"].FirstOrDefault();

            // Sort Column Name  
            var sortColumn = Request.Form["columns[" + Request.Form["order[0][column]"].FirstOrDefault() + "][name]"].FirstOrDefault();

            // Sort Column Direction (asc, desc)  
            var sortColumnDirection = Request.Form["order[0][dir]"].FirstOrDefault();

            // Search Value from (Search box)  
            var searchValue = Request.Form["search[value]"].FirstOrDefault();

            //Paging Size (10, 20, 50,100)  
            int pageSize = length != null ? Convert.ToInt32(length) : 0;

            int skip = start != null ? Convert.ToInt32(start) : 0;

            int recordsTotal = 0;



            // getting all Customer data  
            var appointmentData = (from employee in _context.Employees
                                  select new
                                  {
                                      employee.Id,
                                      employee.Name,
                                      employee.CreatedDate
                                  });

            //Sorting  
            if (!(string.IsNullOrEmpty(sortColumn) && string.IsNullOrEmpty(sortColumnDirection)))
            {
                //  appointmentData = appointmentData.OrderBy(sortColumn + " " + sortColumnDirection);
            }
            //Search  
            if (!string.IsNullOrEmpty(searchValue))
            {
                appointmentData = appointmentData.Where(m => m.Name == searchValue);
            }

            //total number of rows counts   
            recordsTotal = appointmentData.Count();
            //Paging   
            var data = appointmentData.Skip(skip).Take(pageSize).ToList();
            //Returning Json Data  
            return Json(new { draw = draw, recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = data });



        }
        public IActionResult GetAppointment(int Id)
        {
            var model = (from appointment in _context.Appointments.Where(x => x.Id == Id)
                         join meeting in _context.MeetingPurposes on appointment.MeetingPurpose equals meeting.Id
                         select new
                         {
                             appointment.Id,
                             appointment.FullName,
                             appointment.PhoneNumber,
                             appointment.CompanyName,
                             meeting.Name,
                             appointment.MeetingDescription,
                             appointment.CarRegistration,
                             appointment.ProfilePhotoUrl

                         }).FirstOrDefault();

            return Json(model);
        }

        [AllowAnonymous]
        public IActionResult PrintPage(int Id) {
            AppointmentId = Id;
            System.Drawing.Printing.PrintDocument pd = new System.Drawing.Printing.PrintDocument();
           
            pd.PrintPage += new PrintPageEventHandler(pd_PrintPage);
            pd.Print();

            return Ok();
        }
        public IActionResult Dashboard() 
        {

            return View();
        }
        [AllowAnonymous]
        public IActionResult TestPrint(int AppointmentId)
        {
            var row = _context.Appointments.Where(x => x.Id == AppointmentId).FirstOrDefault();

            return View(row);
        }
        public IActionResult MontlyVisitors()
        {
            DashboardDataDto model = new DashboardDataDto();
            List<int> CheckIn = new List<int>();
            List<int> CheckOut = new List<int>();
            List<int> LastTenDays = new List<int>();
            

            //checkIn
            for (int i = 1; i <= 12; i++) {
              
                var totalCheckInForMonth = _context.Appointments.Where(x => x.CheckIn.Value.Date.Month == i).ToList().Count;
                CheckIn.Add(totalCheckInForMonth); 

            }

            for (int i = 1; i <= 12; i++)
            {
                var totalCheckOutForMonth = _context.Appointments.Where(x => x.CheckOut.Value.Date.Month == i).ToList().Count;
                CheckOut.Add(totalCheckOutForMonth);
            }

            for (int i = 1; i <= 10; i++) {
                 
                var totalCheckOutForDay = _context.Appointments.Where(x => x.CheckIn.Value.Date.Day == i 
                && x.CheckIn.Value.Date.Year == DateTime.Now.Year).ToList().Count;
                LastTenDays.Add(totalCheckOutForDay);
            }

            model.CheckIn = CheckIn;
            model.CheckOut = CheckOut;
            model.LastDaysCheckIn = LastTenDays;
            model.TotalAppointment = _context.Appointments.Where(x => x.CheckIn != null && x.CheckOut != null).ToList().Count();
            model.TotalEmployees = _context.Employees.ToList().Count();
            model.TotalVisitors = _context.Appointments.ToList().Count();




            return Ok(model);
        }
        public async Task<IActionResult> TestPrintForImage(int AppointmentId)
        {
            var converter = new HtmlConverter();
            var bytes = converter.FromUrl($"https://localhost:44308/Admin/TestPrint?AppointmentId={AppointmentId}");
            Stream stream = new MemoryStream(bytes);

            return File(stream, "image/jpeg");


        }

        void pd_PrintPage(object sender, PrintPageEventArgs e)
        {

            using (WebClient webClient = new WebClient())
            {
                byte[] data = webClient.DownloadData($"https://localhost:44308/Admin/TestPrintForImage?AppointmentId={AppointmentId}");

                using (MemoryStream mem = new MemoryStream(data))
                {
                    var img = Image.FromStream(mem) as Bitmap;

                    var g = e.Graphics;
                    var stringformat = new StringFormat();
                    stringformat.Alignment = StringAlignment.Far;
                    var solidBrush = new SolidBrush(Color.Black);
                    var fontFamily = new FontFamily("Times New Roman");
                    var font = new Font(fontFamily, 15, FontStyle.Regular, GraphicsUnit.Pixel);
                    string currentDate = $"{DateTime.Now.Date.ToString("dd/MM/yyyy")} {DateTime.Now.ToString("hh:mm tt")}";
                    //var tkcLogo = Image.FromFile();
                    //g.DrawImage(tkcLogo, new Point(40, 35)); 
                    RectangleF rect2 = new RectangleF(100.0F, 25.0F, 182, 25.0F);
                    Point startPoint = new Point();
                    g.DrawImage(img, startPoint);

                }

            } 
            
            


        }

        public IActionResult ForceCheckOut(int AppointmentId)
        {
            var row = _context.Appointments.Where(x => x.Id == AppointmentId).FirstOrDefault();
            if (row != null) {
                row.CheckOut = DateTime.Now;
                _context.SaveChanges();

                return Ok(row.CheckOut);
            }

            return Ok();
        }
    }
}
