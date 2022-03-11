﻿using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace VMS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UploadController : ControllerBase
    {
        private IWebHostEnvironment environment; 
        public UploadController(IWebHostEnvironment _environment)
        {
            environment = _environment;
        }

        [HttpPost("upload-barcode")]
        public async Task<IActionResult> UploadBarcode(IFormFile ProfileImage)
        {

            string uniqueFileName = null;

            if (ProfileImage != null)
            {
                string uploadsFolder = Path.Combine(environment.WebRootPath, "barcode");
                uniqueFileName = Guid.NewGuid().ToString() + "." + ProfileImage.FileName.Split(".").LastOrDefault();
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    ProfileImage.CopyTo(fileStream);
                }
            } 

            return Ok($"{this.Request.Scheme}://{this.Request.Host}{this.Request.PathBase}/barcode/{uniqueFileName}");
        }

    }
}
