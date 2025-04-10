using Dapper;
using ProfesciptaSalesOrder.Web.Data;
using ProfesciptaSalesOrder.Web.Models;

namespace ProfesciptaSalesOrder.Web.Services.SalesOrder;

public class UpdateSalesOrderRequest
{
    public int SalesOrderId { get; set; } 
    public required string SalesOrderNo { get; set; }
    public DateTime SalesOrderDate { get; set; }
    public int CustomerId { get; set; }
    public string? Address { get; set; }
    public required List<DetailItems> DetailItems { get; set; }
}

public class UpdateSalesOrderResponse
{
    public int SalesOrderId { get; set; }
}

public class UpdateSalesOrderService(ConnectionString connectionString)
{
    public async Task<Result<UpdateSalesOrderResponse>> UpdateAsync(UpdateSalesOrderRequest request)
    {
        var updateHeaderSql = @"
            UPDATE SO_ORDER
            SET
                ORDER_NO = @SalesOrderNo,
                ORDER_DATE = @SalesOrderDate,
                COM_CUSTOMER_ID = @CustomerId,
                ADDRESS = @Address
            WHERE SO_ORDER_ID = @SalesOrderId
        ";

        var deleteDetailSql = @"DELETE FROM SO_ITEM WHERE SO_ORDER_ID = @SalesOrderId";

        var insertDetailSql = @"
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
            if (request.DetailItems.Count == 0)
            {
                return Result<UpdateSalesOrderResponse>.Failure("At least one item is required.");
            }

            using var conn = connectionString.Create();
            conn.Open();

            using var transaction = conn.BeginTransaction();

            await conn.ExecuteAsync(updateHeaderSql, new
            {
                request.SalesOrderNo,
                request.SalesOrderDate,
                request.CustomerId,
                request.Address,
                request.SalesOrderId
            }, transaction);

            await conn.ExecuteAsync(deleteDetailSql, new { request.SalesOrderId }, transaction);

            var detailParams = request.DetailItems.Select(x => new
            {
                SalesOrderId = request.SalesOrderId,
                x.ItemName,
                x.Quantity,
                x.Price
            });

            await conn.ExecuteAsync(insertDetailSql, detailParams, transaction);

            transaction.Commit();

            return Result<UpdateSalesOrderResponse>.Success(new UpdateSalesOrderResponse
            {
                SalesOrderId = request.SalesOrderId
            });
        }
        catch (Exception ex)
        {
            return Result<UpdateSalesOrderResponse>.InternalServerError(ex.Message);
        }
    }
}
