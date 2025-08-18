using Microsoft.AspNetCore.Mvc;
using DTOs;
using CoreApp;
using Exceptions;

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserEntityController : ControllerBase
    {
        private readonly UserEntityManager _manager = new UserEntityManager();

        [HttpPost("Create")]
        public IActionResult Create([FromBody] UserEntity userEntity)
        {
            try
            {
                _manager.CreateUserEntity(userEntity);
                return Ok(new
                {
                    message = "Usuario asignado al banco exitosamente",
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
        public IActionResult Delete([FromBody] UserEntity userEntity)
        {
            try
            {
                _manager.DeleteUserEntity(userEntity);
                return Ok(new
                {
                    message = "Asignación de usuario eliminada correctamente",
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
                var result = _manager.RetrieveAllUserEntities();
                return Ok(new
                {
                    message = "Usuarios asignados recuperados exitosamente",
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

        [HttpGet("FinancialEntitiesByUser/{userId}")]
        public IActionResult GetFinancialEntitiesByUser(int userId)
        {
            try
            {
                var result = _manager.GetFinancialEntitiesForUser(userId);
                return Ok(new
                {
                    message = "Entidades financieras del usuario recuperadas exitosamente",
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
