using Dapper;
using ProfesciptaSalesOrder.Web.Data;
using ProfesciptaSalesOrder.Web.Models;

namespace ProfesciptaSalesOrder.Web.Services.SalesOrder;

public class CreateSalesOrderRequest
{
    public required string SalesOrderNo { get; set; }
    public DateTime SalesOrderDate { get; set; }
    public int CustomerId { get; set; }
    public string? Address { get; set; }

    public required List<DetailItems> DetailItems { get; set; }

}
public class DetailItems
{
    public required string ItemName { get; set; }
    public int Quantity { get; set; }
    public double Price { get; set; }
    public int SalesOrderId { get; set; }
}

public class CreateSalesOrderResponse
{
    public int SalesOrderId { get; set; }
}

public class CreateSalesOrderService(ConnectionString connectionString)
{
    public async Task<Result<CreateSalesOrderResponse>> CreateAsync(CreateSalesOrderRequest request)
    {
        var sql = @"
            INSERT INTO SO_ORDER (
                ORDER_NO,
                ORDER_DATE,
                COM_CUSTOMER_ID,
                ADDRESS
            ) OUTPUT INSERTED.SO_ORDER_ID VALUES (
                @SalesOrderNo,
                @SalesOrderDate,
                @CustomerId,
                @Address
            )
        ";

        var detailSql = @"
            INSERT INTO SO_ITEM (
                SO_ORDER_ID,
                ITEM_NAME,
                QUANTITY,
                PRICE
            ) VALUES (
                @SalesOrderId,
                @ItemName,
                @Quantity,
                @Price
            )
        ";

        try
        {
            if (request.DetailItems.Count <= 0)
            {
                return Result<CreateSalesOrderResponse>.Failure($"Must Have at least one item");
            }
            using var conn = connectionString.Create();
            conn.Open();

            var salesOrderNo = await conn.QueryFirstOrDefaultAsync<string>("SELECT ORDER_NO FROM SO_ORDER WHERE ORDER_NO = @SalesOrderNo", new { request.SalesOrderNo });
            if (!string.IsNullOrEmpty(salesOrderNo))
            {
                return Result<CreateSalesOrderResponse>.Failure($"Sales Order with Sales Order No : {request.SalesOrderNo} Already Exists");
            }

            using var transaction = conn.BeginTransaction();

            var salesOrderId = await conn.ExecuteScalarAsync<int>(sql, new
            {
                request.SalesOrderNo,
                request.SalesOrderDate,
                request.CustomerId,
                request.Address
            }, transaction: transaction);

            var detailParameters = request.DetailItems.Select(x => new
            {
                SalesOrderId = salesOrderId,
                x.ItemName,
                x.Quantity,
                x.Price
            }).ToList();

            var detailInsertResult = await conn.ExecuteAsync(detailSql, detailParameters, transaction: transaction);

            transaction.Commit();

            return Result<CreateSalesOrderResponse>.Success(
                new CreateSalesOrderResponse
                {
                    SalesOrderId = salesOrderId
                });
        }
        catch (Exception ex)
        {
            return Result<CreateSalesOrderResponse>.InternalServerError(ex.Message);
        }
    }
}
