using Microsoft.AspNetCore.Mvc;
using ProfesciptaSalesOrder.Web.Services;

namespace ProfesciptaSalesOrder.Web.Controllers.APIControllers;

[ApiController]
[Route("api/customers")]
public class CustomersController(GetCustomersService getCustomersService) : ControllerBase
{
    [HttpGet("")]
    public async Task<ActionResult<List<string>>> GetCustomers()
    {
        var result = await getCustomersService.GetCustomersAsync();
        if (!result.IsSuccess)
        {
            return Problem(result.Error, statusCode: result.StatusCode);
        }

        return Ok(result.Value.Customers);
    }


}
