using System;
using System.Collections.Generic;
using System.Text;

namespace ActiveRecord
{
    public class ColumnAttribute : Attribute
    {
        private String _columnName;
        public String ColumnName
        {
            get { return _columnName; }
            set { _columnName = value; }
        }

        public ColumnAttribute()
        {
        }

        public ColumnAttribute(String columnName)
        {
            ColumnName = columnName;
        }
    }

}
