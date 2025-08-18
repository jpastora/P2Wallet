using Microsoft.AspNetCore.Mvc;
using DTOs;
using CoreApp;
using Exceptions;

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
            try
            {
                _manager.CreateUserMerchant(userMerchant);
                return Ok(new
                {
                    message = "Usuario asignado al comercio exitosamente",
                    icon = "success",
                    title = "¡Éxito!"
                });
            }
            catch (Exception ex)
            {
                ExceptionLogger.LogException(ex);
                return StatusCode(500, new
                {
                    message = ex.Message,
                    icon = "error",
                    title = "Error"
                });
            }
        }

        [HttpDelete("Delete")]
        public IActionResult Delete([FromBody] UserMerchant userMerchant)
        {
            try
            {
                _manager.DeleteUserMerchant(userMerchant);
                return Ok(new
                {
                    message = "Asignación de usuario al comercio eliminada correctamente",
                    icon = "success",
                    title = "¡Éxito!"
                });
            }
            catch (Exception ex)
            {
                ExceptionLogger.LogException(ex);
                return StatusCode(500, new
                {
                    message = ex.Message,
                    icon = "error",
                    title = "Error"
                });
            }
        }

        [HttpGet("RetrieveAll")]
        public IActionResult RetrieveAll()
        {
            try
            {
                var result = _manager.RetrieveAllUserMerchants();
                return Ok(new
                {
                    message = "Usuarios asignados a comercios recuperados exitosamente",
                    icon = "success",
                    title = "¡Éxito!",
                    data = result
                });
            }
            catch (Exception ex)
            {
                ExceptionLogger.LogException(ex);
                return StatusCode(500, new
                {
                    message = ex.Message,
                    icon = "error",
                    title = "Error"
                });
            }
        }

        [HttpGet("MerchantsByUser/{userId}")]
        public IActionResult GetMerchantsByUser(int userId)
        {
            try
            {
                var result = _manager.GetMerchantsForUser(userId);
                return Ok(new
                {
                    message = "Comercios del usuario recuperados exitosamente",
                    icon = "success",
                    title = "¡Éxito!",
                    data = result
                });
            }
            catch (Exception ex)
            {
                ExceptionLogger.LogException(ex);
                return StatusCode(500, new
                {
                    message = ex.Message,
                    icon = "error",
                    title = "Error"
                });
            }
        }
    }
}
