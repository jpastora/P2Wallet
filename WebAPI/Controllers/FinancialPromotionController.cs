using CoreApp;
using DTOs;
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
                return Ok("Financial promotion created successfully.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
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
                return Ok("Financial promotion updated successfully.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet]
        [Route("RetrieveAll")]
        public ActionResult<List<FinancialPromotion>> RetrieveAllPromotions()
        {
            try
            {
                var manager = new FinancialPromotionManager();
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
                var manager = new FinancialPromotionManager();
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
        public ActionResult DeletePromotion(FinancialPromotion promotion)
        {
            try
            {
                var manager = new FinancialPromotionManager();
                manager.DeletePromotion(promotion);
                return Ok("Financial promotion deleted successfully.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}
