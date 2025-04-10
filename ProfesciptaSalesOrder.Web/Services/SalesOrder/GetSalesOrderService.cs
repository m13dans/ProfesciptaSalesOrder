using Dapper;
using ProfesciptaSalesOrder.Web.Data;
using ProfesciptaSalesOrder.Web.Models;

namespace ProfesciptaSalesOrder.Web.Services.SalesOrder;

public class GetSalesOrderRequest
{
    public int SalesOrderId { get; set; }
}

public class GetSalesOrderResponse
{
    public int SalesOrderId { get; set; }
    public required string SalesOrderNo { get; set; }
    public DateTime SalesOrderDate { get; set; }
    public int CustomerId { get; set; }
    public string CustomerName { get; set; }
    public string? Address { get; set; }

    public required List<DetailItems> DetailItems { get; set; }
}

public class GetSalesOrderService(ConnectionString connectionString)
{
    public async Task<Result<GetSalesOrderResponse>> GetSalesOrderAsync(GetSalesOrderRequest request)
    {
        var sql = @"
            SELECT 
                so.SO_ORDER_ID SalesOrderId,
                so.ORDER_NO SalesOrderNo,
                so.ORDER_DATE SalesOrderDate,
                so.COM_CUSTOMER_ID CustomerId,
                so.Address,
                cs.CUSTOMER_NAME CustomerName, 
                it.ITEM_NAME ItemName,
                it.Quantity,
                it.Price
            FROM SO_ORDER so
            LEFT JOIN SO_ITEM it ON so.SO_ORDER_ID = it.SO_ORDER_ID
            LEFT JOIN COM_CUSTOMER cs ON cs.COM_CUSTOMER_ID = so.COM_CUSTOMER_ID
            WHERE so.SO_ORDER_ID = @SalesOrderId
        ";

        try
        {
            using var conn = connectionString.Create();
            conn.Open();

            var result = await conn.QueryAsync<GetSalesOrderFlatRow>(sql, new { request.SalesOrderId });

            var grouped = result.GroupBy(x => new
            {
                x.SalesOrderId,
                x.SalesOrderNo,
                x.SalesOrderDate,
                x.CustomerId,
                x.CustomerName,
                x.Address
            })
            .Select(g => new GetSalesOrderResponse
            {
                SalesOrderId = g.Key.SalesOrderId,
                SalesOrderNo = g.Key.SalesOrderNo,
                SalesOrderDate = g.Key.SalesOrderDate,
                CustomerId = g.Key.CustomerId,
                CustomerName = g.Key.CustomerName,
                Address = g.Key.Address,
                DetailItems = g.Select(x => new DetailItems
                {
                    ItemName = x.ItemName,
                    Quantity = x.Quantity,
                    Price = x.Price
                }).ToList()
            })
            .FirstOrDefault();

            if (grouped is null)
                return Result<GetSalesOrderResponse>.NotFound("Sales order tidak ditemukan.");

            return Result<GetSalesOrderResponse>.Success(grouped);
        }
        catch (Exception ex)
        {
            return Result<GetSalesOrderResponse>.InternalServerError(ex.Message);
        }
    }

    private class GetSalesOrderFlatRow
    {
        public int SalesOrderId { get; set; }
        public string SalesOrderNo { get; set; } = string.Empty;
        public DateTime SalesOrderDate { get; set; }
        public int CustomerId { get; set; }
        public string CustomerName { get; set; } = string.Empty;
        public string? Address { get; set; }
        public string ItemName { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public double Price { get; set; }
    }
}
