using CoreApp;
using DTOs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FinancialEntityController : ControllerBase
    {

        [HttpPost]
        [Route("Create")]
        public ActionResult CreateFinancialEntity(FinancialEntity financialEntity)
        {
            try
            {
                var financialEntityManager = new FinancialEntityManager();
                financialEntityManager.CreateFinancialEntity(financialEntity);
                return Ok("Financial entity created successfully.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPut]
        [Route("Update")]
        public ActionResult UpdateFinancialEntity(FinancialEntity financialEntity)
        {
            try
            {
                var financialEntityManager = new FinancialEntityManager();
                financialEntityManager.UpdateFinancialEntity(financialEntity);
                return Ok("Financial entity updated successfully.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet]
        [Route("RetrieveAll")]
        public ActionResult<List<FinancialEntity>> RetrieveAllFinancialEntities()
        {
            try
            {
                var financialEntityManager = new FinancialEntityManager();
                var listFinancialEntityResult = financialEntityManager.RetrieveAllFinancialEntities();
                return Ok(listFinancialEntityResult);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet]
        [Route("RetrieveById/{id}")]
        public ActionResult<FinancialEntity> RetrieveFinancialEntityById(int id)
        {
            try
            {
                var financialEntityManager = new FinancialEntityManager();
                var financialEntityResult = financialEntityManager.RetrieveFinancialEntityById(id);
                if (financialEntityResult == null)
                {
                    return NotFound("Financial entity not found.");
                }
                return Ok(financialEntityResult);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpDelete]
        [Route("Delete")]
        public ActionResult Delete(FinancialEntity financialEntity)
        {
            try
            {
                var financialEntityManager = new FinancialEntityManager();
                financialEntityManager.DeleteFinancialEntity(financialEntity);
                return Ok("Financial entity deleted successfully.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }


        }
    }
}
