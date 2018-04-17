using System;
using System.Collections.Generic;
using System.Text;

namespace ActiveRecord.Objects.Internal.Mapping
{
    public class TableMap
    {
        public Type Type;

        public TableAttribute TableAttribute;

        public TableMap(Type type, TableAttribute attribute)
        {
            Type = type;
            TableAttribute = attribute;
        }

        public string TableName
        {
            get
            {
                if (TableAttribute.TableName != null)
                {
                    return TableAttribute.TableName;
                }
                else
                {
                    return Type.Name + "s";
                }
            }
        }
    }
}
