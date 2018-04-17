using System;
using System.Collections.Generic;
using System.Text;

using ActiveRecord.Interfaces;
using ActiveRecord.Services;
using ActiveRecord.Objects.Internal.Mapping;
using ActiveRecord.Objects.Internal;

namespace ActiveRecord.Objects.Internal
{
    public class Has<T> : Relationship<T> where T : class, IActiveRecord, new()
    {
        private String _keyColumnName = String.Empty;
        public String KeyColumnName
        {
            get 
            {
                string keyColumnName = String.Empty;
                
                if (_keyColumnName == String.Empty)
                {
                    keyColumnName = _parentObject.ActiveRecordMap.PrimaryKeyColumnMap.ColumnName;
                }
                else
                {
                    keyColumnName = _keyColumnName;
                }

                return keyColumnName;
            }
            
            set 
            {
                _keyColumnName = value;
            }            
        }       

        public Has()
        {        
        }
        
        public Has(String keyColumnName)
        {
            _keyColumnName = keyColumnName;

        }

        protected override void Retrieve()
        {
            String where;
            SqlParameter[] parameters;

            where = KeyColumnName + " = @" + KeyColumnName;

            parameters = new SqlParameter[]
                    {
                        new SqlParameter(KeyColumnName
                                                ,SqlBuilderServices.GetDBTypeFromSystemType(_parentObject.ActiveRecordMap.PrimaryKeyColumnMap.Property.PropertyType) 
                                                ,_parentObject.ActiveRecordMap.PrimaryKeyColumnMap.Property.GetValue(_parentObject, null))
                    };

            ObjectList = ActiveRecord<T>.FindManyWhere(where, parameters);
            _loadStatus = LoadStatuses.Loaded;
        }

        public override void Save()
        {
            String where;
            SqlParameter[] parameters;

            foreach (T obj in ObjectList)
            {
                //Set each child's foreign key to the parent's primary key
                if (obj.Version < _parentObject.Version)
                {
                    obj.ActiveRecordMap.ColumnMappings[KeyColumnName].Property.SetValue(obj, _parentObject.ActiveRecordMap.PrimaryKeyColumnMap.Property.GetValue(_parentObject, null), null);
                    obj.Save();
                }
            }
        }

        public override void Delete()
        {
            String where;
            SqlParameter[] parameters;

            foreach (T obj in ObjectList)
            {               
                obj.Delete();
            }
        }
    }
}
