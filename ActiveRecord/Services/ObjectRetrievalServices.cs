using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Reflection;

using ActiveRecord.Interfaces;
using ActiveRecord.Objects.Internal.Mapping;

namespace ActiveRecord.Services
{
    class ObjectRetrievalServices
    {
        public static T RetrieveObject<T>(IDataReader dataReader, ActiveRecordMap activeRecordMap) where T : class, IActiveRecord, new()
        {
            return RetrieveObject<T>(dataReader, activeRecordMap, false);
        }

        public static T RetrieveObject<T>(IDataReader dataReader, ActiveRecordMap activeRecordMap, bool refresh) where T : class, IActiveRecord, new()
        {
            List<T> objectList = RetrieveObjects<T>(dataReader, activeRecordMap, 1, refresh);
            if (objectList.Count > 0)
            {
                return objectList[0];
            }
            else
            {
                return null;
            }
        }

        public static List<T> RetrieveObjects<T>(IDataReader dataReader, ActiveRecordMap activeRecordMap) where T : class, IActiveRecord, new()
        {
            List<T> objectList = RetrieveObjects<T>(dataReader, activeRecordMap, int.MaxValue, false);
            return objectList;
        }

        public static List<T> RetrieveObjects<T>(IDataReader dataReader, ActiveRecordMap activeRecordMap, bool refresh) where T : class, IActiveRecord, new()
        {
            List<T> objectList = RetrieveObjects<T>(dataReader, activeRecordMap, int.MaxValue, refresh);
            return objectList;
        }

        public static List<T> RetrieveObjects<T>(IDataReader dataReader, ActiveRecordMap activeRecordMap, int maxListCount, bool refresh) where T : class, IActiveRecord, new()
        {            

            List<T> objectList = new List<T>();

            while (dataReader.Read() && objectList.Count <= maxListCount)
            {
                T obj = new T();

                //Map primary key column property value
                Type keyPropertyType = activeRecordMap.PrimaryKeyColumnMap.Property.PropertyType;
                String keyColumnName = activeRecordMap.PrimaryKeyColumnName;
                object keyValue = Convert.ChangeType(dataReader[keyColumnName], keyPropertyType);

                if (ActiveRecordSession.GetSession().ActiveRecordCache[typeof(T)].ContainsKey(keyValue) && ! refresh)
                {
                    obj = ActiveRecordSession.GetSession().ActiveRecordCache[typeof(T)][keyValue] as T;
                }
                else
                {
                    if (refresh && ActiveRecordSession.GetSession().ActiveRecordCache[typeof(T)].ContainsKey(keyValue))
                    {
                        obj = ActiveRecordSession.GetSession().ActiveRecordCache[typeof(T)][keyValue] as T;
                        UnloadGraph<T>(obj, activeRecordMap);   
                    }
                    
                    activeRecordMap.PrimaryKeyColumnMap.Property.SetValue(obj, keyValue, null);

                    //Set column property values
                    foreach (ColumnMap columnMap in activeRecordMap.ColumnMappings.Values)
                    {
                        Type propertyType = columnMap.Property.PropertyType;
                        String columnName = columnMap.ColumnName;
                        //TODO: Implement better null handleing
                        if (dataReader[columnName] != DBNull.Value)
                        {
                            object columnValue = Convert.ChangeType(dataReader[columnName], propertyType);
                            columnMap.Property.SetValue(obj, columnValue, null);
                        }
                    }

                    obj.FinalizeRetrieval();

                    if (!refresh)
                    {
                        ActiveRecordSession.GetSession().ActiveRecordCache[typeof(T)].Add(keyValue, obj);
                    }

                    LoadGraph<T>(obj, activeRecordMap);                    
                }
                
                objectList.Add(obj);
            }

            dataReader.Close();

            return objectList;
        }

        private static void LoadGraph<T>(T obj, ActiveRecordMap activeRecordMap) where T : class, IActiveRecord, new()
        {
            foreach (BelongsToMap belongsToMap in activeRecordMap.BelongsToMappings)
            {
                IRelationship relationship = (IRelationship)belongsToMap.Field.GetValue(obj);

                relationship.Load();
            }
            
            foreach (HasOneMap hasOneMap in activeRecordMap.HasOneMappings)
            {
                IRelationship relationship = (IRelationship)hasOneMap.Field.GetValue(obj);

                relationship.Load();
            }

            foreach (HasManyMap hasManyMap in activeRecordMap.HasManyMappings)
            {
                IRelationship relationship = (IRelationship)hasManyMap.Field.GetValue(obj);

                relationship.Load();
            }

            foreach (HasAndBelongsToManyMap hasAndBelongsToManyMap in activeRecordMap.HasAndBelongsToManyMappings)
            {
                IRelationship relationship = (IRelationship)hasAndBelongsToManyMap.Field.GetValue(obj);

                relationship.Load();
            }
                    
        }

        private static void UnloadGraph<T>(T obj, ActiveRecordMap activeRecordMap) where T : class, IActiveRecord, new()
        {
            foreach (BelongsToMap belongsToMap in activeRecordMap.BelongsToMappings)
            {
                IRelationship relationship = (IRelationship)belongsToMap.Field.GetValue(obj);

                relationship.Unload();
            }

            foreach (HasOneMap hasOneMap in activeRecordMap.HasOneMappings)
            {
                IRelationship relationship = (IRelationship)hasOneMap.Field.GetValue(obj);

                relationship.Unload();
            }

            foreach (HasManyMap hasManyMap in activeRecordMap.HasManyMappings)
            {
                IRelationship relationship = (IRelationship)hasManyMap.Field.GetValue(obj);

                relationship.Unload();
            }

            foreach (HasAndBelongsToManyMap hasAndBelongsToManyMap in activeRecordMap.HasAndBelongsToManyMappings)
            {
                IRelationship relationship = (IRelationship)hasAndBelongsToManyMap.Field.GetValue(obj);

                relationship.Unload();
            }

        }

    
    }
}
