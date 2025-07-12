using CoreApp;
using DTOs;
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
                return Ok("Merchant promotion created successfully.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost]
        [Route("Update")]
        public ActionResult UpdatePromotion(MerchantPromotion promotion)
        {
            try
            {
                var manager = new MerchantPromotionManager();
                manager.UpdatePromotion(promotion);
                return Ok("Merchant promotion updated successfully.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet]
        [Route("RetrieveAll")]
        public ActionResult<List<MerchantPromotion>> RetrieveAllPromotions()
        {
            try
            {
                var manager = new MerchantPromotionManager();
                var list = manager.RetrieveAllPromotions();
                return Ok(list);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
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
                    return NotFound("Promotion not found.");

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
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
                return Ok("Merchant promotion deleted successfully.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}
