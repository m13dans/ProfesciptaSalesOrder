using Microsoft.AspNetCore.Mvc;
using ProfesciptaSalesOrder.Web.Services.SalesOrder;

namespace ProfesciptaSalesOrder.Web.Controllers.APIControllers;

[ApiController]
[Route("api/sales-orders")]
public class SalesOrderAPIController : ControllerBase
{
    [HttpGet("")]
    public async Task<ActionResult<GetSalesOrderListResponse>> GetTopSalesOrder(
        [FromQuery] GetSalesOrderListRequest request,
        [FromServices] GetSalesOrderListService salesOrderService)
    {
        var result = await salesOrderService.GetTopSalesOrder(request);
        if (!result.IsSuccess)
        {
            Problem(result.Error, statusCode: result.StatusCode);
        }

        return Ok(result.Value);
    }

    [HttpPost("")]
    public async Task<ActionResult<CreateSalesOrderResponse>> CreateSalesOrder(
        [FromBody] CreateSalesOrderRequest request,
        [FromServices] CreateSalesOrderService service
        )
    {
        var result = await service.CreateAsync(request);
        if (!result.IsSuccess)
        {
            Problem(result.Error, statusCode: result.StatusCode);
        }

        return Ok(result.Value);
    }
}
