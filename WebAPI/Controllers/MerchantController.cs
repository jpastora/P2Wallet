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
                return Ok("Merchant created successfully.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost]
        [Route("Update")]
        public ActionResult UpdateMerchant(Merchant merchant)
        {
         try 
         {
             var merchantManager = new MerchantManager();
             merchantManager.UpdateMerchant(merchant);
             return Ok("Merchant updated successfully.");
         }
         catch (Exception ex)
         {
             return StatusCode(500, ex.Message);
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
