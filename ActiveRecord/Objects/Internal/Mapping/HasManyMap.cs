using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;

namespace ActiveRecord.Objects.Internal.Mapping
{
    public class HasManyMap
    {
        public FieldInfo Field;

        public HasManyMap(FieldInfo field)
        {
            Field = field;
        }
    }
}
