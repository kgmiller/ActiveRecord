using System;
using System.Collections.Generic;
using System.Text;

using ActiveRecord.Interfaces;
using ActiveRecord.Services;
using ActiveRecord.Objects.Internal.Mapping;
using ActiveRecord.Objects.Internal;

namespace ActiveRecord
{
    public class HasAndBelongsToMany<T> : Relationship<T> where T : class, IActiveRecord, new()
    {
        #region -Private Members-
        private List<object> _retrivedObjects = new List<object>();
        #endregion

        #region -Properties-
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
        
        private String _parentKeyColumnName = String.Empty;
        public String ParentKeyColumnName
        {
            get 
            {
                string parentKeyColumnName = String.Empty;
                if (_parentKeyColumnName == String.Empty)
                {
                    parentKeyColumnName = _parentObject.ActiveRecordMap.PrimaryKeyColumnMap.ColumnName;
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
        
        private String _relationshipTableName = String.Empty;
        public String RelationshipTableName
        {
            get 
            {
                string relationshipTableName = String.Empty;
                if (_relationshipTableName == String.Empty)
                {
                    if (String.Compare(ActiveRecord<T>.GetMap().TableName,_parentObject.ActiveRecordMap.TableName) < 0)
                    {
                        relationshipTableName = ActiveRecord<T>.GetMap().TableName + "To" + _parentObject.ActiveRecordMap.TableName;
                    }
                    else
                    {
                        relationshipTableName = _parentObject.ActiveRecordMap.TableName + "To" + ActiveRecord<T>.GetMap().TableName;                    
                    }                    
                }
                else
                {
                    relationshipTableName = _relationshipTableName;
                }
                
                return relationshipTableName;
            }
            
            set 
            {
                _relationshipTableName = value;
            }
        }
        #endregion

        #region -Constructors-
        public HasAndBelongsToMany()
        {        
        }
        
        public HasAndBelongsToMany(LoadMethods loadMethod)
        {
            LoadMethod = loadMethod;
        }
        
        public HasAndBelongsToMany(String relationshipTableName)
        {
            RelationshipTableName = relationshipTableName;
        }

        public HasAndBelongsToMany(SaveMethods saveMethod)
        {
            SaveMethod = saveMethod;
        }

        public HasAndBelongsToMany(LoadMethods loadMethod, SaveMethods saveMethod)
        {
            LoadMethod = loadMethod;
            SaveMethod = saveMethod;
        }

        public HasAndBelongsToMany(String relationshipTableName, LoadMethods loadMethod)
        {
            RelationshipTableName = relationshipTableName;
            LoadMethod = loadMethod;
        }

        public HasAndBelongsToMany(String relationshipTableName, LoadMethods loadMethod, SaveMethods saveMethod)
        {
            RelationshipTableName = relationshipTableName;
            LoadMethod = loadMethod;
            SaveMethod = saveMethod;
        }

        public HasAndBelongsToMany(String relationshipTableName, String parentKeyColumnName, String childKeyColumnName)
        {
            RelationshipTableName = relationshipTableName;
            ParentKeyColumnName = parentKeyColumnName;
            ChildKeyColumnName = childKeyColumnName;
        }

        #endregion

        #region -Protected Methods-
        protected override void Retrieve()
        {
            String sqlQuery;
            SqlParameter[] parameters;

            sqlQuery = @"SELECT " + SqlBuilderServices.BuildSqlColumnList(ActiveRecord<T>.GetMap().ColumnList(true), ActiveRecord<T>.GetMap().TableName, SqlBuilderServices.ColumnListType.List)
                           + " FROM " + ActiveRecord<T>.GetMap().TableName
                           + " INNER JOIN " + RelationshipTableName + " ON " + RelationshipTableName + "." + ChildKeyColumnName + " = " + ActiveRecord<T>.GetMap().TableName + "." + ChildKeyColumnName
                           + " WHERE " + ParentKeyColumnName + " = @" + ParentKeyColumnName;

            parameters = new SqlParameter[]
                    {
                        new SqlParameter(ParentKeyColumnName
                                                ,SqlBuilderServices.GetDBTypeFromSystemType(_parentObject.ActiveRecordMap.PrimaryKeyColumnMap.Property.PropertyType) 
                                                ,_parentObject.ActiveRecordMap.PrimaryKeyColumnMap.Property.GetValue(_parentObject, null))
                    };

            ObjectList = ActiveRecord<T>.FindManyWithSql(sqlQuery, parameters);

            foreach (T obj in ObjectList)
            {
                _retrivedObjects.Add(obj.Id);
            }

            _loadStatus = LoadStatuses.Loaded;
        }
        #endregion

        #region -Public Methods-
        public override void Save()
        {
            String where;
            SqlParameter[] parameters;

            foreach (T obj in ObjectList)
            {
                //Set each child's foreign key to the parent's primary key
                if (obj.Version < _parentObject.Version)
                {                    
                    obj.Save();
                }

                //Insert relationship row for newly added/created child objects
                if (!_retrivedObjects.Contains(obj.Id))
                {
                    string command = @"INSERT INTO " + RelationshipTableName
                        + "(" + ParentKeyColumnName + "," + ChildKeyColumnName + ")"
                        + " VALUES(@" + ParentKeyColumnName + ", @" + ChildKeyColumnName + ")";

                    parameters = new SqlParameter[]
                    {
                        new SqlParameter(ParentKeyColumnName
                                        , SqlBuilderServices.GetDBTypeFromSystemType(_parentObject.ActiveRecordMap.PrimaryKeyColumnMap.Property.PropertyType)
                                        , _parentObject.Id),

                        new SqlParameter(ChildKeyColumnName
                            , SqlBuilderServices.GetDBTypeFromSystemType(ActiveRecord<T>.GetMap().PrimaryKeyColumnMap.Property.PropertyType)
                            , obj.Id)

                    };

                    //Execute Insert Command
                    ActiveRecordSession.GetSession().DatabaseServices.ExecuteNonQuery(command, parameters);

                    _retrivedObjects.Add(obj.Id);
                }
            }
        }

        public override void Delete()
        {            
            throw new Exception("The method or operation is not implemented.");
        }

        public void SetToIds(List<string> ids)
        {
            List<long> idsList = new List<long>();

            foreach (String id in ids)
            {
                idsList.Add(long.Parse(id));
            }

            SetToIds(idsList);
        }
        
        public void SetToIds(List<long> ids)
        {
            List<T> newObjectList = new List<T>();
            List<T> objectsToRemoveList = new List<T>();

            foreach (long id in ids)
            {
                newObjectList.Add(ActiveRecord<T>.Find(id));
            }

            foreach (T obj in ObjectList)
            {
                if (!newObjectList.Contains(obj))
                {
                    objectsToRemoveList.Add(obj);
                }                    
            }

            foreach (T obj in objectsToRemoveList)
            {
                Remove(obj);
            }
            
            foreach (T obj in newObjectList)
            {
                if (!ObjectList.Contains(obj))
                {
                    ObjectList.Add(obj);
                }
            }
        }
        
        public void Remove(T obj)
        {
            String sqlCommand;
            SqlParameter[] parameters;

            sqlCommand = @"DELETE FROM " + RelationshipTableName
                        + " WHERE " + ChildKeyColumnName + " = @" + ChildKeyColumnName
                        + " AND " + ParentKeyColumnName + " = @" + ParentKeyColumnName;

            parameters = new SqlParameter[]
                    {
                        new SqlParameter(ParentKeyColumnName
                                        , SqlBuilderServices.GetDBTypeFromSystemType(_parentObject.ActiveRecordMap.PrimaryKeyColumnMap.Property.PropertyType)
                                        , _parentObject.Id),

                        new SqlParameter(ChildKeyColumnName
                            , SqlBuilderServices.GetDBTypeFromSystemType(ActiveRecord<T>.GetMap().PrimaryKeyColumnMap.Property.PropertyType)
                            , obj.Id)

                    };

            //Execute Delete Command
            ActiveRecordSession.GetSession().DatabaseServices.ExecuteNonQuery(sqlCommand, parameters);

            ObjectList.Remove(obj);
            _retrivedObjects.Remove(obj.Id);

        }

        #endregion
    }
}
