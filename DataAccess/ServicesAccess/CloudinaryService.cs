using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Http;
using System.IO;
using System.Threading.Tasks;

namespace DataAccess.ServicesAccess
{
    public class CloudinaryService
    {
        private readonly Cloudinary _cloudinary;

        public CloudinaryService()
        {
            var envPath = Path.Combine(Directory.GetCurrentDirectory(), "DataAccess", "ServicesAccess", "Cloudinary.env");
            DotNetEnv.Env.Load(envPath);

            var url = Environment.GetEnvironmentVariable("CLOUDINARY_URL");
            _cloudinary = new Cloudinary(url);
        }

        public async Task<string> SubirImagenAsync(IFormFile archivo)
        {
            await using var stream = archivo.OpenReadStream();

            var uploadParams = new ImageUploadParams
            {
                File = new FileDescription(archivo.FileName, stream),
                Transformation = new Transformation().Width(600).Height(400).Crop("fill")
            };

            var uploadResult = await _cloudinary.UploadAsync(uploadParams);
            return uploadResult.SecureUrl.ToString();
        }
    }
}
