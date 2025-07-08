using DTOs;
using Microsoft.AspNetCore.Mvc;
using DataAccess.CRUD;
using System.Collections.Generic;

[ApiController]
[Route("api/[controller]")]
public class MerchantPromotionsController : ControllerBase
{
    private readonly MerchantPromotionCrudFactory _factory = new MerchantPromotionCrudFactory();

    [HttpPost]
    public IActionResult Create([FromBody] MerchantPromotions promo)
    {
        _factory.Create(promo);
        return Ok("Promotion created.");
    }

    [HttpPut("{id}")]
    public IActionResult Update(int id, [FromBody] MerchantPromotions promo)
    {
        promo.PromotionID = id;
        _factory.Update(promo);
        return Ok("Promotion updated.");
    }

    [HttpDelete("{id}")]
    public IActionResult Delete(int id)
    {
        var promo = new MerchantPromotions { PromotionID = id };
        _factory.Delete(promo);
        return Ok("Promotion deleted.");
    }

    [HttpGet("{id}")]
    public IActionResult GetById(int id)
    {
        var result = _factory.RetrieveById<MerchantPromotions>(id);
        if (result == null) return NotFound();
        return Ok(result);
    }

    [HttpGet]
    public IActionResult GetAll()
    {
        var result = _factory.RetrieveAll<MerchantPromotions>();
        return Ok(result);
    }
}
