using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

using ActiveRecord.Interfaces;

namespace ActiveRecord.Objects.Internal.Mapping
{
    public class ActiveRecordMap
    {
        private Type _type;
        
        public TableMap TableMap;

        public PrimaryKeyColumnMap PrimaryKeyColumnMap;

        public Dictionary<String, ColumnMap> ColumnMappings = new Dictionary<String, ColumnMap>();
        public List<BelongsToMap> BelongsToMappings = new List<BelongsToMap>();
        public List<HasOneMap> HasOneMappings = new List<HasOneMap>();
        public List<HasManyMap> HasManyMappings = new List<HasManyMap>();
        public List<HasAndBelongsToManyMap> HasAndBelongsToManyMappings = new List<HasAndBelongsToManyMap>();

        public ActiveRecordMap(Type type)
        {
            _type = type;
            BuildMap();
        }

        public string TableName
        {
            get
            {
                return TableMap.TableName;
            }
        }

        public string PrimaryKeyColumnName
        {
            get
            {
                return PrimaryKeyColumnMap.ColumnName;
            }
        }

        public List<string> ColumnList(bool includePrimaryKey)
        {
            List<string> columnList = new List<string>();

            if (PrimaryKeyColumnMap != null && includePrimaryKey)
            {
                columnList.Add(PrimaryKeyColumnMap.ColumnName);
            }

            foreach(ColumnMap columnMap in ColumnMappings.Values)
            {
                columnList.Add(columnMap.ColumnName);
            }

            return columnList;
        }
        
        private void BuildMap()
        {
            ProcessClassAttributes();
            ProcessPropertyAttributes();
            ProcessRelationships();
        }

        private void ProcessClassAttributes()
        {
            foreach (object attribute in _type.GetCustomAttributes(false))
            {
                if (attribute is TableAttribute)
                {
                    TableMap = new TableMap(_type, attribute as TableAttribute);
                }   
            }
        }

        private void ProcessPropertyAttributes()
        {
            
            foreach (PropertyInfo property in _type.GetProperties())
            {
                foreach (object attribute in property.GetCustomAttributes(false))
                {
                    if (attribute is ColumnAttribute)
                    {
                        ColumnMap columnMap = new ColumnMap(property, attribute as ColumnAttribute);
                        ColumnMappings.Add(columnMap.ColumnName, columnMap);
                    }

                    if (attribute is PrimaryKeyColumnAttribute)
                    {
                        PrimaryKeyColumnMap = new PrimaryKeyColumnMap(property, attribute as PrimaryKeyColumnAttribute);
                    }
                }
            }

        }

        private void ProcessRelationships()
        {

            foreach (FieldInfo fieldInfo in _type.GetFields())
            {
                //TODO: Compare actual types rather than type names

                if (fieldInfo.FieldType.Name.Contains("BelongsTo") && !fieldInfo.FieldType.Name.Contains("HasAndBelongsToMany"))
                {
                    BelongsToMappings.Add(new BelongsToMap(fieldInfo));
                }

                if (fieldInfo.FieldType.Name.Contains("HasOne"))
                {
                    HasOneMappings.Add(new HasOneMap(fieldInfo));
                }

                if (fieldInfo.FieldType.Name.Contains("HasMany"))
                {
                    HasManyMappings.Add(new HasManyMap(fieldInfo));
                }

                if (fieldInfo.FieldType.Name.Contains("HasAndBelongsToMany"))
                {
                    HasAndBelongsToManyMappings.Add(new HasAndBelongsToManyMap(fieldInfo));
                }

            }
        }     
    
    }
}
