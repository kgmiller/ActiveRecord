using System;
using System.Data.Common;
using System.Collections.Generic;
using System.Text;

namespace ActiveRecord.Interfaces
{
    public interface IDbProvider
    {
        DbConnection CreateConnection();
        DataAdapter CreateDataAdapter(DbCommand selectCommand);
    }
}
