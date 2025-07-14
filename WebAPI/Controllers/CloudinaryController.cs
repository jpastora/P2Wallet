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
        [Route("Save")]
        public async Task<ActionResult> SaveImage(IFormFile photo)
        {
            try
            {
                Cloudinary cloudinary = new Cloudinary(_cloudinaryUrl);
                var fileName = photo.FileName;
                var fileWithPath = Path.Combine("Uploads", fileName); // Uploadas siempre termina sin datos, es temporal
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
                return StatusCode(500, ex.Message);
            }
        }

        [HttpDelete]
        [Route("Delete")]
        public async Task<ActionResult> DeleteImage(string publicId)
        {
            try
            {
                Cloudinary cloudinary = new Cloudinary(_cloudinaryUrl);
                var deleteParams = new DeletionParams(publicId);
                var result = await cloudinary.DestroyAsync(deleteParams);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }


    }
}
