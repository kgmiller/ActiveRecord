using System;
using System.Collections.Generic;
using System.Collections;
using System.Text;
using System.Reflection;
using System.Data;
using System.Security.Cryptography;

using ActiveRecord.Services;
using ActiveRecord.Providers;
using ActiveRecord.Interfaces;
using ActiveRecord.Objects.Internal.Mapping;

namespace ActiveRecord
{
    public class ActiveRecord<T> : IActiveRecord where T : class, IActiveRecord, new()
    {
        private static ActiveRecordSession _activeRecordSession = ActiveRecordSession.GetSession();

        private String _checksum;

        private ObjectStatus _objectStatus;
        public ObjectStatus ObjectStatus
        {
            get
            {
                DetermineObjectStatus();
                return _objectStatus;
            }
        }        

        private ActiveRecordMap _activeRecordMap;
        public ActiveRecordMap ActiveRecordMap
        {
            get { return _activeRecordMap; }
            set { _activeRecordMap = value; }
        }

        private int _version = 0;
        public int Version
        {
            get { return _version; }
            set { _version = value; }
        }

        public object Id
        {
            get
            {
                return ActiveRecordMap.PrimaryKeyColumnMap.Property.GetValue(this, null);
            }
        }

        public ActiveRecord()
        {
            Initialize();
        }

        private void Initialize()
        {
            if (!_activeRecordSession.ActiveRecordCache.ContainsKey(typeof(T)))
            {
                _activeRecordSession.ActiveRecordCache.Add(typeof(T), new Dictionary<object,object>());
            }
            
            if (_activeRecordMap == null)
            {
                _activeRecordMap = ActiveRecord<T>.GetMap();
            }

            InitializeRelationships();

            _objectStatus = ObjectStatus.New;

            if (_activeRecordMap.PrimaryKeyColumnMap != null)
            {
                _activeRecordMap.PrimaryKeyColumnMap.Property.SetValue(this, -1, null);
            }
        }

        private void InitializeRelationships()
        {
            foreach (BelongsToMap belongsToMap in _activeRecordMap.BelongsToMappings)
            {
                IRelationship relationship = (IRelationship)belongsToMap.Field.GetValue(this);
                relationship.InitializeParent(this);
            }

            foreach (HasOneMap hasOneMap in _activeRecordMap.HasOneMappings)
            {
                IRelationship relationship = (IRelationship)hasOneMap.Field.GetValue(this);
                relationship.InitializeParent(this);
            }

            foreach (HasManyMap hasManyMap in _activeRecordMap.HasManyMappings)
            {
                IRelationship relationship = (IRelationship)hasManyMap.Field.GetValue(this);
                relationship.InitializeParent(this);
            }

            foreach (HasAndBelongsToManyMap hasAndBelongsToManyMap in _activeRecordMap.HasAndBelongsToManyMappings)
            {
                IRelationship relationship = (IRelationship)hasAndBelongsToManyMap.Field.GetValue(this);
                relationship.InitializeParent(this);
            }
        
        }

        public void FinalizeRetrieval()
        {
            _checksum = GetCheckSum();
            _objectStatus = ObjectStatus.Retrieved;
        }

        private String GetCheckSum()
        {
            String valuesString = String.Empty;

            valuesString += _activeRecordMap.PrimaryKeyColumnMap.Property.GetValue(this, null).ToString();
            
            foreach (ColumnMap columnMap in _activeRecordMap.ColumnMappings.Values)
            {
                object value = columnMap.Property.GetValue(this, null);
                if (value != null)
                {
                    valuesString += value.ToString();
                }
            }

            byte[] input = Encoding.UTF8.GetBytes(valuesString);
            byte[] output = MD5.Create().ComputeHash(input);
            return Convert.ToBase64String(output);        
        }
        
        public static ActiveRecordMap GetMap()
        {
            return GetMap(typeof(T));
        }
        
        public static ActiveRecordMap GetMap(Type type)
        {
            ActiveRecordMap activeRecordMap;

            if (_activeRecordSession.ActiveRecordMapCache.ContainsKey(type))
            {
                activeRecordMap = _activeRecordSession.ActiveRecordMapCache[type];
            }
            else
            {
                activeRecordMap = new ActiveRecordMap(type);
                _activeRecordSession.ActiveRecordMapCache.Add(type, activeRecordMap);
            }
            
            return activeRecordMap;
        }

        public static T Create(IDictionary values)
        {
            T obj = new T();

            foreach (string propertyName in values.Keys)
            {
                if (obj.ActiveRecordMap.ColumnMappings.ContainsKey(propertyName))
                {
                    object valueObj = Convert.ChangeType(values[propertyName], obj.ActiveRecordMap.ColumnMappings[propertyName].Property.PropertyType);
                    obj.ActiveRecordMap.ColumnMappings[propertyName].Property.SetValue(obj, valueObj, null);
                }
            }

            obj.Save();
            
            return obj;
        }

