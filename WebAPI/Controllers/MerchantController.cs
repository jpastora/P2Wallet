using CoreApp;
using DTOs;
using Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MerchantController : ControllerBase
    {
        [HttpPost]
        [Route("Create")]
        public ActionResult CreateMerchant(Merchant merchant)
        {
            try
            {
                var merchantManager = new MerchantManager();
                merchantManager.CreateMerchant(merchant);

                return Ok(new
                {
                    message = "Comercio agregado exitosamente.",
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
        public ActionResult UpdateMerchant(Merchant merchant)
        {
            try
            {
                var merchantManager = new MerchantManager();
                merchantManager.UpdateMerchant(merchant);

                return Ok(new
                {
                    message = "Comercio actualizado exitosamente.",
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
        public ActionResult RetrieveAllMerchants()
        {
            try
            {
                var merchantManager = new MerchantManager();
                var listMerchantResult = merchantManager.RetrieveAllMerchants();

                return Ok(new
                {
                    message = "Comercios recuperados exitosamente.",
                    icon = "success",
                    title = "¡Éxito!",
                    data = listMerchantResult
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
        public ActionResult RetrieveMerchantById(int id)
        {
            try
            {
                var merchantManager = new MerchantManager();
                var merchantResult = merchantManager.RetrieveMerchantById(id);

                if (merchantResult == null)
                {
                    return NotFound(new
                    {
                        message = "Comercio no encontrado.",
                        icon = "warning",
                        title = "Aviso"
                    });
                }

                return Ok(new
                {
                    message = "Comercio encontrado.",
                    icon = "success",
                    title = "¡Éxito!",
                    data = merchantResult
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
        public ActionResult DeleteMerchant(Merchant merchant)
        {
            try
            {
                var merchantManager = new MerchantManager();
                merchantManager.DeleteMerchant(merchant);

                return Ok(new
                {
                    message = "Comercio eliminado exitosamente.",
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
        public ActionResult RetrieveMerchantByTaxID(string taxId)
        {
            try
            {
                var merchantManager = new MerchantManager();
                var merchantResult = merchantManager.RetrieveByTaxID(taxId);

                if (merchantResult == null)
                {
                    return NotFound(new
                    {
                        message = "No se encontró el comercio con ese Tax ID.",
                        icon = "warning",
                        title = "Aviso"
                    });
                }

                return Ok(new
                {
                    message = "Comercio encontrado.",
                    icon = "success",
                    title = "¡Éxito!",
                    data = merchantResult
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
