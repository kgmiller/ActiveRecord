using System;
using System.Collections.Generic;
using System.Text;

using ActiveRecord.Interfaces;
using ActiveRecord.Services;
using ActiveRecord.Providers;
using ActiveRecord.Objects.Internal.Mapping;

namespace ActiveRecord
{
    public class ActiveRecordSession
    {
        private static ActiveRecordSession _instance;
        private static Object _ActiveRecordSessionLock = typeof(ActiveRecordSession);
        
        private IDbProvider _dbProvider;
        internal IDbProvider DbProvider
        {
            get { return _dbProvider; }
            set { _dbProvider = value; }
        }

        private DatabaseServices _databaseServices;
        public DatabaseServices DatabaseServices
        {
            get { return _databaseServices; }
            set { _databaseServices = value; }
        }

        private Dictionary<Type, ActiveRecordMap> _activeRecordMapCache;
        public Dictionary<Type, ActiveRecordMap> ActiveRecordMapCache
        {
            get { return _activeRecordMapCache; }
            set { _activeRecordMapCache = value; }
        }

        private Dictionary<Type, Dictionary<object, object>> _activeRecordCache;
        public Dictionary<Type, Dictionary<object, object>> ActiveRecordCache
        {
            get { return _activeRecordCache; }
            set { _activeRecordCache = value; }
        }


        private ActiveRecordSession(IDbProvider dbProvider)
        {
            DbProvider = dbProvider;
            DatabaseServices = new DatabaseServices(DbProvider);
            ActiveRecordMapCache = new Dictionary<Type, ActiveRecordMap>();
            ActiveRecordCache = new Dictionary<Type, Dictionary<object, object>>();           
        }

        public static ActiveRecordSession GetSession()
        {
            if (_instance == null)
            {
                CreateSession();
            }

            return _instance;
        }

        public static void CreateSession(IDbProvider dbProvider)
        {
            lock (_ActiveRecordSessionLock)
            {
                if (_instance == null)
                {
                    _instance = new ActiveRecordSession(dbProvider);
                }
            }
        }

        public static void CreateSession()
        {
            CreateSession(new MSSqlServerProvider());                      
        }    
    }
}
