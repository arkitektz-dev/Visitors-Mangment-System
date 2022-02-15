﻿using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
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
        public JsonResult AddAppoitment(AddAppointmentDto appointment)
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
                CheckIn = DateTime.Now,
                CreatedBy = 1
            };

            _context.Appointments.Add(objSave);
            _context.SaveChanges();



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
                    row.CheckOut = DateTime.Now.Date;
                    _context.SaveChanges();           
                }
            }
            
            return Ok("Error");
        }

        public IActionResult UnAuthorized()
        {

            return View();
        }
    }
}
