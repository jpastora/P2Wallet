using CoreApp;
using DTOs;
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
                return Ok("Account movement created successfully.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
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
                return Ok("Account movement updated successfully.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet]
        [Route("RetrieveAll")]
        public ActionResult<List<AccountMovement>> RetrieveAllAccountMovements()
        {
            try
            {
                var manager = new AccountMovementManager();
                var result = manager.RetrieveAllAccountMovements();
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet]
        [Route("RetrieveById/{id}")]
        public ActionResult<AccountMovement> RetrieveAccountMovementById(int id)
        {
            try
            {
                var manager = new AccountMovementManager();
                var result = manager.RetrieveAccountMovementById(id);
                if (result == null)
                {
                    return NotFound("Account movement not found.");
                }
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
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
                return Ok("Account movement deleted successfully.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}

