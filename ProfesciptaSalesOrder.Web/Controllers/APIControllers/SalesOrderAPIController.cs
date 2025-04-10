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
            return Problem(result.Error, statusCode: result.StatusCode);
        }

        return Ok(result.Value);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<GetSalesOrderResponse>> GetSalesOrder(
        int id,
        [FromServices] GetSalesOrderService service)
    {
        var result = await service.GetSalesOrderAsync(new GetSalesOrderRequest() { SalesOrderId = id});
        if (!result.IsSuccess)
        {
            return Problem(result.Error, statusCode: result.StatusCode);
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
            return Problem(result.Error, statusCode: result.StatusCode);
        }

        return Ok(result.Value);
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<UpdateSalesOrderResponse>> CreateSalesOrder(
        [FromBody] UpdateSalesOrderRequest request,
        [FromServices] UpdateSalesOrderService service
        )
    {
        var result = await service.UpdateAsync(request);
        if (!result.IsSuccess)
        {
            return Problem(result.Error, statusCode: result.StatusCode);
        }

        return Ok(result.Value);
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id, [FromServices] DeleteSalesOrderService service)
    {
        var result = await service.DeleteAsync(new DeleteSalesOrderRequest { SalesOrderId = id });
        if (!result.IsSuccess)
            return Problem(result.Error, statusCode: result.StatusCode);

        return Ok(result.Value);
    }
}
