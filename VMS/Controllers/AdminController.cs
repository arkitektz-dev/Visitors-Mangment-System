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

namespace VMS.Controllers
{
    //[Authorize]
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

        public IActionResult LoadData()
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
                                           appointment.CarRegistration
                                       });
                                       
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

        public IActionResult PrintPage(int Id) {
            AppointmentId = Id;
            System.Drawing.Printing.PrintDocument pd = new System.Drawing.Printing.PrintDocument();
           
            pd.PrintPage += new PrintPageEventHandler(pd_PrintPage);
            pd.Print();

            return Ok();
        }


        public IActionResult TestPrint(int AppointmentId)
        {
            var row = _context.Appointments.Where(x => x.Id == AppointmentId).FirstOrDefault();

            return View(row);
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
    }
}
