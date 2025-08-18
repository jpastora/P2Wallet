using CoreApp;
using DTOs;
using Exceptions;
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

                return Ok(new
                {
                    message = "Entidad financiera agregada exitosamente.",
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
        public ActionResult UpdateFinancialEntity(FinancialEntity financialEntity)
        {
            try
            {
                var financialEntityManager = new FinancialEntityManager();
                financialEntityManager.UpdateFinancialEntity(financialEntity);

                return Ok(new
                {
                    message = "Entidad financiera actualizada exitosamente.",
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
        public ActionResult RetrieveAllFinancialEntities()
        {
            try
            {
                var financialEntityManager = new FinancialEntityManager();
                var listFinancialEntityResult = financialEntityManager.RetrieveAllFinancialEntities();

                return Ok(new
                {
                    message = "Entidades financieras recuperadas exitosamente.",
                    icon = "success",
                    title = "¡Éxito!",
                    data = listFinancialEntityResult
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
        public ActionResult RetrieveFinancialEntityById(int id)
        {
            try
            {
                var financialEntityManager = new FinancialEntityManager();
                var financialEntityResult = financialEntityManager.RetrieveFinancialEntityById(id);

                if (financialEntityResult == null)
                {
                    return NotFound(new
                    {
                        message = "Entidad financiera no encontrada.",
                        icon = "warning",
                        title = "Aviso"
                    });
                }

                return Ok(new
                {
                    message = "Entidad financiera encontrada.",
                    icon = "success",
                    title = "¡Éxito!",
                    data = financialEntityResult
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
        public ActionResult Delete(FinancialEntity financialEntity)
        {
            try
            {
                var financialEntityManager = new FinancialEntityManager();
                financialEntityManager.DeleteFinancialEntity(financialEntity);

                return Ok(new
                {
                    message = "Entidad financiera eliminada exitosamente.",
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
        [Route("GetByTaxID")]
        public ActionResult RetrieveEntityByTaxID(string taxId)
        {
            try
            {
                var financialEntityManager = new FinancialEntityManager();
                var entityResult = financialEntityManager.RetrieveByTaxID(taxId);

                if (entityResult == null)
                {
                    return NotFound(new
                    {
                        message = "No se encontró la entidad financiera con ese Tax ID.",
                        icon = "warning",
                        title = "Aviso"
                    });
                }

                return Ok(new
                {
                    message = "Entidad financiera encontrada.",
                    icon = "success",
                    title = "¡Éxito!",
                    data = entityResult
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
