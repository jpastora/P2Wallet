using CoreApp;
using DTOs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;


namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdministratorController : ControllerBase
    {
        [HttpPost]
        [Route("Create")]
        public ActionResult CreateAdministrator(Administrator administrator)
        {
            try
            {
                var administratorManager = new AdministratorManager();
                administratorManager.CreateAdministrator(administrator);
                return Ok("Administrator created successfully.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost]
        [Route("Update")]
        public ActionResult UpdateAdministrator(Administrator administrator)
        {
            try
            {
                var administratorManager = new AdministratorManager();
                administratorManager.UpdateAdministrator(administrator);
                return Ok("Administrator updated successfully.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet]
        [Route("RetrieveAll")]
        public ActionResult<List<Administrator>> RetrieveAllAdministrators()
        {
            try
            {
                var administratorManager = new AdministratorManager();
                var listAdministratorResult = administratorManager.RetrieveAllAdministrators();
                return Ok(listAdministratorResult);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet]
        [Route("RetrieveById/{id}")]
        public ActionResult<Administrator> RetrieveAdministratorById(int id)
        {
            try
            {
                var administratorManager = new AdministratorManager();
                var administratorResult = administratorManager.RetrieveAdministratorById(id);
                if (administratorResult == null)
                {
                    return NotFound("Administrator not found.");
                }
                return Ok(administratorResult);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }


        }

        [HttpDelete]
        [Route("Delete")]
        public ActionResult DeleteAdministrator(Administrator administrator)
        {
            try
            {
                var administratorManager = new AdministratorManager();
                administratorManager.DeleteAdministrator(administrator);
                return Ok("Administrator deleted successfully.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}