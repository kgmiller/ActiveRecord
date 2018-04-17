using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;

namespace ActiveRecord.Objects.Internal.Mapping
{
    public class BelongsToMap
    {
        public FieldInfo Field;

        public BelongsToMap(FieldInfo field)
        {
            Field = field;
        }
    
    }
}
