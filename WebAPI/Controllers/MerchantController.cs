using CoreApp;
using DTOs;
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
                // Retorna un objeto JSON con mensaje de éxito
                return Ok(new
                {
                    message = "Comercio agregado exitosamente.",
                    icon = "success",
                    title = "¡Éxito!"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    // Retorna un objeto JSON con mensaje de error
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
                return StatusCode(500, new
                {
                    // Retorna un objeto JSON con mensaje de error
                    message = ex.Message,
                    icon = "error",
                    title = "Error"
                });
            }
        }

        [HttpGet]
        [Route("RetrieveAll")]
        public ActionResult<List<User>> RetrieveAllMerchants()
        {
            try
            {
                var merchantManager = new MerchantManager();
                var listMerchantResult = merchantManager.RetrieveAllMerchants();
                return Ok(listMerchantResult);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet]
        [Route("RetrieveById/{id}")]
        public ActionResult<Merchant> RetrieveMerchantById(int id)
        {
            try
            {
                var merchantManager = new MerchantManager();
                var merchantResult = merchantManager.RetrieveMerchantById(id);
                if (merchantResult == null)
                {
                    return NotFound("Merchant not found.");
                }
                return Ok(merchantResult);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
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
                return Ok("Merchant deleted successfully.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}
