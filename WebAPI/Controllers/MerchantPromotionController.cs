using CoreApp;
using DTOs;
using Exceptions;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MerchantPromotionController : ControllerBase
    {
        [HttpPost]
        [Route("Create")]
        public ActionResult CreatePromotion(MerchantPromotion promotion)
        {
            try
            {
                var manager = new MerchantPromotionManager();
                manager.CreatePromotion(promotion);

                return Ok(new
                {
                    message = "Promoción de comercio creada exitosamente.",
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
        public ActionResult UpdatePromotion(MerchantPromotion promotion)
        {
            try
            {
                var manager = new MerchantPromotionManager();
                manager.UpdatePromotion(promotion);

                return Ok(new
                {
                    message = "Promoción de comercio actualizada exitosamente.",
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
                var manager = new MerchantPromotionManager();
                var list = manager.RetrieveAllPromotions();

                return Ok(new
                {
                    message = "Promociones de comercio recuperadas exitosamente.",
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
                var manager = new MerchantPromotionManager();
                var result = manager.RetrievePromotionById(id);

                if (result == null)
                {
                    return NotFound(new
                    {
                        message = "Promoción de comercio no encontrada.",
                        icon = "warning",
                        title = "Aviso"
                    });
                }

                return Ok(new
                {
                    message = "Promoción de comercio encontrada.",
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
        public ActionResult DeletePromotion(MerchantPromotion promotion)
        {
            try
            {
                var manager = new MerchantPromotionManager();
                manager.DeletePromotion(promotion);

                return Ok(new
                {
                    message = "Promoción de comercio eliminada exitosamente.",
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
