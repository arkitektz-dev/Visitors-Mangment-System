using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VMS.Models;
using VMS.ViewModels;

namespace VMS.Controllers
{
    public class SettingController : Controller
    {
         
        private VMSDbContext _context; 
        public SettingController(VMSDbContext context)
        {
            _context = context;
        }

        public IActionResult PrinterSettings()
        {
            var checkPrinterValue = _context.Settings.Where(x => x.SettingKey == "Printer").FirstOrDefault();
            if (checkPrinterValue != null)
            {
                var model = new PrinterSettingViewModel();
                model.PrinterName = checkPrinterValue.SettingValue;
                return View(model);
            }
            else {
                var model = new PrinterSettingViewModel();
                model.PrinterName = "Not Printer Added";
                return View(model);
            }

           
        }

        [HttpPost]
        public IActionResult PrinterSettings(PrinterSettingViewModel model)
        {
            var checkPrinterValue = _context.Settings.Where(x => x.SettingKey == "Printer").FirstOrDefault();
            if (checkPrinterValue != null)
            {
                checkPrinterValue.SettingValue = model.PrinterName;
                _context.SaveChanges();
            }
            else {
                Setting row = new Setting();
                row.SettingKey = "Printer";
                row.SettingValue = model.PrinterName;
                _context.Settings.Add(row);
                _context.SaveChanges();
            }

            return View(model);
        }


        public IActionResult CredentialsManagers()
        {
            var apiKey = _context.Settings.Where(x => x.SettingKey == "ApiKey").FirstOrDefault();
             
            var model = new EmailConfigViewModel()
            {
                ApiKey = apiKey.SettingValue
            };
            

            return View(model);
        }

        [HttpPost]
        public IActionResult CredentialsManagers(EmailConfigViewModel model)
        {
            var emailConfig = _context.Settings.Where(x => x.SettingKey == "ApiKey").FirstOrDefault();
            if (emailConfig != null) {
                emailConfig.SettingValue = model.ApiKey;
                _context.SaveChanges();
            }

            return View(model);
        }



    }
}
