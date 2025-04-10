using Dapper;
using ProfesciptaSalesOrder.Web.Data;
using ProfesciptaSalesOrder.Web.Models;

namespace ProfesciptaSalesOrder.Web.Services.SalesOrder;

public class DeleteSalesOrderRequest
{
    public int SalesOrderId { get; set; }
}

public class DeleteSalesOrderResponse
{
    public int SalesOrderId { get; set; }
}

public class DeleteSalesOrderService(ConnectionString connectionString)
{
    public async Task<Result<DeleteSalesOrderResponse>> DeleteAsync(DeleteSalesOrderRequest request)
    {
        var deleteDetailSql = @"DELETE FROM SO_ITEM WHERE SO_ORDER_ID = @SalesOrderId";
        var deleteHeaderSql = @"DELETE FROM SO_ORDER WHERE SO_ORDER_ID = @SalesOrderId";

        try
        {
            using var conn = connectionString.Create();
            conn.Open();

            using var transaction = conn.BeginTransaction();

            var exists = await conn.ExecuteScalarAsync<int>(
                "SELECT COUNT(1) FROM SO_ORDER WHERE SO_ORDER_ID = @SalesOrderId",
                new { request.SalesOrderId },
                transaction
            );

            if (exists == 0)
            {
                return Result<DeleteSalesOrderResponse>.NotFound("Sales order tidak ditemukan.");
            }

            await conn.ExecuteAsync(deleteDetailSql, new { request.SalesOrderId }, transaction);
            await conn.ExecuteAsync(deleteHeaderSql, new { request.SalesOrderId }, transaction);

            transaction.Commit();

            return Result<DeleteSalesOrderResponse>.Success(new DeleteSalesOrderResponse
            {
                SalesOrderId = request.SalesOrderId
            });
        }
        catch (Exception ex)
        {
            return Result<DeleteSalesOrderResponse>.InternalServerError(ex.Message);
        }
    }
}
