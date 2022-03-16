using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using QRCoder;
using SendGrid;
using SendGrid.Helpers.Mail;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using VMS.ActionFilter;
using VMS.Dtos;
using VMS.Hubs;
using VMS.Models;

namespace VMS.Controllers
{
    public class AppointmentController : Controller
    {
        private IWebHostEnvironment environment;
        private VMSDbContext _context;
        private readonly IHubContext<QrCode> _hub;
        public AppointmentController(IWebHostEnvironment _environment, VMSDbContext context, IHubContext<QrCode> hub)
        {
            environment = _environment;
            _context = context;
            _hub = hub;
        }

        [WhiteListFilter]
        public async Task<IActionResult> IndexAsync(string Id)
        { 
            if (Id == null || Id == "") {
                return View();
            }

            var checkQr = _context.GeneratedTokens.Where(x => x.TokenNumber == Id).FirstOrDefault();
            if (checkQr != null) {
                if (checkQr.IsUsed == true) {
                    return RedirectToAction("UnAuthorized", "Appointment");
                }
                checkQr.IsUsed = true;
                _context.SaveChanges();

                await _hub.Clients.All.SendAsync("qrCodeChecker", Id);
            }

           

            return View();
           
        }

        public JsonResult ListEmployee()
        {


            var employeeList = _context.Employees.ToList();

            return Json(employeeList);
        }




        [HttpPost]
        public async Task<JsonResult> AddAppoitment(AddAppointmentDto appointment)
        {
            
            var objSave = new Appointment()
            {
                TenantId = 1,
                FullName = appointment.FullName,
                PhoneNumber = appointment.PhoneNumber,
                CompanyName = appointment.CompanyName,
                MeetingPurpose = appointment.MeetingPurpose,
                MeetingDescription = appointment.MeetingDescription,
                CarRegistration = appointment.CarRegistration,
                ProfilePhotoUrl = appointment.PhotoName,
                IsPhoto = appointment.PhotoName != null ?  true : false,
                VisitingEmployee = appointment.VisitingEmployee,
                IsFlu = appointment.isFlu,
                CheckIn = DateTime.UtcNow,
                CreatedBy = 1
            };

            if (appointment.MeetingPurpose == 5) {
                objSave.CheckOut = DateTime.UtcNow;
            }

            _context.Appointments.Add(objSave);
            _context.SaveChanges();


            if (appointment.MeetingPurpose == 5)
            {
                objSave.CheckOut = DateTime.UtcNow;
            }

            var meetingPurposeName = _context.MeetingPurposes.Where(x => x.Id == appointment.MeetingPurpose).FirstOrDefault();
            var employerDetail = _context.Employees.Where(x => x.Name == appointment.VisitingEmployee).FirstOrDefault();
            if (employerDetail != null) {

                var apiKey = "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";
                var client = new SendGridClient(apiKey);
                var subject = "";
                if (appointment.MeetingPurpose != 5)
                {
                    subject =
                          $"{appointment.FullName} ({appointment.CompanyName}) is here to see you";
                }
                else
                {
                    subject =
                          $"Your delivery has arrived";

                }


                var from = new EmailAddress("arkitektzsolutions@gmail.com", subject);
                var to = new EmailAddress(employerDetail.Email, employerDetail.Name);
                var plainTextContent = "";
                var htmlContent = "";


                if (appointment.MeetingPurpose != 5)
                {
                    htmlContent =
                          $"<p>Dear {employerDetail.Name},</p>" +
                          $"<p>Your visitor {appointment.FullName} from {appointment.CompanyName} has arrived and wating for you,</p>" +
                          $"<p>The purpose of the meeting is {meetingPurposeName.Name}</p>";
                }
                else {
                    htmlContent =
                           $"<p>Dear {employerDetail.Name},</p>" +
                           $"<p>Your delivery has arrived</p>";  
                }



                var msg = MailHelper.CreateSingleEmail(from, to, subject, plainTextContent, htmlContent);
                var response = await client.SendEmailAsync(msg);
            }




            return Json(objSave);
        }

        [HttpPost]
        public async Task<IActionResult> UploadImage(IFormFile file)
        {
            if (file == null)
                return Ok(null);

            Guid obj = Guid.NewGuid();
            
            string filename = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName.Trim('"');

            filename = this.EnsureCorrectFilename(filename);

            using (FileStream output = System.IO.File.Create(this.GetPathAndFilename(filename+obj.ToString()+".jpg")))
                await file.CopyToAsync(output);


            return Ok(filename + obj.ToString() + ".jpg");
        }

        private string EnsureCorrectFilename(string filename)
        {
            if (filename.Contains("\\"))
                filename = filename.Substring(filename.LastIndexOf("\\") + 1);

            return filename;
        }

        private string GetPathAndFilename(string filename)
        {
            return environment.WebRootPath + "\\uploads\\" + filename;
        }

        [HttpPost]
        public IActionResult BarcodeSignOut(int Barcode)
        {
            var row = _context.Appointments.Where(x => x.Id == Barcode).FirstOrDefault();
            if (row != null) {
                if (row.CheckOut == null) { 
                    row.CheckOut = DateTime.UtcNow;
                    _context.SaveChanges();
                    return Ok("Saved");
                }
            }
            
            return Ok("Error");
        }


        public IActionResult SignInBarCode(int Barcode)
        {

            return Ok();
        }

        public IActionResult UnAuthorized()
        {

            return View();
        }

        public IActionResult ThankYou()
        {

            return View();
        }

        [HttpPost]
        public IActionResult SignInUsingInvite(int Barcode)
        {
            var appointment = _context.Appointments.Where(x => x.Id == Barcode).FirstOrDefault();
            if (appointment != null) {
                if (appointment.CheckIn == null) {
                    appointment.CheckIn = DateTime.UtcNow;
                    _context.SaveChanges();
                     
                    var meetingPurposeName = _context.MeetingPurposes.Where(x => x.Id == appointment.MeetingPurpose).FirstOrDefault();
                    var employerDetail = _context.Employees.Where(x => x.Name == appointment.VisitingEmployee).FirstOrDefault();
                    if (employerDetail != null)
                    {

                        var apiKey = "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";
                        var client = new SendGridClient(apiKey);
                        var subject = $"{appointment.FullName} ({appointment.CompanyName}) is here to see you";
                        var from = new EmailAddress("arkitektzsolutions@gmail.com", subject);
                        var to = new EmailAddress(employerDetail.Email, employerDetail.Name);
                        var plainTextContent = "";
                        var htmlContent = 
                            $"<p>Dear {employerDetail.Name},</p>" +
                            $"<p>Your visitor {appointment.FullName} from {appointment.CompanyName} has arrived and wating for you,</p>" +
                            $"<p>The purpose of the meeting is {meetingPurposeName.Name}</p>";
 

                        var msg = MailHelper.CreateSingleEmail(from, to, subject, plainTextContent, htmlContent);
                        //var response = await client.SendEmailAsync(msg);

                        var task2 = Task.Run(() => client.SendEmailAsync(msg));
                        task2.Wait();
                    }



                    return Ok(appointment.Id);
                }
                return Ok("Error");

            }

            return Ok("Error");
        }

      
        

    }
}
