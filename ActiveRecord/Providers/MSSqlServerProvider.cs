using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Configuration;
using System.Web.Configuration;

using ActiveRecord.Interfaces;

namespace ActiveRecord.Providers
{
    class MSSqlServerProvider : IDbProvider
    {
        public DbConnection CreateConnection()
        {
            String connectionString = String.Empty;

            connectionString = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;

            return (DbConnection) new SqlConnection(connectionString);
        }

        public DataAdapter CreateDataAdapter(DbCommand selectCommand)
        {
            return (DataAdapter) new SqlDataAdapter();
        }
    }
}