        public static void Save(long Id, IDictionary values)
        {
            T obj = Find(Id);

            foreach (string propertyName in values.Keys)
            {
                if (obj.ActiveRecordMap.ColumnMappings.ContainsKey(propertyName))
                {
                    object valueObj = Convert.ChangeType(values[propertyName], obj.ActiveRecordMap.ColumnMappings[propertyName].Property.PropertyType);
                    obj.ActiveRecordMap.ColumnMappings[propertyName].Property.SetValue(obj, valueObj, null);
                }
            }

            obj.Save();
            obj.Refresh();

        }
        
        public void Save(IDictionary values)
        {
            foreach (string propertyName in values.Keys)
            {
                if (this.ActiveRecordMap.ColumnMappings.ContainsKey(propertyName))
                {
                    object valueObj = Convert.ChangeType(values[propertyName], this.ActiveRecordMap.ColumnMappings[propertyName].Property.PropertyType);
                    this.ActiveRecordMap.ColumnMappings[propertyName].Property.SetValue(this, valueObj, null);
                }
            }

            this.Save();
        }

        public void Save()
        {
            SaveBelongsGraph();

            DetermineObjectStatus();            
            
            if (_objectStatus == ObjectStatus.New)
            {
                Insert();
            }
            else if (_objectStatus == ObjectStatus.Changed)
            {
                Update();
            }
            
            Version++;

            SaveHasGraph();
        }

        private void SaveBelongsGraph()
        {
            foreach (BelongsToMap belongsToMap in _activeRecordMap.BelongsToMappings)
            {
                IRelationship relationship = (IRelationship)belongsToMap.Field.GetValue(this);
                if (relationship.SaveMethod != SaveMethods.ReadOnly)
                {
                    relationship.Save();
                }
            }
        }

        private void SaveHasGraph()
        {
            foreach (HasOneMap hasOneMap in _activeRecordMap.HasOneMappings)
            {
                IRelationship relationship = (IRelationship)hasOneMap.Field.GetValue(this);
                if (relationship.SaveMethod != SaveMethods.ReadOnly)
                {
                    relationship.Save();
                }
            }
            
            foreach (HasManyMap hasManyMap in _activeRecordMap.HasManyMappings)
            {
                IRelationship relationship = (IRelationship)hasManyMap.Field.GetValue(this);
                if (relationship.SaveMethod != SaveMethods.ReadOnly)
                {
                    relationship.Save();
                }
            }

            foreach (HasAndBelongsToManyMap hasAndBelongsToMany in _activeRecordMap.HasAndBelongsToManyMappings)
            {
                IRelationship relationship = (IRelationship)hasAndBelongsToMany.Field.GetValue(this);
                if (relationship.SaveMethod != SaveMethods.ReadOnly)
                {
                    relationship.Save();
                }
            }

        }

        public static void Delete(long id)
        {
            T obj = Find(id);
            obj.Delete();
        }
        
        private void DetermineObjectStatus()
        {
            if (_activeRecordMap.PrimaryKeyColumnMap != null)
            {
                if ((long)_activeRecordMap.PrimaryKeyColumnMap.Property.GetValue(this, null) == -1 && _objectStatus != ObjectStatus.Deleted)
                {
                    _objectStatus = ObjectStatus.New;
                }
                else if (_checksum != GetCheckSum() && _objectStatus != ObjectStatus.Deleted)
                {
                    _objectStatus = ObjectStatus.Changed;
                }
            }
            else
            {
                _objectStatus = ObjectStatus.New;
            }        
        }
                
        public void Insert()
        {
            string command = @"INSERT INTO " + _activeRecordMap.TableName
                            + "(" + SqlBuilderServices.BuildSqlColumnList(_activeRecordMap.ColumnList(false), _activeRecordMap.TableName, SqlBuilderServices.ColumnListType.List) + ")"
                           + " VALUES(" + SqlBuilderServices.BuildSqlColumnList(_activeRecordMap.ColumnList(false), _activeRecordMap.TableName, SqlBuilderServices.ColumnListType.ParameterizedList) + ")";

            SqlParameter[] parameters = new SqlParameter[_activeRecordMap.ColumnMappings.Count];
            
            //Set Column Parameters
            int parameterIndex = 0;
            foreach (ColumnMap columnMap in _activeRecordMap.ColumnMappings.Values)
            {
                SqlParameter parameter = new SqlParameter(columnMap.ColumnName 
                                                ,SqlBuilderServices.GetDBTypeFromSystemType(columnMap.Property.PropertyType) 
                                                ,columnMap.Property.GetValue(this, null));

                if (parameter.Type == DbType.DateTime && (DateTime)parameter.Value == DateTime.MinValue)
                {
                    parameter.Value = DateTime.Parse("1/1/1900");
                }                

                parameters[parameterIndex] = parameter;
                parameterIndex++;
            }
            //Execute Insert Command
            _activeRecordSession.DatabaseServices.ExecuteNonQuery(command, parameters);

            //Retrive Primary Key
            string query = @"SELECT MAX(" + _activeRecordMap.PrimaryKeyColumnName + ")"
                            + " FROM " + _activeRecordMap.TableName
                            + " WHERE " + SqlBuilderServices.BuildSqlColumnList(_activeRecordMap.ColumnList(false), _activeRecordMap.TableName, SqlBuilderServices.ColumnListType.WhereParameterizedList);

            _activeRecordMap.PrimaryKeyColumnMap.Property.SetValue(this, _activeRecordSession.DatabaseServices.ExecuteScalar(query, parameters), null);

        }

