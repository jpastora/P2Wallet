using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.DAO
{
    namespace DataAccess.DAO
    {

        public class SqlDAO
        {

            private static SqlDAO _instance;
            private static readonly object _lock = new();

            private string _connectionString;

            private SqlDAO()
            {
                if (string.IsNullOrWhiteSpace(_connectionString))
                {
                    throw new InvalidOperationException(
                        "La cadena de conexión no está configurada. " +
                        "Configure 'ConnectionStrings:DefaultConnection' en appsettings o la variable de entorno 'ConnectionStrings__DefaultConnection'.");
                }
            }

            public static void Configure(string connectionString)
            {
                if (string.IsNullOrWhiteSpace(connectionString))
                {
                    throw new ArgumentException("La cadena de conexión no puede estar vacía.", nameof(connectionString));
                }

                lock (_lock)
                {
                    _instance = null;
                    _instance = new SqlDAO { _connectionString = connectionString };
                }
            }

            public static SqlDAO GetInstance()
            {
                if (_instance == null)
                {
                    lock (_lock)
                    {
                        _instance ??= new SqlDAO();
                    }
                }
                return _instance;
            }

            public void ExecuteProcedure(SqlOperation sqlOperation)
            {
                using (var conn = new SqlConnection(_connectionString))
                {
                    using (var command = new SqlCommand(sqlOperation.ProcedureName, conn)
                    {
                        CommandType = System.Data.CommandType.StoredProcedure
                    })
                    {
                        //Set de los parametros
                        foreach (var param in sqlOperation.Parameters)
                        {
                            command.Parameters.Add(param);
                        }
                        //Ejectura el SP
                        conn.Open();
                        command.ExecuteNonQuery();
                    }

                }
            }

            public List<Dictionary<string, object>> ExecuteQueryProcedure(SqlOperation sqlOperation)
            {

                var lstResults = new List<Dictionary<string, object>>();

                using (var conn = new SqlConnection(_connectionString))

                {
                    using (var command = new SqlCommand(sqlOperation.ProcedureName, conn)
                    {
                        CommandType = System.Data.CommandType.StoredProcedure
                    })
                    {
                        foreach (var param in sqlOperation.Parameters)
                        {
                            command.Parameters.Add(param);
                        }
                        conn.Open();

                        var reader = command.ExecuteReader();

                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {

                                var rowDict = new Dictionary<string, object>();

                                for (var index = 0; index < reader.FieldCount; index++)
                                {
                                    var key = reader.GetName(index);
                                    var value = reader.GetValue(index);
                                    rowDict[key] = value;
                                }
                                lstResults.Add(rowDict);
                            }
                        }

                    }
                }

                return lstResults;
            }
        }



    }
}
