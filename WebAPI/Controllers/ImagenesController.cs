using DataAccess.ServicesAccess;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace WebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ImagenesController : ControllerBase
    {
        private readonly CloudinaryService _cloudinaryService;

        public ImagenesController(CloudinaryService cloudinaryService)
        {
            _cloudinaryService = cloudinaryService;
        }

        [HttpPost("subir")]
        public async Task<IActionResult> SubirImagen([FromForm] IFormFile archivo)
        {
            if (archivo == null || archivo.Length == 0)
                return BadRequest("Archivo inválido.");

            var url = await _cloudinaryService.SubirImagenAsync(archivo);
            return Ok(new { url });
        }
    }
}
