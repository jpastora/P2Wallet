using Microsoft.AspNetCore.Mvc;
using DTOs;
using CoreApp;

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserMerchantController : ControllerBase
    {
        private readonly UserMerchantManager _manager = new UserMerchantManager();

        [HttpPost("Create")]
        public IActionResult Create([FromBody] UserMerchant userMerchant)
        {
            _manager.CreateUserMerchant(userMerchant);
            return Ok();
        }

        [HttpDelete("Delete")]
        public IActionResult Delete([FromBody] UserMerchant userMerchant)
        {
            _manager.DeleteUserMerchant(userMerchant);
            return Ok();
        }

        [HttpGet("RetrieveAll")]
        public IActionResult RetrieveAll()
        {
            var result = _manager.RetrieveAllUserMerchants();
            return Ok(result);
        }

        // Nuevo: obtener comercios completos asociados a un usuario
        [HttpGet("MerchantsByUser/{userId}")]
        public IActionResult GetMerchantsByUser(int userId)
        {
            var result = _manager.GetMerchantsForUser(userId);
            return Ok(result);
        }
    }
}
