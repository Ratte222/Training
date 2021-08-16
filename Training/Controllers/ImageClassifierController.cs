using BLL.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace Training.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ImageClassifierController : ControllerBase
    {
        private readonly IComputerVision _computerVision;

        public ImageClassifierController(IComputerVision computerVision)
        {
            _computerVision = computerVision;
        }

        [HttpPost("IdentifiCatOrDog")]
        [ProducesResponseType(typeof(string), 200)]
        //[ProducesResponseType(typeof(ValidationException), 400)]
        [ProducesResponseType(typeof(string), 500)]
        public IActionResult IdentifiCatOrDog(IFormFile file)
        {
            string path = SaveFiles(file);
            string result = _computerVision.CatVsDogClassifier_(path);
            if(System.IO.File.Exists(path))
            {
                System.IO.File.Delete(path);
            }
            return Ok(result);
        }


        private string SaveFiles(IFormFile file)
        {
            var fileName = string.Empty;
            string PathDB = string.Empty;
            string newFileName = string.Empty;
            if (file.Length > 0)
            {
                //Getting FileName
                fileName = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName.Trim('"');

                //Assigning Unique Filename (Guid)
                var myUniqueFileName = Convert.ToString(Guid.NewGuid());

                //Getting file Extension
                var FileExtension = Path.GetExtension(fileName);

                // concating  FileName + FileExtension
                newFileName = myUniqueFileName + FileExtension;

                // Combines two strings into a path.
                string partialPath = Path.Combine(Directory.GetCurrentDirectory(), "Temp", "PhotoForMl");
                fileName = Path.Combine(partialPath, newFileName);
                if (!Directory.Exists(partialPath))
                    Directory.CreateDirectory(partialPath);
                // if you want to store path of folder in database
                PathDB = fileName;

                using (FileStream fs = System.IO.File.Create(fileName))
                {
                    file.CopyTo(fs);
                    fs.Flush();
                }
            }
            return PathDB;
        }
    }
}
