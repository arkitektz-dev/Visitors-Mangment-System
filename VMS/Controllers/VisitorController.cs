using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VMS.Dtos;
using VMS.Models;

namespace VMS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VisitorController : ControllerBase
    {
         
        private VMSDbContext _context;
        public VisitorController(VMSDbContext context)
        {
            _context = context;
        }

     
        [HttpPost("add-appointment")]
        public IActionResult AddAppointment(AddAppointmentDto appointment)
        {

            var row = _context.Appointments.Where(
              x => x.GlobalAppointmentId == appointment.GlobalAppointmentId &&
              x.FullName == appointment.FullName).FirstOrDefault();

            if (row == null) {
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
                    IsPhoto = appointment.PhotoName != null ? true : false,
                    VisitingEmployee = appointment.VisitingEmployee,
                    IsFlu = appointment.isFlu,
                    GlobalAppointmentId = appointment.GlobalAppointmentId,
                    CreatedBy = 1,
                    CreatedDate = DateTime.Now
                };

                _context.Appointments.Add(objSave);
                _context.SaveChanges();

                return Ok(objSave);
            }

       

        

            return Ok(appointment);
        }

        [HttpGet("outlook-tenant-intitaltime")]
        public IActionResult AddStartingTime(int tenantId)
        {
            var row = _context.AddInStartTimes.Where(x => x.TenantId == tenantId).FirstOrDefault();
            if (row != null) {
                return Ok(row.StartTime);
            }

            var insertRow = new AddInStartTime()
            {
                StartTime = DateTime.Now.Date.AddDays(-4),
                TenantId = tenantId
            };

            _context.AddInStartTimes.Add(insertRow);
            _context.SaveChanges();

            return Ok(insertRow.StartTime);
        }
    }
}
