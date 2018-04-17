using System;
using System.Collections.Generic;
using System.Text;
using System.Data;

namespace ActiveRecord.Services
{
    public class SqlBuilderServices
    {
        public enum ColumnListType
        {
            List,
            ParameterizedList,
            UpdateParameterizedList,
            WhereParameterizedList
        };
        
        public static String BuildSqlColumnList(List<string> columnList, String tableName, ColumnListType columnListType)
        {
            //TODO: Use change to a string builder
            String sqlColumnList = string.Empty;
            String parameterPreface = "@";

            foreach (String column in columnList)
            {
                if (columnListType == ColumnListType.List)
                {
                    sqlColumnList += tableName + "." + column + ", ";
                }
                else if (columnListType == ColumnListType.ParameterizedList)
                {
                    sqlColumnList += parameterPreface + column + ", ";
                }
                else if (columnListType == ColumnListType.UpdateParameterizedList)
                {
                    sqlColumnList += tableName + "." + column + " = " + parameterPreface + column + ", ";                
                }
                else if (columnListType == ColumnListType.WhereParameterizedList)
                {
                    sqlColumnList += tableName + "." + column + " = " + parameterPreface + column + " AND ";                
                }
            }

            if (columnListType == ColumnListType.WhereParameterizedList)
            {
                //Remove last " AND "
                sqlColumnList = sqlColumnList.Substring(0, sqlColumnList.Length - 5);            
            }
            else
            {
                //Remove last ", "
                sqlColumnList = sqlColumnList.Substring(0, sqlColumnList.Length - 2);            
            }

            return sqlColumnList;
        }

        public static DbType GetDBTypeFromSystemType(Type type)
        {
            switch (type.ToString())
            {
                //TODO: Figure out how to get around the TEXT field != NVARCHAR when used in comparisons
                case "System.String":
                    return DbType.String;
                    break;
                case "System.Int16":
                    return DbType.Int16;
                    break;
                case "System.Int32":
                    return DbType.Int32;
                    break;
                case "System.Int64":
                    return DbType.Int64;
                    break;
                case "System.Boolean":
                    return DbType.Boolean;
                    break;
                case "System.Double":
                    return DbType.Double;
                    break;
                case "System.Decimal":
                    return DbType.Decimal;
                    break;
                case "System.DateTime":
                    return DbType.DateTime;
                    break;
                case "System.Byte[]":
                    return DbType.Binary;
                    break;
                case "System.Guid":
                    return DbType.Guid;
                    break;
            }

            return DbType.String;
        }

    
    }
}
