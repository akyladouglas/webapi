using System;
using System.Data.SqlClient;

namespace AtendTeleMedicina.Infrastructure.Data.Context
{
    public class SqlServerContext : ISqlServerContext
    {
        private readonly string _connectionString;
        private SqlConnection _connection;

        public SqlConnection Connection => _connection ?? (_connection = new SqlConnection(_connectionString));

        //public SqlServerContext() => _connectionString = new SqlConnectionStringBuilder(
        //    @"Server=localhost;Initial Catalog=sgcidade;Persist Security Info=False;User ID=dev;
        //    Password=@@321ved;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;").ToString();

        public SqlServerContext() => _connectionString = new SqlConnectionStringBuilder(
            @"server=localhost;database=sgcidade;user=dev;password=@@321ved;min pool size=20;max pool size=150;Persist Security Info=False").ToString();

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }
    }
}