        public void Update()
        {

            string command = @"UPDATE " + _activeRecordMap.TableName
                            + " SET " + SqlBuilderServices.BuildSqlColumnList(_activeRecordMap.ColumnList(false), _activeRecordMap.TableName, SqlBuilderServices.ColumnListType.UpdateParameterizedList)
                            + " WHERE " + _activeRecordMap.PrimaryKeyColumnName + " = @" + _activeRecordMap.PrimaryKeyColumnName;

            SqlParameter[] parameters = new SqlParameter[_activeRecordMap.ColumnMappings.Count + 1];
            
            //Set Column Parameters
            int parameterIndex = 0;
            foreach (ColumnMap columnMap in _activeRecordMap.ColumnMappings.Values)
            {
                parameters[parameterIndex] = new SqlParameter(columnMap.ColumnName
                                                , SqlBuilderServices.GetDBTypeFromSystemType(columnMap.Property.PropertyType)
                                                , columnMap.Property.GetValue(this, null));

                if (parameters[parameterIndex].Type == DbType.DateTime && (DateTime)parameters[parameterIndex].Value == DateTime.MinValue)
                {
                    parameters[parameterIndex].Value = DateTime.Parse("1/1/1900");
                }

                parameterIndex++;
            }

            //Set Primary Key Parameter
            parameters[parameterIndex] = new SqlParameter(_activeRecordMap.PrimaryKeyColumnName
                                                , SqlBuilderServices.GetDBTypeFromSystemType(_activeRecordMap.PrimaryKeyColumnMap.Property.PropertyType)
                                                , _activeRecordMap.PrimaryKeyColumnMap.Property.GetValue(this, null));

            _activeRecordSession.DatabaseServices.ExecuteNonQuery(command, parameters);
        
        }

        public void Delete()
        {
            
            string command = @"DELETE FROM " + _activeRecordMap.TableName
                 + " WHERE " + _activeRecordMap.PrimaryKeyColumnName + " = @" + _activeRecordMap.PrimaryKeyColumnName;

            SqlParameter[] parameters = new SqlParameter[]
            {
                    new SqlParameter(_activeRecordMap.PrimaryKeyColumnName
                                                , SqlBuilderServices.GetDBTypeFromSystemType(_activeRecordMap.PrimaryKeyColumnMap.Property.PropertyType)
                                                , _activeRecordMap.PrimaryKeyColumnMap.Property.GetValue(this, null))

            };

            _activeRecordSession.DatabaseServices.ExecuteNonQuery(command, parameters);

            this._objectStatus = ObjectStatus.Deleted;
        
        }

        public void Refresh()
        {
            ActiveRecordMap activeRecordMap = ActiveRecord<T>.GetMap();

            string query = @"SELECT " + SqlBuilderServices.BuildSqlColumnList(activeRecordMap.ColumnList(true), activeRecordMap.TableName, SqlBuilderServices.ColumnListType.List)
                           + " FROM " + activeRecordMap.TableName
                           + " WHERE " + activeRecordMap.PrimaryKeyColumnName + " = @" + activeRecordMap.PrimaryKeyColumnName;


            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter(activeRecordMap.PrimaryKeyColumnName 
                                                ,SqlBuilderServices.GetDBTypeFromSystemType(activeRecordMap.PrimaryKeyColumnMap.Property.PropertyType) 
                                                ,this.Id)
            };

            IDataReader dataReader = _activeRecordSession.DatabaseServices.ExecuteDataReader(query, parameters);

            ObjectRetrievalServices.RetrieveObject<T>(dataReader, activeRecordMap, true);            
        }
        
