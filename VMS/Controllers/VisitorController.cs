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
              x => x.FullName == appointment.FullName &&
              x.VisitingEmployee == appointment.VisitingEmployee &&
              x.MeetingPurpose == appointment.MeetingPurpose &&
              x.CheckIn.Value.Date == appointment.CheckIn.Date).FirstOrDefault();

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
                    CheckIn = appointment.CheckIn.Date,
                    CreatedBy = 1
                };

                _context.Appointments.Add(objSave);
                _context.SaveChanges();
            }

       

        

            return Ok(appointment);
        }

    }
}
