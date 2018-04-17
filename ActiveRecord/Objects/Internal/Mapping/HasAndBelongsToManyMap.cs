using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;

namespace ActiveRecord.Objects.Internal.Mapping
{
    public class HasAndBelongsToManyMap
    {
        public FieldInfo Field;

        public HasAndBelongsToManyMap(FieldInfo field)
        {
            Field = field;
        }
    
    }
}
