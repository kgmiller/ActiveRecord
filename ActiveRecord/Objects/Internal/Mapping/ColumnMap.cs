using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Reflection;

namespace ActiveRecord.Objects.Internal.Mapping
{
    public class ColumnMap
    {
        public PropertyInfo Property;
        
        public ColumnAttribute Attribute;

        public ColumnMap(PropertyInfo property, ColumnAttribute attribute)
        {
            Property = property;
            Attribute = attribute;
        }

        public string ColumnName
        {
            get
            {
                if (Attribute.ColumnName != null)
                {
                    return Attribute.ColumnName;
                }
                else
                {
                    return Property.Name;
                }
            }
        }
    }
}
