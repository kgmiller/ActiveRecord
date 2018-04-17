using System;
using System.Collections.Generic;
using System.Text;

namespace ActiveRecord.Interfaces
{
    public interface IRelationship
    {
        SaveMethods SaveMethod
        {
            get;
            set;
        }
        void Load();
        void Unload();
        void Save();
        void InitializeParent(IActiveRecord parentObject);
    }
}
