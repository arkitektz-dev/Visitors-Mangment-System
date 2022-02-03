using ClosedXML.Excel;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using VMS.Models;
using VMS.ViewModels;

namespace VMS.Controllers
{
    public class EmployeeController : Controller
    {

        private IWebHostEnvironment environment;
        private VMSDbContext _context;
        
        public EmployeeController(IWebHostEnvironment _environment, VMSDbContext context)
        {
            environment = _environment;
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult LoadEmployeeData()
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
            var employmentData = (from employee in _context.Employees 
                                   select new
                                   { 
                                       employee.Id, 
                                       employee.Name
                                   });

            //Sorting  
            if (!(string.IsNullOrEmpty(sortColumn) && string.IsNullOrEmpty(sortColumnDirection)))
            {
                //  appointmentData = appointmentData.OrderBy(sortColumn + " " + sortColumnDirection);
            }
            //Search  
            if (!string.IsNullOrEmpty(searchValue))
            {
                employmentData = employmentData.Where(m => m.Name == searchValue);
            }

            //total number of rows counts   
            recordsTotal = employmentData.Count();
            //Paging   
            var data = employmentData.Skip(skip).Take(pageSize).ToList();
            //Returning Json Data  
            return Json(new { draw = draw, recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = data });



        }

        [HttpPost]
        public IActionResult CreateEmployee(EmployeeViewModel param)
        {
            var model = new Employee()
            {
                Name = param.Name,
                TenantId = 1,
                CreatedBy = 1
            };

            _context.Employees.Add(model);
            _context.SaveChanges();

            return Json("Done");
        }

        public IActionResult DeleteEmployee(int EmployeeId) {

            var employee = _context.Employees.Where(x => x.Id == EmployeeId).FirstOrDefault();
            var appoitment = _context.Appointments.Where(x => x.VisitingEmployee == employee.Name).FirstOrDefault();

            if (appoitment != null) {
                return Json("Fail");
            }

            if (employee != null) { 
                _context.Remove(employee);
                _context.SaveChanges();

                return Json("Done");
            }

            return Json("Fail");
                 
        }


        public IActionResult EditEmployee(int EmployeeId,EmployeeViewModel param)
        {

            var model = _context.Employees.Where(x => x.Id == EmployeeId).FirstOrDefault();
            if (model != null) {

                model.Name = param.Name;
                model.TenantId = param.TenantId;

                _context.SaveChanges();

            }
            
            return Json("Done");
        }

    
        public IActionResult ImportEmployee()
        { 
            return View();
        }


        [HttpPost]
        public IActionResult ImportEmployee(IFormFile file) 
        {
            List<string> employeeList = new List<string>();
            string path = Path.Combine(environment.WebRootPath, Path.GetFileName(file.FileName));
            using (Stream fileStream = new FileStream(path, FileMode.Create))
            {
                 file.CopyTo(fileStream);
            }
            using (XLWorkbook workbook = new XLWorkbook(path))
            {
                IXLWorksheet worksheet = workbook.Worksheet(1);
                bool FirstRow = true;
                //Range for reading the cells based on the last cell used.  
                string readRange = "1:1";
                foreach (IXLRow row in worksheet.RowsUsed())
                {
                    foreach (IXLCell cell in row.Cells(readRange))
                    {
                        //Debug.WriteLine(cell.Value.ToString());
                        employeeList.Add(cell.Value.ToString());

                    }

                }
                 
            }
             
            return View();
        }


    }
}
