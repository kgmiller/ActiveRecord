using System;
using System.Collections.Generic;
using System.Text;

namespace ActiveRecord
{
    public class PrimaryKeyColumnAttribute : Attribute
    {
        private String columnName;
        public String ColumnName
        {
            get { return columnName; }
            set { columnName = value; }
        }

        public PrimaryKeyColumnAttribute()
        {
        }

        public PrimaryKeyColumnAttribute(String columnName)
        {
            ColumnName = columnName;
        }
    }

}
