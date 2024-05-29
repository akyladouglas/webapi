using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;
using System;

namespace AtendTeleMedicina.Infra.Data.Context
{
    public class MySqlContext : IMySqlContext
    {
        private readonly string _connectionString;
        private MySqlConnection _connection;

        public MySqlConnection Connection => _connection ?? (_connection = new MySqlConnection(_connectionString));

        public MySqlContext(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }
        //@"server=54.233.175.92;database=eas_covid19;user=covid19;password=441d755;
        //MinPoolSize = 0; MaxPoolSize=100;PersistSecurityInfo=False;ConnectionTimeout=7200;Pooling=true;").ToString();
        //@"server=54.233.175.92;database=eas_homologacao_covid19;user=covid19;password=441d755;
        //MinPoolSize=0;MaxPoolSize=100;PersistSecurityInfo=False;ConnectionTimeout=7200;Pooling=true;").ToString();
        //@"server=novetech.czbzgask0p9x.sa-east-1.rds.amazonaws.com;database=eas_covid19;user=homologacao;password=441d755;
        //minpoolsize=0;maxpoolsize=100;persistsecurityinfo=false;connectiontimeout=7200;pooling=true;").ToString();

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }
    }
}
