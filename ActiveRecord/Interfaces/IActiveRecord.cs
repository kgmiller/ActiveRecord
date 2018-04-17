using System;
using System.Collections.Generic;
using System.Text;

using ActiveRecord.Objects.Internal.Mapping;

namespace ActiveRecord.Interfaces
{
    public interface IActiveRecord
    {
        ActiveRecordMap ActiveRecordMap
        {
            get;
            set;
        }

        int Version
        {
            get;
            set;
        }

        object Id
        {
            get;
        }

        ObjectStatus ObjectStatus
        {
            get;
        }

        void FinalizeRetrieval();

        void Save();
        void Delete();
        void Refresh();
    }
}
