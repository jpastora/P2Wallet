using Microsoft.AspNetCore.Mvc;
using DTOs;
using CoreApp;

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
            _manager.CreateUserEntity(userEntity);
            return Ok(new { message = "Usuario asignado al banco exitosamente" });
        }

        [HttpDelete("Delete")]
        public IActionResult Delete([FromBody] UserEntity userEntity)
        {
            _manager.DeleteUserEntity(userEntity);
            return Ok();
        }

        [HttpGet("RetrieveAll")]
        public IActionResult RetrieveAll()
        {
            var result = _manager.RetrieveAllUserEntities();
            return Ok(result);
        }

        // Nuevo: obtener entidades financieras completas asociadas a un usuario
        [HttpGet("FinancialEntitiesByUser/{userId}")]
        public IActionResult GetFinancialEntitiesByUser(int userId)
        {
            var result = _manager.GetFinancialEntitiesForUser(userId);
            return Ok(result);
        }
    }
}
