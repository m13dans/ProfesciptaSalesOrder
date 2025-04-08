using Microsoft.Data.SqlClient;
using System.Data;

namespace ProfesciptaSalesOrder.Web.Data;

public class ConnectionString(IConfiguration config)
{
    public IDbConnection Create() => new SqlConnection(config.GetConnectionString("SqlExpress"));
}
