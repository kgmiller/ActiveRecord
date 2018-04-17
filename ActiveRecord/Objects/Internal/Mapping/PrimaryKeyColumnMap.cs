using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;

namespace ActiveRecord.Objects.Internal.Mapping
{
    public class PrimaryKeyColumnMap
    {
        public PropertyInfo Property;

        public PrimaryKeyColumnAttribute Attribute;

        public PrimaryKeyColumnMap(PropertyInfo property, PrimaryKeyColumnAttribute attribute)
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
