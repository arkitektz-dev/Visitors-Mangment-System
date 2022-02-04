using ClosedXML.Excel;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
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


        public IActionResult EditEmployee(int EmployeeId, EmployeeViewModel param)
        {

            var model = _context.Employees.Where(x => x.Id == EmployeeId).FirstOrDefault();
            if (model != null) {

                model.Name = param.Name;
                model.TenantId = param.TenantId;

                _context.SaveChanges();

            }

            return Json("Done");
        }

        public IActionResult SheetList()
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
            var sheetsData = (from sheet in _context.ExcelSheetImports
                                  select new
                                  {
                                      sheet.Id,
                                      sheet.SheetName,
                                      sheet.RowsCount,
                                      sheet.RowsImported,
                                      sheet.RowsFailed
                                  });

            //Sorting  
            if (!(string.IsNullOrEmpty(sortColumn) && string.IsNullOrEmpty(sortColumnDirection)))
            {
                //  appointmentData = appointmentData.OrderBy(sortColumn + " " + sortColumnDirection);
            }
            //Search  
            if (!string.IsNullOrEmpty(searchValue))
            {
                sheetsData = sheetsData.Where(m => m.SheetName == searchValue);
            }

            //total number of rows counts   
            recordsTotal = sheetsData.Count();
            //Paging   
            var data = sheetsData.Skip(skip).Take(pageSize).ToList();
            //Returning Json Data  
            return Json(new { draw = draw, recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = data });



        }

        public IActionResult ImportEmployee(int? ExcelSheetImport)
        {

            if (ExcelSheetImport == null){
                var return_model = new ImportedEmployeeViewModel()
                {
                    Id = 1,
                    isShow = false
                };
                return View(return_model);
            }

            var model = new ImportedEmployeeViewModel()
            {
                Id = (int)ExcelSheetImport,
                isShow = true
            };

            return View(model);
        }

        [HttpPost]
        public IActionResult FailedSheetRows(int? ExcelSheetImport)
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

            var excelSheetFailedRows = (from sheet in _context.ExcelSheetImports
                                        select sheet.FailedJson).FirstOrDefault();

            var failedRowsJson = JsonConvert.DeserializeObject<List<FailedRows>>(excelSheetFailedRows);


           
            //Sorting  
            if (!(string.IsNullOrEmpty(sortColumn) && string.IsNullOrEmpty(sortColumnDirection)))
            {
                //  appointmentData = appointmentData.OrderBy(sortColumn + " " + sortColumnDirection);
            }
            //Search  
            if (!string.IsNullOrEmpty(searchValue))
            {
                failedRowsJson = (List<FailedRows>)failedRowsJson.Where(m => m.EmployeeName == searchValue);
            }

            //total number of rows counts   
            recordsTotal = failedRowsJson.Count();
            //Paging   
            var data = failedRowsJson.Skip(skip).Take(pageSize).ToList();
            //Returning Json Data  
            return Json(new { draw = draw, recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = data });



        }

        [HttpPost]
        public IActionResult SuccessSheetRows(int? ExcelSheetImport)
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

            var excelSheetImportedRows = (from sheet in _context.ExcelSheetImports
                                        select sheet.ImportedJson).FirstOrDefault();

            var importedRowsJson = JsonConvert.DeserializeObject<List<ImportedRows>>(excelSheetImportedRows);



            //Sorting  
            if (!(string.IsNullOrEmpty(sortColumn) && string.IsNullOrEmpty(sortColumnDirection)))
            {
                //  appointmentData = appointmentData.OrderBy(sortColumn + " " + sortColumnDirection);
            }
            //Search  
            if (!string.IsNullOrEmpty(searchValue))
            {
                importedRowsJson = (List<ImportedRows>)importedRowsJson.Where(m => m.EmployeeName == searchValue);
            }

            //total number of rows counts   
            recordsTotal = importedRowsJson.Count();
            //Paging   
            var data = importedRowsJson.Skip(skip).Take(pageSize).ToList();
            //Returning Json Data  
            return Json(new { draw = draw, recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = data });



        }




        [HttpPost]
        public IActionResult ImportEmployee(IFormFile file) 
        {
            List<string> employeeList = new List<string>();
            List<FailedRows> failedRows = new List<FailedRows>();
            List<ImportedRows> importedRows = new List<ImportedRows>();
            ExcelSheetImport sheet = new ExcelSheetImport();


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

            foreach (var item in employeeList) {

                var checkEmployee = _context.Employees.Where(x => x.Name == item).FirstOrDefault();
                if (checkEmployee == null)
                {
                    var rowEmployee = _context.Employees.Add(new Employee()
                    {
                        Name = item,
                        TenantId = 1,
                        CreatedDate = DateTime.Now,
                        CreatedBy = 2
                    });

                    importedRows.Add(new ImportedRows()
                    {
                        EmployeeName = item,
                        AddedDate = DateTime.Now.Date
                    });
                }
                else {
                    failedRows.Add(new FailedRows()
                    {
                        EmployeeName = item,
                        Error = "Employee Name already exsists",
                        AddedDate = DateTime.Now.Date
                    });
                }


                
               

            }

            sheet = new ExcelSheetImport()
            {
                SheetName = file.FileName,
                RowsCount = failedRows.Count() + importedRows.Count(),
                RowsImported = importedRows.Count(),
                RowsFailed = failedRows.Count(),
                ImportedJson = JsonConvert.SerializeObject(importedRows),
                FailedJson = JsonConvert.SerializeObject(failedRows),
                ImportDate = DateTime.Now.Date,
                ImportedBy = 1
            };

            _context.ExcelSheetImports.Add(sheet);
            _context.SaveChanges();


            var model = new ImportedEmployeeViewModel()
            {
                Id = (int)sheet.Id,
                isShow = true
            };

            return View(model);
        }



    }

    class FailedRows
    { 
        public string EmployeeName { get; set; }
        public string Error { get; set; }
        public DateTime AddedDate { get; set; }
    }

    class ImportedRows
    { 
        public string EmployeeName { get; set; }
        public DateTime AddedDate { get; set; }
    }
}
