using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;

namespace ActiveRecord.Objects.Internal.Mapping
{
    public class HasOneMap
    {
        public FieldInfo Field;

        public HasOneMap(FieldInfo field)
        {
            Field = field;
        }
    
    }
}
