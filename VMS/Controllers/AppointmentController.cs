﻿using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using VMS.Dtos;
using VMS.Models;

namespace VMS.Controllers
{
    public class AppointmentController : Controller
    {
        private IWebHostEnvironment environment;
        private VMSDbContext _context;
        public AppointmentController(IWebHostEnvironment _environment, VMSDbContext context)
        {
            environment = _environment;
            _context = context;
        }


        public IActionResult Index()
        {
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
                CheckIn = DateTime.Now.Date,
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
    }
}
