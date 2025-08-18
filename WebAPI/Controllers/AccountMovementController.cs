using CoreApp;
using DTOs;
using Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountMovementController : ControllerBase
    {
        [HttpPost]
        [Route("Create")]
        public ActionResult CreateAccountMovement(AccountMovement movement)
        {
            try
            {
                var manager = new AccountMovementManager();
                manager.CreateAccountMovement(movement);

                return Ok(new
                {
                    message = "Movimiento de cuenta creado exitosamente.",
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

        [HttpPut]
        [Route("Update")]
        public ActionResult UpdateAccountMovement(AccountMovement movement)
        {
            try
            {
                var manager = new AccountMovementManager();
                manager.UpdateAccountMovement(movement);

                return Ok(new
                {
                    message = "Movimiento de cuenta actualizado exitosamente.",
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

        [HttpGet]
        [Route("RetrieveAll")]
        public ActionResult RetrieveAllAccountMovements()
        {
            try
            {
                var manager = new AccountMovementManager();
                var result = manager.RetrieveAllAccountMovements();

                return Ok(new
                {
                    message = "Movimientos de cuenta recuperados exitosamente.",
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

        [HttpGet]
        [Route("RetrieveById/{id}")]
        public ActionResult RetrieveAccountMovementById(int id)
        {
            try
            {
                var manager = new AccountMovementManager();
                var result = manager.RetrieveAccountMovementById(id);

                if (result == null)
                {
                    return NotFound(new
                    {
                        message = "Movimiento de cuenta no encontrado.",
                        icon = "warning",
                        title = "Aviso"
                    });
                }

                return Ok(new
                {
                    message = "Movimiento de cuenta encontrado.",
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

        [HttpDelete]
        [Route("Delete")]
        public ActionResult DeleteAccountMovement(AccountMovement movement)
        {
            try
            {
                var manager = new AccountMovementManager();
                manager.DeleteAccountMovement(movement);

                return Ok(new
                {
                    message = "Movimiento de cuenta eliminado exitosamente.",
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
    }
}
