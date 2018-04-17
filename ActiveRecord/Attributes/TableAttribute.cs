using System;
using System.Collections.Generic;
using System.Text;

namespace ActiveRecord
{
    public class TableAttribute : Attribute
    {
        private String _tableName;

        public String TableName
        {
            get { return _tableName; }
            set { _tableName = value; }
        }

        public TableAttribute()
        {
        }

        public TableAttribute(String tableName)
        {
            TableName = tableName;
        }
    }

}