        public static T Find(long id)
        {
            if (id == null)
            {
                return null;
            }
            
            ActiveRecordMap activeRecordMap = ActiveRecord<T>.GetMap();

            string query = @"SELECT " + SqlBuilderServices.BuildSqlColumnList(activeRecordMap.ColumnList(true), activeRecordMap.TableName, SqlBuilderServices.ColumnListType.List)
                           + " FROM " + activeRecordMap.TableName
                           + " WHERE " + activeRecordMap.PrimaryKeyColumnName + " = @" + activeRecordMap.PrimaryKeyColumnName;


            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter(activeRecordMap.PrimaryKeyColumnName 
                                                ,SqlBuilderServices.GetDBTypeFromSystemType(activeRecordMap.PrimaryKeyColumnMap.Property.PropertyType) 
                                                ,id)
            };

            IDataReader dataReader = _activeRecordSession.DatabaseServices.ExecuteDataReader(query, parameters);

            return ObjectRetrievalServices.RetrieveObject<T>(dataReader, activeRecordMap);
        }

        public static T FindFirstWhere(String where)
        {
            ActiveRecordMap activeRecordMap = ActiveRecord<T>.GetMap();

            string query = @"SELECT " + SqlBuilderServices.BuildSqlColumnList(activeRecordMap.ColumnList(true), activeRecordMap.TableName, SqlBuilderServices.ColumnListType.List)
                           + " FROM " + activeRecordMap.TableName
                           + " WHERE " + where;

            IDataReader dataReader = _activeRecordSession.DatabaseServices.ExecuteDataReader(query);
            return ObjectRetrievalServices.RetrieveObject<T>(dataReader, activeRecordMap);
        }

        public static T FindFirstWhere(String where, SqlParameter[] parameters)
        {
            ActiveRecordMap activeRecordMap = ActiveRecord<T>.GetMap();

            string query = @"SELECT " + SqlBuilderServices.BuildSqlColumnList(activeRecordMap.ColumnList(true), activeRecordMap.TableName, SqlBuilderServices.ColumnListType.List)
                           + " FROM " + activeRecordMap.TableName
                           + " WHERE " + where;

            IDataReader dataReader = _activeRecordSession.DatabaseServices.ExecuteDataReader(query, parameters);
            return ObjectRetrievalServices.RetrieveObject<T>(dataReader, activeRecordMap);
        }

        public static List<T> FindAll()
        {
            ActiveRecordMap activeRecordMap = ActiveRecord<T>.GetMap();

            string query = @"SELECT " + SqlBuilderServices.BuildSqlColumnList(activeRecordMap.ColumnList(true), activeRecordMap.TableName, SqlBuilderServices.ColumnListType.List)
                           + " FROM " + activeRecordMap.TableName;

            IDataReader dataReader = _activeRecordSession.DatabaseServices.ExecuteDataReader(query);
            return ObjectRetrievalServices.RetrieveObjects<T>(dataReader, activeRecordMap);
        }

        public static List<T> FindManyWhere(String where)
        {
            ActiveRecordMap activeRecordMap = ActiveRecord<T>.GetMap();

            string query = @"SELECT " + SqlBuilderServices.BuildSqlColumnList(activeRecordMap.ColumnList(true), activeRecordMap.TableName, SqlBuilderServices.ColumnListType.List)
                           + " FROM " + activeRecordMap.TableName
                           + " WHERE " + where;

            IDataReader dataReader = _activeRecordSession.DatabaseServices.ExecuteDataReader(query);
            return ObjectRetrievalServices.RetrieveObjects<T>(dataReader, activeRecordMap);
        }

        public static List<T> FindManyWhere(String where, SqlParameter[] parameters)
        {
            ActiveRecordMap activeRecordMap = ActiveRecord<T>.GetMap();

            string query = @"SELECT " + SqlBuilderServices.BuildSqlColumnList(activeRecordMap.ColumnList(true), activeRecordMap.TableName, SqlBuilderServices.ColumnListType.List)
                           + " FROM " + activeRecordMap.TableName
                           + " WHERE " + where;

            IDataReader dataReader = _activeRecordSession.DatabaseServices.ExecuteDataReader(query, parameters);
            return ObjectRetrievalServices.RetrieveObjects<T>(dataReader, activeRecordMap);
        }

        public static List<T> FindManyWithSql(String sqlQuery, SqlParameter[] parameters)
        {
            ActiveRecordMap activeRecordMap = ActiveRecord<T>.GetMap();

            IDataReader dataReader = _activeRecordSession.DatabaseServices.ExecuteDataReader(sqlQuery, parameters);
            return ObjectRetrievalServices.RetrieveObjects<T>(dataReader, activeRecordMap);
        }

        public static List<T> FindManyWithSql(String sqlQuery)
        {
            ActiveRecordMap activeRecordMap = ActiveRecord<T>.GetMap();

            IDataReader dataReader = _activeRecordSession.DatabaseServices.ExecuteDataReader(sqlQuery);
            return ObjectRetrievalServices.RetrieveObjects<T>(dataReader, activeRecordMap);
        }
   
    }
}
