using System;
using System.Collections.Generic;
using System.Text;

namespace ActiveRecord
{
    public class BaseRelationshipAttribute : Attribute
    {
        public enum LoadMethods
        {
            Active,
            Lazy
        }

        public LoadMethods LoadMethod = LoadMethods.Active;
    }
}
