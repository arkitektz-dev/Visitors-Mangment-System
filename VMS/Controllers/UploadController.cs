using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
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

        [HttpGet("upload-barcode")]
        public async Task<IActionResult> UploadBarcode(string number)
        {

            string uniqueFileName = null;

            if (number != null)
            {

                BarcodeLib.Barcode b = new BarcodeLib.Barcode();
                Image img = b.Encode(BarcodeLib.TYPE.CODE39, number.ToString(), Color.Black, Color.White, 290, 120);
                Bitmap bImage = (Bitmap)img;  // Your Bitmap Image
                System.IO.MemoryStream ms = new MemoryStream();

                string path = Path.Combine(environment.WebRootPath, "barcode");
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }

                string uploadsFolder = Path.Combine(environment.WebRootPath, "barcode");
                uniqueFileName = Guid.NewGuid().ToString() + "." + "png";
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    bImage.Save(fileStream, ImageFormat.Jpeg);
                }
            } 

            return Ok($"{this.Request.Scheme}://{this.Request.Host}{this.Request.PathBase}/barcode/{uniqueFileName}");
        }

    }
}
