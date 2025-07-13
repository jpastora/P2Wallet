using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers
{
    [Route("api/cloudinary")]
    [ApiController]
    public class CloudinaryController : ControllerBase
    {
        private string _cloudinaryUrl;

        public CloudinaryController(IConfiguration configuration) 
        { 
            _cloudinaryUrl = configuration.GetSection("Cloudinary").GetSection("URL").Value;
        }

        [HttpPost]
        [Route("save")]
        public async Task<ActionResult> SaveImage(IFormFile photo)
        {
            try
            {
                Cloudinary cloudinary = new Cloudinary(_cloudinaryUrl);
                var fileName = photo.FileName;
                var fileWithPath = Path.Combine("Uploads", fileName);
                var stream = new FileStream(fileWithPath, FileMode.Create);
                photo.CopyTo(stream);
                stream.Close();
                var uploadParams = new ImageUploadParams
                {
                    File = new FileDescription(fileWithPath),
                    UseFilename = true,
                    Overwrite = true,
                    Folder = "Yavi"
                };
                var uploadResult = await cloudinary.UploadAsync(uploadParams);
                System.IO.File.Delete(fileWithPath); // Limpia el archivo local después de subirlo
                return Ok(uploadResult);    
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

    }
}
