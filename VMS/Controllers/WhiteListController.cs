using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VMS.Hubs;
using VMS.Models;

namespace VMS.Controllers
{
    public class WhiteListController : Controller
    {
        private IWebHostEnvironment environment;
        private VMSDbContext _context;
        private readonly IHubContext<QrCode> _hub;
        public WhiteListController(IWebHostEnvironment _environment, VMSDbContext context, IHubContext<QrCode> hub)
        {
            environment = _environment;
            _context = context;
            _hub = hub;
        }

        public async Task<IActionResult> Index()
        {
            
            return View();
        }

        [HttpPost]
        public IActionResult LoadIPAddressData()
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
            var employmentData = (from row in _context.WhiteListIpaddresses
                                  select new
                                  {
                                      row.Id,
                                      row.Ipaddress, 
                                      row.Description
                                  });

            //Sorting  
            if (!(string.IsNullOrEmpty(sortColumn) && string.IsNullOrEmpty(sortColumnDirection)))
            {
                //  appointmentData = appointmentData.OrderBy(sortColumn + " " + sortColumnDirection);
            }
            //Search  
            if (!string.IsNullOrEmpty(searchValue))
            {
                employmentData = employmentData.Where(m => m.Ipaddress == searchValue);
            }

            //total number of rows counts   
            recordsTotal = employmentData.Count();
            //Paging   
            var data = employmentData.Skip(skip).Take(pageSize).ToList();
            //Returning Json Data  
            return Json(new { draw = draw, recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = data });



        }

        [HttpPost]
        public IActionResult AddIPAddress(string IPAddress, string Description)
        {
            var checkIpAddress = _context.WhiteListIpaddresses.Where(x => x.Ipaddress == IPAddress).FirstOrDefault();
            if (checkIpAddress != null) {
                return Ok();
            }

            var model = new WhiteListIpaddress()
            {
                Ipaddress = IPAddress,
                Description = Description,
                CreatedDate = DateTime.Now.Date
            };

            _context.WhiteListIpaddresses.Add(model);
            _context.SaveChanges();

            return Ok();
        }


        [HttpPost]
        public IActionResult DeleteIPAddress(int Id) 
        {
            var IPAddress = _context.WhiteListIpaddresses.Where(x => x.Id == Id).FirstOrDefault();
            if (IPAddress != null) {
                _context.WhiteListIpaddresses.Remove(IPAddress);
                _context.SaveChanges();
               
                return Ok();
            }

            return Ok();
            
        }

    }
}
