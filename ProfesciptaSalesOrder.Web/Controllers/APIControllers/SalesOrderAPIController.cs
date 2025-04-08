using Microsoft.AspNetCore.Mvc;
using ProfesciptaSalesOrder.Web.Services;

namespace ProfesciptaSalesOrder.Web.Controllers.APIControllers;

[ApiController]
[Route("api/sales-orders")]
public class SalesOrderAPIController(SalesOrderService salesOrderService) : ControllerBase
{
    [HttpGet("")]
    public async Task<ActionResult<SalesOrderResponse>> GetTopSalesOrder([FromQuery] SalesOrderRequest request)
    {
        var result = await salesOrderService.GetTopSalesOrder(request);
        if (!result.IsSuccess)
        {
            Problem(result.Error, statusCode: result.StatusCode);
        }

        return Ok(result.Value);
    }
}
