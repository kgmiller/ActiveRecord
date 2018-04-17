using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Text;

using ActiveRecord.Interfaces;

namespace ActiveRecord.Services
{
    /// <summary>
    /// Houses functionality related to database integration.
    /// </summary>
    public class DatabaseServices
    {
        private IDbProvider _dbProvider;

        public IDbProvider DbProvider
        {
            get { return _dbProvider; }
            set { _dbProvider = value; }
        }

        public DatabaseServices(IDbProvider dbProvider)
        {
            DbProvider = dbProvider;
        }

        /// <summary>
        /// Creates a new database connection.
        /// </summary>
        /// <returns>A new database connection.</returns>
        public DbConnection CreateConnection()
        {
            return DbProvider.CreateConnection();
        }

        /// <summary>
        /// Creates a new data adapter.
        /// </summary>
        /// <returns>A new data adapter.</returns>
        public DataAdapter CreateDataAdapter(DbCommand selectCommand)
        {
            return DbProvider.CreateDataAdapter(selectCommand);
        }

        /// <summary>
        /// Adds a parameter with the specified attributes to the specified command.
        /// </summary>
        /// <param name="cmd">The command to add a parameter to.</param>
        /// <param name="name">The parameter name.</param>
        /// <param name="type">The type of the parameter.</param>
        /// <param name="value">The initial value for the parameter.</param>
        public void AddParameter(DbCommand cmd, string name, DbType type, object value)
        {
            DbParameter parameter = cmd.CreateParameter();
            parameter.ParameterName = name;
            parameter.DbType = type;
            parameter.Value = value;            
            cmd.Parameters.Add(parameter);
        }

        /// <summary>
        /// Executes command text against the database returning a data table.
        /// </summary>
        /// <param name="commandText">The command text to execute.</param>
        /// <returns>A data table populated with the results of the command.</returns>
        public DataTable ExecuteDataTable(string commandText)
        {
            DataSet retVal = new DataSet();

            using (DbConnection con = CreateConnection())
            {
                con.Open();
                using (DbCommand cmd = con.CreateCommand())
                {
                    cmd.CommandText = commandText;

                    using (DataAdapter adapter = CreateDataAdapter(cmd))
                    {
                        adapter.Fill(retVal);
                    }
                }
            }

            return retVal.Tables[0];
        }

        /// <summary>
        /// Executes command text against the database returning a data reader.
        /// </summary>
        /// <param name="commandText">The command text to execute.</param>
        /// <returns>A data reader containing the results of the command.</returns>
        public IDataReader ExecuteDataReader(string commandText)
        {
            return ExecuteDataReader(commandText, new SqlParameter[0]);
        }

        /// <summary>
        /// Executes command text with parameters against the database returning a data reader.
        /// </summary>
        /// <param name="commandText">The command text to execute.</param>
        /// <param name="parameters">The parameters to use.</param>
        /// <returns>A data reader containing the results of the command.</returns>
        public IDataReader ExecuteDataReader(string commandText, params SqlParameter[] parameters)
        {
            // Create a connection but don't dispose of it (let the reader do it).
            DbConnection con = CreateConnection();
            con.Open();

            // Create the command.
            DbCommand cmd = con.CreateCommand();
            cmd.CommandText = commandText;
            cmd.CommandType = CommandType.Text;
            foreach (SqlParameter abridgedParameter in parameters)
            {
                AddParameter(cmd, abridgedParameter.Name, abridgedParameter.Type, abridgedParameter.Value);
            }

            // Return the reader from the command.
            return cmd.ExecuteReader(CommandBehavior.CloseConnection);
        }

        /// <summary>
        /// Executes command text against the database with no return value.
        /// </summary>
        /// <param name="commandText">The command text to execute.</param>
        public void ExecuteNonQuery(string commandText)
        {
            ExecuteNonQuery(commandText, new SqlParameter[0]);
        }

        /// <summary>
        /// Executes command text against the database with parameters and no return value.
        /// </summary>
        /// <param name="commandText">The command text to execute.</param>
        /// <param name="parameters">The parameters to use.</param>
        public void ExecuteNonQuery(string commandText, params SqlParameter[] parameters)
        {
            using (DbConnection con = CreateConnection())
            {
                con.Open();
                using (DbCommand cmd = con.CreateCommand())
                {
                    cmd.CommandText = commandText;
                    cmd.CommandType = CommandType.Text;
                    foreach (SqlParameter abridgedParameter in parameters)
                    {                        
                        AddParameter(cmd, abridgedParameter.Name, abridgedParameter.Type, abridgedParameter.Value);
                    }

                    cmd.ExecuteNonQuery();
                }
            }
        }

        public object ExecuteScalar(string commandText, params SqlParameter[] parameters)
        {
            using (DbConnection con = CreateConnection())
            {
                con.Open();
                using (DbCommand cmd = con.CreateCommand())
                {
                    cmd.CommandText = commandText;
                    cmd.CommandType = CommandType.Text;
                    foreach (SqlParameter abridgedParameter in parameters)
                    {
                        AddParameter(cmd, abridgedParameter.Name, abridgedParameter.Type, abridgedParameter.Value);
                    }

                    return cmd.ExecuteScalar();
                }
            }
        }
    }
}
