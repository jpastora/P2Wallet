using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Exceptions;
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

                await using var stream = photo.OpenReadStream();
                var uploadParams = new ImageUploadParams
                {
                    File = new FileDescription(photo.FileName, stream),
                    UseFilename = true,
                    Overwrite = true,
                    Folder = "Yavi"
                };

                var uploadResult = await cloudinary.UploadAsync(uploadParams);
                return Ok(uploadResult);
            }
            catch (Exception ex)
            {
                ExceptionLogger.LogException(ex);
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
                ExceptionLogger.LogException(ex);
                return StatusCode(500, ex.Message);
            }
        }


    }
}
