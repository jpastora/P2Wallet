using DTOs;
using Microsoft.AspNetCore.Mvc;
using DataAccess.CRUD;
using System.Collections.Generic;

[ApiController]
[Route("api/[controller]")]
public class FinancialEntityPromotionsController : ControllerBase
{
    private readonly FinancialEntityPromotionsCrudFactory _factory = new FinancialEntityPromotionsCrudFactory();

    [HttpPost]
    public IActionResult Create([FromBody] FinancialEntityPromotions promo)
    {
        _factory.Create(promo);
        return Ok("Promotion created.");
    }

    [HttpPut("{id}")]
    public IActionResult Update(int id, [FromBody] FinancialEntityPromotions promo)
    {
        promo.PromotionID = id;
        _factory.Update(promo);
        return Ok("Promotion updated.");
    }

    [HttpDelete("{id}")]
    public IActionResult Delete(int id)
    {
        var promo = new FinancialEntityPromotions { PromotionID = id };
        _factory.Delete(promo);
        return Ok("Promotion deleted.");
    }

    [HttpGet("{id}")]
    public IActionResult GetById(int id)
    {
        var result = _factory.RetrieveById<FinancialEntityPromotions>(id);
        if (result == null) return NotFound();
        return Ok(result);
    }

    [HttpGet]
    public IActionResult GetAll()
    {
        var result = _factory.RetrieveAll<FinancialEntityPromotions>();
        return Ok(result);
    }
}
