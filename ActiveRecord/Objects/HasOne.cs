using System;
using System.Collections.Generic;
using System.Text;

using ActiveRecord.Interfaces;
using ActiveRecord.Services;
using ActiveRecord.Objects.Internal.Mapping;
using ActiveRecord.Objects.Internal;

namespace ActiveRecord
{
    public class HasOne<T> : Has<T> where T : class, IActiveRecord, new()
    {
        #region -Constructors-
        public HasOne()
        {        
        }

        public HasOne(LoadMethods loadMethod)
        {
            LoadMethod = loadMethod;
        }
        #endregion
        
        public T Object
        {
            get
            {
                EnsureLoaded();

                if (ObjectList.Count > 0)
                {
                    return ObjectList[0];
                }
                else
                {
                    return null;
                }
            }

            set
            {
                if (ObjectList.Count > 0)
                {
                    ObjectList[0] = value;
                }
                else
                {
                    ObjectList.Add(value);
                    _loadStatus = Relationship<T>.LoadStatuses.Loaded;
                    
                }
            }
        }
    }
}
