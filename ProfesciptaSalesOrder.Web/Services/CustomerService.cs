using Dapper;
using ProfesciptaSalesOrder.Web.Data;
using ProfesciptaSalesOrder.Web.Models;

namespace ProfesciptaSalesOrder.Web.Services;

public class GetCustomerResponse
{
    public List<Customers> Customers { get; set; }
}

public class Customers
{
    public int CustomerId { get; set; }
    public string CustomerName { get; set; }
}

public class GetCustomersService(ConnectionString connectionString)
{
    public async Task<Result<GetCustomerResponse>> GetCustomersAsync()
    {
        var sql = @"SELECT CUSTOMER_NAME CustomerName, COM_CUSTOMER_ID CustomerId FROM COM_CUSTOMER";

        try
        {
            using var conn = connectionString.Create();

            var queryResult = await conn.QueryAsync<Customers>(sql);
            var result = queryResult.ToList();

            var response = new GetCustomerResponse() { Customers = result };

            return Result<GetCustomerResponse>.Success(response);
        }
        catch (Exception ex)
        {
            return Result<GetCustomerResponse>.InternalServerError(ex.Message);
        }
    }
}

