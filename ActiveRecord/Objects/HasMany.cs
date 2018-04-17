using System;
using System.Collections.Generic;
using System.Text;

using ActiveRecord.Interfaces;
using ActiveRecord.Services;
using ActiveRecord.Objects.Internal.Mapping;
using ActiveRecord.Objects.Internal;

namespace ActiveRecord
{
    public class HasMany<T> : Has<T> where T : class, IActiveRecord, new()
    {
        public List<T> Object
        {
            get
            {
                EnsureLoaded();
                return ObjectList;
            }

            set
            {
                ObjectList = value;
            }
        }
    }
}
