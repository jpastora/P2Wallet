using CoreApp;
using DTOs;
using Exceptions;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FinancialPromotionController : ControllerBase
    {
        [HttpPost]
        [Route("Create")]
        public ActionResult CreatePromotion(FinancialPromotion promotion)
        {
            try
            {
                var manager = new FinancialPromotionManager();
                manager.CreatePromotion(promotion);

                return Ok(new
                {
                    message = "Promoción financiera creada exitosamente.",
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
        public ActionResult UpdatePromotion(FinancialPromotion promotion)
        {
            try
            {
                var manager = new FinancialPromotionManager();
                manager.UpdatePromotion(promotion);

                return Ok(new
                {
                    message = "Promoción financiera actualizada exitosamente.",
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
        public ActionResult RetrieveAllPromotions()
        {
            try
            {
                var manager = new FinancialPromotionManager();
                var list = manager.RetrieveAllPromotions();

                return Ok(new
                {
                    message = "Promociones financieras recuperadas exitosamente.",
                    icon = "success",
                    title = "¡Éxito!",
                    data = list
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
        public ActionResult RetrievePromotionById(int id)
        {
            try
            {
                var manager = new FinancialPromotionManager();
                var result = manager.RetrievePromotionById(id);

                if (result == null)
                {
                    return NotFound(new
                    {
                        message = "Promoción financiera no encontrada.",
                        icon = "warning",
                        title = "Aviso"
                    });
                }

                return Ok(new
                {
                    message = "Promoción financiera encontrada.",
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
        public ActionResult DeletePromotion(FinancialPromotion promotion)
        {
            try
            {
                var manager = new FinancialPromotionManager();
                manager.DeletePromotion(promotion);

                return Ok(new
                {
                    message = "Promoción financiera eliminada exitosamente.",
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
