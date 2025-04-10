using Dapper;
using ProfesciptaSalesOrder.Web.Data;
using ProfesciptaSalesOrder.Web.Models;

namespace ProfesciptaSalesOrder.Web.Services;

public class GetCustomerResponse
{
    public List<string> Customers { get; set; }
}


public class GetCustomersService(ConnectionString connectionString)
{
    public async Task<Result<GetCustomerResponse>> GetCustomersAsync()
    {
        var sql = @"SELECT CUSTOMER_NAME Customers FROM COM_CUSTOMER";

        try
        {
            using var conn = connectionString.Create();

            var queryResult = await conn.QueryAsync<string>(sql);
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

