using Microsoft.AspNetCore.Mvc;
using Tutorial9.Model;
using Tutorial9.Services;

namespace Tutorial9.Controllers;

[ApiController]
[Route("api/[controller]")]
public class WarehouseController : ControllerBase
{
    private readonly IDbService _dbService;

    public WarehouseController(IDbService dbService)
    {
        _dbService = dbService;
    }

    [HttpPost]
    public async Task<IActionResult> FulfillOrderAsync([FromBody] OrderFulfillmentDto dto)
    {
        var result = await _dbService.FulfillOrder(dto);
        return Ok(result);
    }
}