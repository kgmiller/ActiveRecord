using System;
using System.Collections.Generic;
using System.Text;

using ActiveRecord.Interfaces;
using ActiveRecord.Services;
using ActiveRecord.Objects.Internal.Mapping;
using ActiveRecord.Objects.Internal;

namespace ActiveRecord
{
    public class BelongsTo<T> : Relationship<T> where T : class, IActiveRecord, new()
    {
        private String _parentKeyColumnName = String.Empty;
        public String ParentKeyColumnName
        {
            get 
            {
                string parentKeyColumnName = String.Empty;

                if (_parentKeyColumnName == String.Empty)
                {
                    parentKeyColumnName = ActiveRecord<T>.GetMap().PrimaryKeyColumnMap.ColumnName;
                }
                else
                {
                    parentKeyColumnName = _parentKeyColumnName;
                }
                
                return parentKeyColumnName; 
            }
            set 
            {
                _parentKeyColumnName = value; 
            }
        }

        private String _childKeyColumnName = String.Empty;
        public String ChildKeyColumnName
        {
            get
            {
                string childKeyColumnName = String.Empty;

                if (_childKeyColumnName == String.Empty)
                {
                    childKeyColumnName = ActiveRecord<T>.GetMap().PrimaryKeyColumnMap.ColumnName;
                }
                else
                {
                    childKeyColumnName = _childKeyColumnName;
                }
                
                return childKeyColumnName;
            }
            set
            {
                _childKeyColumnName = value;
            }
        }            

        public BelongsTo()
        {        
        }

        public BelongsTo(LoadMethods loadMethod)
        {
            LoadMethod = loadMethod;
        }
        
        public BelongsTo(SaveMethods saveMethod)
        {
            SaveMethod = saveMethod;
        }

        public BelongsTo(LoadMethods loadMethod, SaveMethods saveMethod)
        {
            LoadMethod = loadMethod;
            SaveMethod = saveMethod;
        }

        public BelongsTo(String parentKeyColumnName, String childKeyColumnName)
        {
            ParentKeyColumnName = parentKeyColumnName;
            ChildKeyColumnName = childKeyColumnName;
        }

        public BelongsTo(String parentKeyColumnName, String childKeyColumnName, LoadMethods loadMethod, SaveMethods saveMethod)
        {
            ParentKeyColumnName = parentKeyColumnName;
            ChildKeyColumnName = childKeyColumnName;
            LoadMethod = loadMethod;
            SaveMethod = saveMethod;
        }

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

        protected override void Retrieve()
        {            
            String where;
            SqlParameter[] parameters;

            where = ChildKeyColumnName + " = @" + ParentKeyColumnName;

            parameters = new SqlParameter[]
                    {
                        new SqlParameter(ParentKeyColumnName
                                                ,SqlBuilderServices.GetDBTypeFromSystemType(ActiveRecord<T>.GetMap().PrimaryKeyColumnMap.Property.PropertyType) 
                                                ,_parentObject.ActiveRecordMap.ColumnMappings[ParentKeyColumnName].Property.GetValue(_parentObject, null))
                    };

            ObjectList = ActiveRecord<T>.FindManyWhere(where, parameters);
            _loadStatus = LoadStatuses.Loaded;
        }

        public override void Save()
        {
            String where;
            SqlParameter[] parameters;

            //TODO: Change to foreach ala Has object
            if (ObjectList.Count > 0)
            {
                if (ObjectList[0] != null && ObjectList[0].Version <= _parentObject.Version)
                {
                    ObjectList[0].Save();
                }

                //Set the parent's foreign key value to the child's primary key value
                _parentObject.ActiveRecordMap.ColumnMappings[ParentKeyColumnName].Property.SetValue(_parentObject, ActiveRecord<T>.GetMap().PrimaryKeyColumnMap.Property.GetValue(ObjectList[0], null), null);

            }
        }
        
        public override void Delete()
        {
            throw new Exception("The method or operation is not implemented.");
        }

    }
}
