using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace ActiveRecord
{
    public struct SqlParameter
    {
        public string Name;
        public DbType Type;
        public object Value;

        public SqlParameter(string name, DbType type, object value)
        {
            Name = name;
            Type = type;
            Value = value;
        }
    }
}
