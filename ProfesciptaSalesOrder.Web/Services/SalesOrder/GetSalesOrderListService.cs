using Dapper;
using ProfesciptaSalesOrder.Web.Data;
using ProfesciptaSalesOrder.Web.Models;
using System.Text.Json.Serialization;

namespace ProfesciptaSalesOrder.Web.Services.SalesOrder;


public class GetSalesOrderListRequest
{
    public string? Keywords { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public DateTime? OrderDate { get; set; }
}

public class GetSalesOrderListResponse
{
    public int SalesOrderId { get; set; }
    public string OrderNo { get; set; }
    public DateTime OrderDate { get; set; }
    public string CustomerName { get; set; }
}


public class GetSalesOrderListService(ConnectionString connectionString)
{
    public async Task<Result<List<GetSalesOrderListResponse>>> GetTopSalesOrder(GetSalesOrderListRequest request)
    {
        var sql = "";

        if (string.IsNullOrEmpty(request.Keywords) && request.OrderDate == null)
        {
            sql = @"
                SELECT TOP 100 so.SO_ORDER_ID SalesOrderId, so.ORDER_NO OrderNo, so.ORDER_DATE OrderDate, cust.CUSTOMER_NAME CustomerName
                FROM SO_ORDER so
                JOIN COM_CUSTOMER cust ON so.COM_CUSTOMER_ID = cust.COM_CUSTOMER_ID
            ";

            try
            {
                using var conn = connectionString.Create();

                var queryResult = await conn.QueryAsync<GetSalesOrderListResponse>(sql);
                var result = queryResult.ToList();

                return Result<List<GetSalesOrderListResponse>>.Success(result);
            }
            catch (Exception ex)
            {
                return Result<List<GetSalesOrderListResponse>>.InternalServerError(ex.Message);
            }
        }

        sql = @"
            SELECT TOP 100 so.SO_ORDER_ID SalesOrderId, so.ORDER_NO OrderNo, so.ORDER_DATE OrderDate, cust.CUSTOMER_NAME CustomerName
            FROM SO_ORDER so
            JOIN COM_CUSTOMER cust ON so.COM_CUSTOMER_ID = cust.COM_CUSTOMER_ID
            WHERE (so.ORDER_NO LIKE '%' + @Keywords + '%'
            OR cust.CUSTOMER_NAME LIKE '%' + @Keywords + '%')
            AND (@OrderDate IS NULL OR so.ORDER_DATE = @OrderDate)
        ";

        try
        {
            using var conn = connectionString.Create();

            request.Keywords ??= "";
            var queryResult = await conn.QueryAsync<GetSalesOrderListResponse>(sql, new { request.Keywords, request.OrderDate });
            var result = queryResult.ToList();

            return Result<List<GetSalesOrderListResponse>>.Success(result);
        }
        catch (Exception ex)
        {
            return Result<List<GetSalesOrderListResponse>>.InternalServerError(ex.Message);
        }
    }
}

